
public class QuestCompletionChangedEvent
{
    public string questId;
	public string followUpScenarioId;
	public bool isCompleted;
	public QuestCompletionChangedEvent(string questId, string followUpScenarioId, bool isCompleted)
	{
		this.questId = questId;
		this.followUpScenarioId = followUpScenarioId;
		this.isCompleted = isCompleted;
	}
}
