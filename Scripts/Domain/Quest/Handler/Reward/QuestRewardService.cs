using System.Collections.Generic;

public class QuestRewardService
{
    private readonly Dictionary<QuestRewardType, IRewardHandler> handlers
        = new Dictionary<QuestRewardType, IRewardHandler>()
        {
            { QuestRewardType.Item, new ItemRewardHandler() }
        };

    public void Give(IQuestRewardEarner earner, QuestRewardContext context)
    {
        handlers[context.type].Handle(earner, context);
    }

}
