
public interface IScenarioNodeHandler
{
    void PlayAsFirstNode(IScenarioContextRunner scenarioPlayer, ScenarioNodeContext context);
    void PlayAsNextNode(IScenarioContextRunner scenarioPlayer, ScenarioNodeContext context);
    void FinishNode(IScenarioContextRunner scenarioPlayer, ScenarioNodeContext context);
}
