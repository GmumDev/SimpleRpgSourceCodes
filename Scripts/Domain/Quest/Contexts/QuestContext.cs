using System;

[Serializable]
public class QuestContext
{
    public string id;
    public string title;
    public string descript;
    public string followUpScenarioId;

	public QuestConditionContext[] conditionContexts;
    public QuestRewardContext[] rewardContexts;

    public int expRewardAmount;
}
