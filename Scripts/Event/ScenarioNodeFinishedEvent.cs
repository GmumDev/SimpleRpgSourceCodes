
public class ScenarioNodeFinishedEvent
{
    public ScenarioNodeFinishedEventType eventType;

    public string followUpQuestId;
	public string CompleteQuestId;
	public string followUpScenarioId;
	public string loadSceneName;
	public string timelineId;

	public ScenarioNodeFinishedEvent(
        ScenarioNodeFinishedEventType eventType,
		string followUpQuestId,
		string CompleteQuestId,
		string followUpScenarioId,
		string loadSceneName,
		string timelineId)
    {
        this.eventType = eventType;
		this.CompleteQuestId = CompleteQuestId;
		this.followUpQuestId = followUpQuestId;
        this.followUpScenarioId = followUpScenarioId;
		this.loadSceneName = loadSceneName;
		this.timelineId = timelineId;
	}
}
