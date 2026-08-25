
public class ScenarioDialogueNodeHandler : IScenarioNodeHandler
{
    void PublishNodePlayedEvent(IScenarioContextRunner ctxRunner, ScenarioNodeContext context)
    {
        ScenarioNodePlayedEvent ev = context.scenarioNodePlayedEvent;
		EventBus.Publish(ev);
	}
    void PublishNodeFinishedEvent(IScenarioContextRunner ctxRunner, ScenarioNodeContext context)
    {
        ScenarioNodeFinishedEvent ev = context.scenarioNodeFinishedEvent;
		EventBus.Publish(ev);
    }
    void IScenarioNodeHandler.FinishNode(IScenarioContextRunner ctxRunner, ScenarioNodeContext context)
    {
        ctxRunner.ClearDialogue();
        PublishNodeFinishedEvent(ctxRunner, context);
    }

    void IScenarioNodeHandler.PlayAsFirstNode(IScenarioContextRunner ctxRunner, ScenarioNodeContext context)
    {
        ctxRunner.DoDialogue(context);
        PublishNodePlayedEvent(ctxRunner, context);
    }

    void IScenarioNodeHandler.PlayAsNextNode(IScenarioContextRunner ctxRunner, ScenarioNodeContext context)
    {
        ctxRunner.DoDialogue(context);
        PublishNodePlayedEvent(ctxRunner, context);
    }
}
