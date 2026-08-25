using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestInProgressUISystem: MonoBehaviour
{
    static QuestInProgressUISystem instance;

	class SingleProgressUI
	{
		public GameObject panel;
		public TextMeshProUGUI title;
		public TextMeshProUGUI[] progresses;
		public SingleProgressUI(GameObject panel, TextMeshProUGUI title, TextMeshProUGUI[] progresses)
		{
			this.panel = panel;
			this.title = title;
			this.progresses = progresses;
		}
	}
	private void Awake()
	{
		if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
	}
    [Header("Quest Progress Panel")]
    [SerializeField]
    GameObject[] questInProgressPanel;

    // Refer obj in runtime(void start)
    TextMeshProUGUI[] questTitles;

    // Refer obj in runtime(void start)
    TextMeshProUGUI[][] questProgressText;


    List<SubscriptionToken> subscriptionTokens;

    Dictionary<string, SingleProgressUI> progressUIContainer;

    const int maxQuestShownCnt = 5;
    const int maxQuestConditionCnt = 3;
    int curQuestShownCnt;

    private void OnEnable()
	{
		subscriptionTokens = new List<SubscriptionToken>();
		subscriptionTokens.Add(EventBus.Subscribe<QuestProgressUpdatedEvent>(OnQuestProgressUpdated));
		subscriptionTokens.Add(EventBus.Subscribe<QuestAcceptedEvent>(OnQuestAccepted));
		subscriptionTokens.Add(EventBus.Subscribe<QuestCompletedEvent>(OnQuestCompleted));
	}
    private void Start()
    {
        // QuestInProgressPanel[] 에서 progressUIContainer로 가져옴.
        // 이러면 런타임에 QuestId로 각 패널에 접근 가능해짐. 
        progressUIContainer = new Dictionary<string, SingleProgressUI>();
        curQuestShownCnt = 0;

        // panel에서 getCompInChildren으로 TMP 가져오기
        questTitles = new TextMeshProUGUI[maxQuestShownCnt];
        questProgressText = new TextMeshProUGUI[maxQuestShownCnt][];
        for (int i = 0; i < questInProgressPanel.Length; i++)
        {
            questProgressText[i] = new TextMeshProUGUI[maxQuestConditionCnt];
            var textGUIs = questInProgressPanel[i].GetComponentsInChildren<TextMeshProUGUI>();
            questTitles[i] = textGUIs[0];
            for (int j = 0; j < maxQuestConditionCnt; j++)
                questProgressText[i][j] = textGUIs[j + 1];

            questInProgressPanel[i].SetActive(false);
		}

        // Quest 데이터 갱신(QuestStates가 Awake에서 초기화돼서 여기는 Start여야됨)
		foreach (var qid in QuestManager.Instance.QuestStates.Keys)
		{
			SetNewQuestProgressUI(qid);
		}
	}
    void SetNewQuestProgressUI(string questId)
	{
		progressUIContainer.Add(questId, new SingleProgressUI(
			questInProgressPanel[curQuestShownCnt],
			questTitles[curQuestShownCnt],
			questProgressText[curQuestShownCnt]));

		// get quest
		var quest = QuestManager.Instance.QuestStates[questId];

		// set title
		progressUIContainer[questId].title.text = quest.data.title;

		// set condition texts
		for (int i = 0; i < maxQuestConditionCnt; i++)
		{
			if (i < quest.progress.Count)
				progressUIContainer[questId].progresses[i].text = quest.progress[i].UIText;
			else
				progressUIContainer[questId].progresses[i].text = "";
		}


		// active panel
		questInProgressPanel[Mathf.Min(curQuestShownCnt, maxQuestShownCnt - 1)].SetActive(true);

		curQuestShownCnt++;
	}
	void OnQuestProgressUpdated(QuestProgressUpdatedEvent ev)
	{
		if (QuestManager.Instance.QuestStates[ev.state.data.id].isCompleted)
		{
			progressUIContainer[ev.state.data.id].progresses[0].text = "퀘스트 완료!";
			for (int i = 1; i < ev.state.data.conditionContexts.Length; i++)
			{
				progressUIContainer[ev.state.data.id].progresses[i].text = "";
			}
		}
		else
		{
			for (int i = 0; i < ev.state.data.conditionContexts.Length; i++)
			{
				progressUIContainer[ev.state.data.id].progresses[i].text = ev.state.progress[i].UIText;
			}
		}
	}

	void OnQuestAccepted(QuestAcceptedEvent ev)
    {
		SetNewQuestProgressUI(ev.ctx.id);
    }
    void OnQuestCompleted(QuestCompletedEvent ev)
    {
        progressUIContainer[ev.ctx.id].panel.SetActive(false);
	}
	private void OnDisable()
	{
		if (subscriptionTokens != null)
			foreach (var token in subscriptionTokens)
			{
				EventBus.Unsubscribe(token);
			}
	}
}
