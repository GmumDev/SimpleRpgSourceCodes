
public class NPCScenarioChangedEvent
{
    public string npcId;
    public string scenarioIdFrom;
	public string scenarioIdTo;

	public NPCScenarioChangedEvent(string npcId, string scenarioIdFrom, string scenarioIdTo)
	{
		this.npcId = npcId;
		this.scenarioIdFrom = scenarioIdFrom;
		this.scenarioIdTo = scenarioIdTo;
	}
}
