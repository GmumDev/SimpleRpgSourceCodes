
public class KillConditionChecker : IConditionChecker
{
	bool IConditionChecker.Check(IQuestManager manager, QuestConditionContext conditionCtx, QuestConditionProgress progressCtx)
	{
		return progressCtx.curAmount >= progressCtx.goalAmount;	
	}
}
