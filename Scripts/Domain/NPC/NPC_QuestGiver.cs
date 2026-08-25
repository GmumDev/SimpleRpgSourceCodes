using System;
using UnityEngine;

public class NPC_QuestGiver : NPC
{
	[SerializeField]
	Transform camTarget;

	[Serializable]
	public class OwnedQuestCompletion
	{
		public string questId;
		public SubscriptionToken onQuestCompletionChangedToken;
		public string originScenarioId;
		public OwnedQuestCompletion(string questId, SubscriptionToken onQuestCompletionChangedToken, string originScenarioId)
		{
			this.questId = questId;
			this.onQuestCompletionChangedToken = onQuestCompletionChangedToken;
			this.originScenarioId = originScenarioId;
		}
	}

	private bool isTalking;
    private SubscriptionToken scenarioFinishedToken;
	private SubscriptionToken scenarioNodeFinishedToken;
	private SubscriptionToken NPCScenarioChangedToken;


	protected async override void Awake()
	{
		base.Awake();
		await awakeCompleted.Task;

		NPCScenarioChangedToken = EventBus.Subscribe<NPCScenarioChangedEvent>(OnNPCScenarioChanged);

		foreach (var completion in npcSo.ownedQuestStates.Values)
		{
			completion.onQuestCompletionChangedToken
				= EventBus.Subscribe<QuestCompletionChangedEvent>(OnQuestCompletionChanged);
		}
	}


	void OnNPCScenarioChanged(NPCScenarioChangedEvent ev)
	{
		foreach(var dependancy in npcSo.NPCDependancies)
		{
			if (dependancy.otherNpcId != ev.npcId
				|| dependancy.otherNpcFromScenarioId != ev.scenarioIdFrom
				|| dependancy.otherNpcToScenarioId != ev.scenarioIdTo
				|| dependancy.myFromScenarioId != npcSo.scenarioId) continue;

			string tmp = npcSo.scenarioId;
			npcSo.scenarioId = dependancy.myToScenarioId;
			EventBus.Publish(new NPCScenarioChangedEvent(npcSo.id, tmp, npcSo.scenarioId));
			return;
		}
	}

	// talking finished
	void OnScenarioFinished(ScenarioFinishedEvent ev)
    {
        EventBus.Unsubscribe(scenarioFinishedToken);
		EventBus.Unsubscribe(scenarioNodeFinishedToken);
		CameraManager.Instance.TogglePlayerCulling();
		isTalking = false;
	}

	// talking finished. 시나리오에 안붙이고 노드에 후속 퀘, 시나리오 붙이는 이유는 선택지 때문
	void OnScenarioNodeFinished(ScenarioNodeFinishedEvent ev)
	{
		if((ev.eventType & ScenarioNodeFinishedEventType.FollowUpQuest) == ScenarioNodeFinishedEventType.FollowUpQuest
			&& (ev.eventType & ScenarioNodeFinishedEventType.CompleteQuest) == ScenarioNodeFinishedEventType.CompleteQuest)
		{
			Debug.LogWarning("FollowUp Quest, And also Complete that quest!! both are incompatable!!");
		}
		if ((ev.eventType & ScenarioNodeFinishedEventType.FollowUpScenario) == ScenarioNodeFinishedEventType.FollowUpScenario)   // Subscribe During NPC talking ~ NPC talk finish
		{
			string tmp = npcSo.scenarioId;
			npcSo.scenarioId = ev.followUpScenarioId;
			EventBus.Publish(new NPCScenarioChangedEvent(npcSo.id, tmp, npcSo.scenarioId));
		}

		// Gived Quest in this node. Subscribe Completion. Auto-Complete and switch scenario via event. 
		if ((ev.eventType & ScenarioNodeFinishedEventType.FollowUpQuest) == ScenarioNodeFinishedEventType.FollowUpQuest)   // Subscribe During NPC talking ~ NPC talk finish
		{
			npcSo.ownedQuestStates.Add(ev.followUpQuestId, new OwnedQuestCompletion(
					questId: ev.followUpQuestId,
					onQuestCompletionChangedToken: EventBus.Subscribe<QuestCompletionChangedEvent>(OnQuestCompletionChanged),
					originScenarioId: npcSo.scenarioId
			));
		}

		// NPC Satisfied for this conversation. Actual complete. 
		if ((ev.eventType & ScenarioNodeFinishedEventType.CompleteQuest) == ScenarioNodeFinishedEventType.CompleteQuest)	
		{
			if (npcSo.ownedQuestStates.ContainsKey(ev.CompleteQuestId))
			{
				// unsub complete listener
				EventBus.Unsubscribe(npcSo.ownedQuestStates[ev.CompleteQuestId].onQuestCompletionChangedToken);

				// remove npc's given(and player-running) quest from dict
				npcSo.ownedQuestStates.Remove(ev.CompleteQuestId);

				// complete. Get Reward. 
				QuestManager.Instance.CompleteQuest(ev.CompleteQuestId);
			}
		}
	}

	// Kill, Obtain 등 퀘스트 progress 변화에 따른 completion 변화의 경우
	// Complete then set npcSO.sencarioID to followupscenario, else origin
	void OnQuestCompletionChanged(QuestCompletionChangedEvent ev)	
	{
		if(npcSo.ownedQuestStates.ContainsKey(ev.questId))
		{
			string tmp = npcSo.scenarioId;
			if (ev.isCompleted)
				npcSo.scenarioId = ev.followUpScenarioId;
			else
				npcSo.scenarioId = npcSo.ownedQuestStates[ev.questId].originScenarioId;

			// changed scenarioid
			if(tmp != npcSo.scenarioId)
			{
				EventBus.Publish(new NPCScenarioChangedEvent(npcSo.id, tmp, npcSo.scenarioId));
			}
		}
	}
	
    public override void OnInteract()
    {
        isTalking = ScenarioManager.Instance.PlayScenario(npcSo.scenarioId);

        if (isTalking == false) return;

		if (scenarioFinishedToken != null)
			EventBus.Unsubscribe(scenarioFinishedToken);
		if (scenarioNodeFinishedToken != null)
			EventBus.Unsubscribe(scenarioNodeFinishedToken);

		scenarioFinishedToken = EventBus.Subscribe<ScenarioFinishedEvent>(OnScenarioFinished);
        scenarioNodeFinishedToken = EventBus.Subscribe<ScenarioNodeFinishedEvent>(OnScenarioNodeFinished);
		

        CameraManager.Instance.FocusTalker(camTarget);
		CameraManager.Instance.TogglePlayerCulling();
	}

	protected void OnDisable()
    {
        if (scenarioFinishedToken != null)
            EventBus.Unsubscribe(scenarioFinishedToken);
        if (scenarioNodeFinishedToken != null)
            EventBus.Unsubscribe(scenarioNodeFinishedToken);
        if (NPCScenarioChangedToken != null)
            EventBus.Unsubscribe(NPCScenarioChangedToken);
	}
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }
}
