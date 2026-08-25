
public class ScenarioChoiceNodeHandler : IScenarioNodeHandler
{
    void PublishNodePlayedEvent(IScenarioContextRunner ctxRunner, ScenarioNodeContext context)
    {
        ScenarioNodePlayedEvent ev = context.scenarioNodePlayedEvent;
		EventBus.Publish(ev);
	}
    void PublishNodeFinishedEvent(IScenarioContextRunner ctxRunner, ScenarioNodeContext context)
    {
        ScenarioNodeFinishedEvent ev1 = context.choices[context.selectedChoiceIndex].GetFinishedEvent();
		EventBus.Publish(ev1);
		ScenarioNodeFinishedEvent ev2 = context.scenarioNodeFinishedEvent;
		EventBus.Publish(ev2);
	}
    void IScenarioNodeHandler.FinishNode(IScenarioContextRunner ctxRunner, ScenarioNodeContext context)
    {
        ctxRunner.ClearDialogueAndChoices();
        PublishNodeFinishedEvent(ctxRunner, context);
    }

    void IScenarioNodeHandler.PlayAsFirstNode(IScenarioContextRunner ctxRunner, ScenarioNodeContext context)
    {
        ctxRunner.DoDialogueWithChoices(context);
        PublishNodePlayedEvent(ctxRunner, context);
    }

    void IScenarioNodeHandler.PlayAsNextNode(IScenarioContextRunner ctxRunner, ScenarioNodeContext context)
    {
        ctxRunner.DoDialogueWithChoices(context);
        PublishNodePlayedEvent(ctxRunner, context);
    }
}
