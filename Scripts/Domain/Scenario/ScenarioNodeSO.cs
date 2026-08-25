using UnityEngine;


public abstract class ScenarioNodeSO: ScriptableObject
{
	public bool hasPlayedEvent;
	public ScenarioNodePlayedEventType playedEventType;

	// generate event instance on ToContext
	public ScenarioNodeFinishedEventType finishedEventType;
    public string finishedFollowUpQuestId;
    public string finishedCompleteQuestId;
    public string finishedFollowUpScenarioId;
	public string loadSceneName;
	public string timelineId;
	// ~ 

	protected ScenarioNodeFinishedEvent GetFinishedEvent()
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
	protected ScenarioNodePlayedEvent GetPlayedEvent()
	{
		var obj = new ScenarioNodePlayedEvent(
			eventType: playedEventType
		);
		return obj;
	}
	public abstract ScenarioNodeContext ToContext();

}
