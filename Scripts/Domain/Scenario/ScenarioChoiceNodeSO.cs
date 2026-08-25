using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Scenario/ScenarioChoiceNodeSO")]
public class ScenarioChoiceNodeSO : ScenarioNodeSO
{

	public ScenarioChoice[] choices;
	public string speakerStr;
	public string dialogueStr;

	public override ScenarioNodeContext ToContext()
    {
        var context = new ScenarioNodeContext();
		context.type = ScenarioNodeType.Choice;
		context.speakerStr = speakerStr;
		context.dialogueStr = dialogueStr;
        context.scenarioNodePlayedEvent = GetPlayedEvent();
        context.scenarioNodeFinishedEvent = GetFinishedEvent();
        
		context.choices = choices;

        return context;
    }
    
}
[Serializable]
public class ScenarioChoice
{
	public string choiceName;
	public ScenarioNodeSO nextNode;
	// generate event instance on ToContext
	public ScenarioNodeFinishedEventType finishedEventType;
	public string finishedFollowUpQuestId;
	public string finishedCompleteQuestId;
	public string finishedFollowUpScenarioId;
	public string loadSceneName;
	public string timelineId;
	public ScenarioNodeFinishedEvent GetFinishedEvent()
	{
		var obj = new ScenarioNodeFinishedEvent(
			eventType: finishedEventType,
			followUpQuestId: finishedFollowUpQuestId,
		 	CompleteQuestId: finishedCompleteQuestId,
			followUpScenarioId: finishedFollowUpScenarioId,
			loadSceneName: loadSceneName,
			timelineId: timelineId
		);
		return obj;
	}
	// ~ 
}