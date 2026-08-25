
public interface IScenarioContextRunner
{
    void DoDialogue(ScenarioNodeContext ctx);
    void ClearDialogue();

	void DoDialogueWithChoices(ScenarioNodeContext ctx);
	void ClearDialogueAndChoices();
}
