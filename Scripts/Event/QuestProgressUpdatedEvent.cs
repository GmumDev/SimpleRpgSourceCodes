using static QuestManager;

public class QuestProgressUpdatedEvent
{
	public QuestState state;
	public QuestProgressUpdatedEvent(QuestState state)
	{
		this.state = state;
	}
}
