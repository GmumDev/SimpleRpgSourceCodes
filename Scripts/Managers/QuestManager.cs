using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QuestManager : MonoBehaviour, IQuestManager, IQuestRewardEarner
{
	[Serializable]
	public class QuestState
	{
		public QuestContext data;
		public List<QuestConditionProgress> progress;
		public bool[] isConditionCompleted;
		public bool isCompleted 
		{ 
			get
			{
				foreach (bool v in isConditionCompleted)
					if (!v) return false;
				return true;
			}
		}
		public bool HasConditionType(QuestConditionType type)
		{
			foreach(var c in progress)
			{
				if (c.type == type) return true;
			}
			return false;
		}

        public QuestState(QuestContext data, List<QuestConditionProgress> progress)
        {
            this.data = data;
            this.progress = progress;
			isConditionCompleted = new bool[progress.Count];
        }

	}
	private static QuestManager instance;
	public static IQuestManager Instance { get => instance; }

	[SerializeField] string questDataKey;
	QuestSaveDataSO questStates;
	public Dictionary<string, QuestState> QuestStates { get => questStates.datas; }

	List<SubscriptionToken> subscriptionTokens = new List<SubscriptionToken>();

	QuestRewardService rewardService = new QuestRewardService();

	private async void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
			return;
		}
		while (AddressableGroupLoader.Instance.IsLoaded == false)
		{
			await Task.Yield();
		}
		questStates = AddressableGroupLoader.Instance.GetSaveData<QuestSaveDataSO>(questDataKey);

		questStates.Deserialize();
	}

	private void OnEnable()
	{
		subscriptionTokens.Add(EventBus.Subscribe<InventoryChangedEvent>(OnInventoryChanged));
		subscriptionTokens.Add(EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled));
		subscriptionTokens.Add(EventBus.Subscribe<ScenarioNodeFinishedEvent>(OnScenarioNodeFinished));
	}
	private void Start()
	{
		foreach(var quest in questStates.datas)
		{
			EventBus.Publish(new QuestCompletionChangedEvent(quest.Value.data.id, quest.Value.data.followUpScenarioId, quest.Value.isCompleted));
		}
	}
	void IQuestManager.CompleteQuest(string questID)
	{
		if (questStates.datas.ContainsKey(questID) == false) return;
		if (questStates.datas[questID].isCompleted == false) return;

		(this as IQuestRewardEarner).EarnExpReward(questStates.datas[questID].data.expRewardAmount);
		foreach (var ctx in questStates.datas[questID].data.rewardContexts)
		{
            rewardService.Give(this, ctx);
		}

		EventBus.Publish(new QuestCompletedEvent(questStates.datas[questID].data));


		questStates.datas.Remove(questID);
	}
	void IQuestManager.AcceptQuest(string questID)
	{
		if (questStates.datas.ContainsKey(questID)) return;

		var context = SOLoader<QuestSO>.Instance.GetSO(questID).ToContext();
		List<QuestConditionProgress> _progresses = new List<QuestConditionProgress>();
		HashSet<QuestConditionType> conditionTypes = new HashSet<QuestConditionType>();
		foreach(var ctx in context.conditionContexts)
        {
            conditionTypes.Add(ctx.type);
            switch (ctx.type)
			{
				case QuestConditionType.Obtain:
                    _progresses.Add(QuestConditionProgress.GetObtainConditionProgress(ctx.itemID, ctx.itemCntToObtain));
                    break;
				case QuestConditionType.Kill:
                    _progresses.Add(QuestConditionProgress.GetKillConditionProgress(ctx.enemyId, ctx.enemyCntToKill));
                    break;
			}
		}
		QuestState _questState = new QuestState(
			data: context,
			progress: _progresses
		);
		questStates.datas.Add(questID, _questState);

        EventBus.Publish(new QuestAcceptedEvent(context));
	}

	//
	void OnInventoryChanged(InventoryChangedEvent ev)
	{
		foreach (var _questState in questStates.datas.Values)
		{
			if (_questState.HasConditionType(QuestConditionType.Obtain) == false) continue;

			bool old_completion = _questState.isCompleted;
			bool isUpdated = false;
			for (int i = 0; i < _questState.progress.Count; i++)
            {
                var condition = _questState.data.conditionContexts[i];
				if (condition.type != QuestConditionType.Obtain) continue;
                if (condition.itemID != ev.itemId) continue;

                var progress = _questState.progress[i];
				progress.curAmount += ev.deltaCnt;
				isUpdated = true;

				_questState.isConditionCompleted[i] = QuestConditionService.IsMet(this, condition, progress);
            }

            bool new_completion = _questState.isCompleted;

            if (new_completion != old_completion)
			{
				EventBus.Publish(new QuestCompletionChangedEvent(_questState.data.id, _questState.data.followUpScenarioId, new_completion));
			}
			if (isUpdated) EventBus.Publish(new QuestProgressUpdatedEvent(_questState));
		}
	}
	void OnEnemyKilled(EnemyKilledEvent ev)
	{
		foreach(var _questState in questStates.datas.Values)
        {
            if (_questState.HasConditionType(QuestConditionType.Kill) == false) continue;
			if (_questState.isCompleted) continue;  // Enemy Kill Count cannot be reduced. No need to double check. 

            bool isUpdated = false;
            for (int i = 0; i < _questState.progress.Count; i++)
            {
                var condition = _questState.data.conditionContexts[i];
				if (condition.type != QuestConditionType.Kill) continue;
				if (condition.enemyId != ev.enemyId) continue;

                var progress = _questState.progress[i];

				progress.curAmount += ev.enemyKilledCnt;
				isUpdated = true;

                _questState.isConditionCompleted[i] = QuestConditionService.IsMet(this, condition, progress);
            }
			bool completion = _questState.isCompleted;

            if (completion) // Enemy Kill Count cannot be reduced. SO definitely changed to TRUE
            {
				EventBus.Publish(new QuestCompletionChangedEvent(_questState.data.id, _questState.data.followUpScenarioId, completion));
			}
			if (isUpdated)
			{
				EventBus.Publish(new QuestProgressUpdatedEvent(_questState));
			}
		}
	}
	void OnScenarioNodeFinished(ScenarioNodeFinishedEvent ev)
	{
		if((ev.eventType & ScenarioNodeFinishedEventType.FollowUpQuest) == ScenarioNodeFinishedEventType.FollowUpQuest)
        {
            (this as IQuestManager).AcceptQuest(ev.followUpQuestId);
		}
		if ((ev.eventType & ScenarioNodeFinishedEventType.CompleteQuest) == ScenarioNodeFinishedEventType.CompleteQuest)
		{
			(this as IQuestManager).CompleteQuest(ev.CompleteQuestId);
		}
	}

	void IQuestRewardEarner.EarnItemReward(string id, int cnt) => InventorySystem.Instance.ObtainItem(id, cnt);
	void IQuestRewardEarner.EarnGoldReward(int cnt) => throw new System.NotImplementedException();
	void IQuestRewardEarner.EarnExpReward(int cnt) => Player.GainExp(cnt);
	

	void OnDisable()
    {
		if(subscriptionTokens != null)
			foreach(var token in subscriptionTokens)
				EventBus.Unsubscribe(token);
    }
    private void OnApplicationQuit()
    {
        questStates.Serialize();
        SaveDataOverwriter.Instance.SaveToFile(questStates);
    }

}
