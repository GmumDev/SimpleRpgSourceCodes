using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Quest/ItemRewardSO")]

public class QuestItemRewardSO: QuestRewardSO
{
	public string itemId;
	public int itemCount;

    public override QuestRewardContext ToContext()
    {
        var context = new QuestRewardContext();
        context.type = type;
        context.rewardId = itemId;
        context.amount = itemCount;

        return context;
    }
}