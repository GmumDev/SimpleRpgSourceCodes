using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour, IScenarioManager, IScenarioContextRunner
{
    private static ScenarioManager instance;
    public static IScenarioManager Instance { get => instance; }

	// UI를 분리 안 한 죄
	[Header("Scenario Panel")]
	[SerializeField]
	GameObject scenarioPanel;
	Animator panelAnim;
    [Header("Choice Panel")]
	[SerializeField]
	GameObject choicesPanel;
	[SerializeField]
	Image[] choicePanels;
	[SerializeField]
	TextMeshProUGUI[] choiceNames;
	[SerializeField]
	Color defaultChoiceColor;
	[SerializeField]
	Color selectedChoiceColor;

	[Header("Dialogue Panel")]
	[SerializeField]
	TextMeshProUGUI dialogueSpeakerUGUI;
	[SerializeField]
	TextMeshProUGUI dialogueTextUGUI;

    ScenarioContext curScenario;
    ScenarioNodeSO curNodeSO;
    ScenarioNodeContext curNodeContext;



    bool isPlaying;
    bool IScenarioManager.IsPlaying => isPlaying;
	int selectedChoiceIndex = 0;
	int maxChoicesNum = 0; // num of choices about current node
	bool isSelecting;

	List<SubscriptionToken> tokens;
	private void Awake()
	{
		if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
			return;
		}
		tokens = new List<SubscriptionToken>();
		//tokens.Add(EventBus.Subscribe<ScenarioNodePlayedEvent>(OnNodePlayed));
		tokens.Add(EventBus.Subscribe<ScenarioNodeFinishedEvent>(OnNodeFinished));
		tokens.Add(EventBus.Subscribe<TimelineFinishedEvent>(OnTimelineFinished));
		panelAnim = scenarioPanel.GetComponent<Animator>();
	}
	private void OnDestroy()
	{
		if (tokens != null)
			foreach (var token in tokens)
				EventBus.Unsubscribe(token);
	}

	// -------------- Event Listener -----------
	//void OnNodePlayed(ScenarioNodePlayedEvent ev)
	//{

	//}
	void OnNodeFinished(ScenarioNodeFinishedEvent ev)
	{
		if(ev.eventType == ScenarioNodeFinishedEventType.DoTimeline)
			scenarioPanel.SetActive(false);
	}
	void OnTimelineFinished(TimelineFinishedEvent ev)
	{
		scenarioPanel.SetActive(true);
		panelAnim.Update(0);
	}


	// -------------- Public APIs -------------
	public bool PlayScenario(string scenarioId)
    {
        if (isPlaying) return false;

		Player.ForceWatchState();
		ScenarioSO scenario = SOLoader<ScenarioSO>.Instance.GetSO(scenarioId);

        isPlaying = true;
        curNodeSO = scenario.startNode;
        curScenario = scenario.ToContext();
        curNodeContext = curNodeSO.ToContext();

		ScenarioService.PlayAsFirstNode(this, curNodeContext);

		scenarioPanel.SetActive(true);
		panelAnim.Update(0);
		return true;
    }
    public void NextNode()
    {
        if (isPlaying == false)
        {
            return;
        }
		
		// publish node finish event
		ScenarioService.FinishNode(this, curNodeContext);


		// get next node, vary to node type
		ScenarioNodeSO nextnodeSO = null;
        switch(curNodeContext.type)
        {
            case ScenarioNodeType.Dialogue:
				nextnodeSO = curNodeContext.nextNode;
                break;
            case ScenarioNodeType.Choice:
				nextnodeSO = curNodeContext.choices[selectedChoiceIndex].nextNode;
                break;
            default:
                throw new NotImplementedException(message: "Set valid type to nodeSO");
		}

		// is last node? then finish. 
		if (nextnodeSO == null)  
		{
			isPlaying = false;
			scenarioPanel.SetActive(false);
			CameraManager.Instance.PrioritizeFollowCam();
			EventBus.Publish(new ScenarioFinishedEvent(curScenario.id));
			return;
		}
		// else, set cur node to next node. And Play. 
		else
		{
			curNodeSO = nextnodeSO;
			curNodeContext = curNodeSO.ToContext();
			ScenarioService.PlayAsNextNode(this, curNodeContext);
		}
    }
	public void SelectChoices(Vector2 navInput)
	{
		if (!isSelecting) return;

		choicePanels[selectedChoiceIndex].color = defaultChoiceColor;
		if (navInput.x > 0) selectedChoiceIndex++;
		if (navInput.x < 0) selectedChoiceIndex--;

		selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, maxChoicesNum - 1);

		choicePanels[selectedChoiceIndex].color = selectedChoiceColor;
		curNodeContext.selectedChoiceIndex = selectedChoiceIndex;
	}

	void IScenarioContextRunner.DoDialogue(ScenarioNodeContext ctx)
	{
		dialogueSpeakerUGUI.text = ctx.speakerStr;
		dialogueTextUGUI.text = ctx.dialogueStr;
    }

    void IScenarioContextRunner.ClearDialogue()
	{
		dialogueSpeakerUGUI.text = "";
		dialogueTextUGUI.text = "";
	}
	void IScenarioContextRunner.DoDialogueWithChoices(ScenarioNodeContext ctx)
	{
		dialogueSpeakerUGUI.text = ctx.speakerStr;
		dialogueTextUGUI.text = ctx.dialogueStr;
		maxChoicesNum = ctx.choices.Length;

		for (int i = 0; i < maxChoicesNum; i++)
        {
            choiceNames[i].text = ctx.choices[i].choiceName;
			choicePanels[i].gameObject.SetActive(true);
		}
		selectedChoiceIndex = 0;
		choicePanels[selectedChoiceIndex].color = selectedChoiceColor;
		isSelecting = true;
		choicesPanel.SetActive(true);
	}

	void IScenarioContextRunner.ClearDialogueAndChoices()
	{
		dialogueSpeakerUGUI.text = "";
		dialogueTextUGUI.text = "";

		for (int i = 0; i < maxChoicesNum; i++)
		{
			choicePanels[i].gameObject.SetActive(false);
		}
		isSelecting = false;
		choicePanels[selectedChoiceIndex].color = defaultChoiceColor;
		choicesPanel.SetActive(false);
	}



}
