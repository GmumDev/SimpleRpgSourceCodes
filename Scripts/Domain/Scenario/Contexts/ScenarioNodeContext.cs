
public class ScenarioNodeContext
{
    public ScenarioNodeType type;
	public string speakerStr;
	public string dialogueStr;

	// node event
	public ScenarioNodePlayedEvent scenarioNodePlayedEvent;
	public ScenarioNodeFinishedEvent scenarioNodeFinishedEvent;

	// dialogue
	public ScenarioNodeSO nextNode;
    // choice
    public ScenarioChoice[] choices;
    public int selectedChoiceIndex;

}
