using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Scenario/DialogueNodeSO")]
public class ScenarioDialogueNodeSO: ScenarioNodeSO
{
	public ScenarioNodeSO nextNode;
	public string speakerStr;
	public string dialogueStr;

    public override ScenarioNodeContext ToContext()
    {
        var context = new ScenarioNodeContext();
        context.speakerStr = speakerStr;
        context.type = ScenarioNodeType.Dialogue;
        context.nextNode = nextNode;
		context.scenarioNodePlayedEvent = GetPlayedEvent();
        context.scenarioNodeFinishedEvent = GetFinishedEvent();
        context.dialogueStr = dialogueStr;

        return context;
    }
    
}
