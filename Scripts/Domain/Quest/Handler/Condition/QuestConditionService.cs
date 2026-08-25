using System.Collections.Generic;

public static class QuestConditionService
{
    private static readonly Dictionary<QuestConditionType, IConditionChecker> handlers
        = new Dictionary<QuestConditionType, IConditionChecker>()
        {
            { QuestConditionType.Obtain, new ObtainConditionChecker() },
			{ QuestConditionType.Kill, new KillConditionChecker() }
		};
    public static bool IsMet(IQuestManager manager, QuestConditionContext conditionCtx, QuestConditionProgress progressCtx)
    {
        return handlers[conditionCtx.type].Check(manager, conditionCtx, progressCtx);

    }
}
