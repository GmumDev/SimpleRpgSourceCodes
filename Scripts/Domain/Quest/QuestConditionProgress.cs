using System;
using System.Text;

[Serializable]
public class QuestConditionProgress
{
	public QuestConditionType type;
	public bool isComplete;
	
	public string targetName;
	public string UIText
	{
		get
		{
			string s = "";
			switch (type)
			{
				case QuestConditionType.Obtain: s = " 얻기"; break;
                case QuestConditionType.Kill: s = " 처지하기"; break;
            }
			StringBuilder sb = new StringBuilder();
			sb.Append(targetName).Append(s).Append("(").Append(curAmount).Append(" / ").Append(goalAmount).Append(")");
			return sb.ToString();
		}
	}
	public int goalAmount;
	public int curAmount;

	// obtain
	public string itemID;

	// kill
	public string enemyId;

	public static QuestConditionProgress GetObtainConditionProgress(string itemId, int goalAmount)
	{
		QuestConditionProgress progress = new QuestConditionProgress();
		progress.type = QuestConditionType.Obtain;
		progress.itemID = itemId;
        progress.goalAmount = goalAmount;
		progress.targetName = ItemDataContainer.GetNameWithId(itemId);

		return progress;
	}
	public static QuestConditionProgress GetKillConditionProgress(string enemyId, int goalAmount)
	{
		QuestConditionProgress progress = new QuestConditionProgress();
		progress.type = QuestConditionType.Kill;
		progress.enemyId = enemyId;
		progress.goalAmount = goalAmount;
		progress.targetName = EnemyDataContainer.GetEnemySOWithId(enemyId).Name;

        return progress;
	}
}
