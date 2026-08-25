using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class QuestBookUISystem : MonoBehaviour
{
	static QuestBookUISystem instance;
	public static QuestBookUISystem Instance { get => instance; }
	
	[Serializable]
	class RightContents
	{
		public TextMeshProUGUI title;
		public TextMeshProUGUI discription;
		public List<TextMeshProUGUI> conditions;
		public List<TextMeshProUGUI> rewards;
		public TextMeshProUGUI expReward;
	}

	[Header("Quest Book UI")]
	[SerializeField]
	GameObject questBookPanel;

	[Header("Left Contents")]
	[SerializeField]
	RectTransform leftContentParent;
	[SerializeField]
	GameObject questPaperPrefab;
	IObjectPool<QuestPaperOfQuestBook> paperPool;
	Dictionary<string, QuestPaperOfQuestBook> idToPaper;

	[Header("Right Contents")]
	[SerializeField] GameObject clickedPanel;
	[SerializeField] GameObject waitingPanel;
	[SerializeField] RightContents rightContents;

	bool isOpened;
	public bool IsOpened { get => isOpened; }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
			return;
		}
		idToPaper = new Dictionary<string, QuestPaperOfQuestBook>();
		paperPool = new ObjectPool<QuestPaperOfQuestBook>(
			createFunc: CreateItem,
			actionOnGet: OnGet,
			actionOnRelease: OnRelease,
			actionOnDestroy: OnDestroyItem,
			collectionCheck: true,   // helps catch double-release mistakes
			defaultCapacity: 5,
			maxSize: 20
		);
		tokens = new List<SubscriptionToken>();
		tokens.Add(EventBus.Subscribe<QuestAcceptedEvent>(OnQuestAccepted));
		tokens.Add(EventBus.Subscribe<QuestCompletedEvent>(OnQuestCompleted));
	}
	private void Start()
	{
		// Quest 데이터 갱신(QuestStates가 Awake에서 초기화돼서 여기는 Start여야됨)
		foreach (var qid in QuestManager.Instance.QuestStates.Keys)
		{
			if (idToPaper.ContainsKey(qid))
			{
				throw new Exception("이미 퀘스트 받았는데 왜 또받음");
			}
			idToPaper.Add(qid, paperPool.Get());
			idToPaper[qid].Init(OnClickedQuest, qid, QuestManager.Instance.QuestStates[qid].data.title);
		}
	}
	List<SubscriptionToken> tokens;
	void UpdateUI(string qid)
	{
		var quest = QuestManager.Instance.QuestStates[qid];
		// set title
		rightContents.title.text = quest.data.title;
		rightContents.discription.text = quest.data.descript;
		for(int i = 0; i < rightContents.conditions.Count; i++)
		{
			// set condition texts
			if (i < quest.progress.Count)
				rightContents.conditions[i].text = quest.progress[i].UIText;
			else
				rightContents.conditions[i].text = "";
		}

		for(int i = 0; i < rightContents.rewards.Count; i++)
		{
			// set rewards texts
			if (i < quest.data.rewardContexts.Length)
				rightContents.rewards[i].text = quest.data.rewardContexts[i].UIText;
			else
				rightContents.rewards[i].text = "";
		}
		// set exp reward text
		rightContents.expReward.text = "경험치 +" + quest.data.expRewardAmount.ToString();

		ToggleRightPanel(true);
	}
	void ToggleRightPanel(bool isClicked)
	{
		clickedPanel.SetActive(isClicked);
		waitingPanel.SetActive(!isClicked);
	}
	void OnQuestAccepted(QuestAcceptedEvent ev)
	{
		if(idToPaper.ContainsKey(ev.ctx.id))
		{
			throw new Exception("이미 퀘스트 받았는데 왜 또받음");
		}
		idToPaper.Add(ev.ctx.id, paperPool.Get());
		idToPaper[ev.ctx.id].Init(OnClickedQuest, ev.ctx.id, ev.ctx.title);
	}
	void OnQuestCompleted(QuestCompletedEvent ev)
	{
		if (idToPaper.ContainsKey(ev.ctx.id) == false)
		{
			throw new Exception("받은 적이 없는 퀘스트를 어떻게 완료함");
		}
		paperPool.Release(idToPaper[ev.ctx.id]);
		idToPaper.Remove(ev.ctx.id);
	}
	public void ToggleBook()
	{
		if (isOpened) CloseBook();
		else OpenBook();
	}
	public void OpenBook()
	{
		isOpened = true;
		questBookPanel.SetActive(true);
		ToggleRightPanel(false);
	}
	public void CloseBook()
	{
		isOpened = false;
		questBookPanel.SetActive(false);
	}
	public void OnClickedQuest(string questId)
	{
		UpdateUI(questId);
	}
	private void OnDestroy()
	{
		if(tokens != null)
			foreach (var token in tokens)
				EventBus.Unsubscribe(token);
	}

	#region ObjectPool
	private QuestPaperOfQuestBook CreateItem()
	{
		GameObject gameObject = Instantiate(questPaperPrefab, leftContentParent);

		gameObject.SetActive(false);
		return gameObject.GetComponent<QuestPaperOfQuestBook>();
	}

	// Called when an item is taken from the pool.
	private void OnGet(QuestPaperOfQuestBook obj)
	{
		obj.gameObject.SetActive(true);
	}

	// Called when an item is returned to the pool.
	private void OnRelease(QuestPaperOfQuestBook obj)
	{
		obj.gameObject.SetActive(false);
	}

	// Called when the pool decides to destroy an item (e.g., above max size).
	private void OnDestroyItem(QuestPaperOfQuestBook obj)
	{
		Destroy(obj.gameObject);
	}
	#endregion
}
