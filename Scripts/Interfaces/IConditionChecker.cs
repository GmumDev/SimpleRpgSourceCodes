
public interface IConditionChecker
{
    bool Check(IQuestManager manager, QuestConditionContext conditionCtx, QuestConditionProgress conditionProgress);
}
