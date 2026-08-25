using System.Collections.Generic;
using static QuestManager;

public interface IQuestManager
{
    Dictionary<string, QuestState> QuestStates { get; }
    void CompleteQuest(string questID);
	void AcceptQuest(string questID);
}
