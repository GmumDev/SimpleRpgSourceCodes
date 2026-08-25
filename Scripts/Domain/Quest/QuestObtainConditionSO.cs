using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Quest/ObtainConditionSO")]
public class QuestObtainConditionSO: QuestConditionSO
{
    public string itemID;
    public int itemCntToObtain;
    public bool removeItemsOnComplete;

    public override QuestConditionContext ToContext()
    {
        type = QuestConditionType.Obtain;
        var context = new QuestConditionContext();
        context.type = type;

        context.itemID = itemID;
        context.itemCntToObtain = itemCntToObtain;
        context.removeItemsOnComplete = removeItemsOnComplete;

        return context;
    }
}
