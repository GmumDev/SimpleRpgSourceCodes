
public class ObtainConditionChecker : IConditionChecker
{
    bool IConditionChecker.Check(IQuestManager manager, QuestConditionContext conditionCtx, QuestConditionProgress progressCtx)
    {
        return progressCtx.curAmount >= progressCtx.goalAmount;
    }
}
