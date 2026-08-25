using System;

[Serializable]
public class QuestConditionContext
{
    public QuestConditionType type;

    // obtain
    public string itemID;
    public int itemCntToObtain;
    public bool removeItemsOnComplete;

	// kill
	public string enemyId;
	public int enemyCntToKill;
}
