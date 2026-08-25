using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Quest/KillConditionSO")]

public class QuestKillConditionSO: QuestConditionSO
{
    public string enemyId;
    public int enemyCntToKill;

	public override QuestConditionContext ToContext()
	{
		type = QuestConditionType.Kill;
		var context = new QuestConditionContext();
		context.type = type;
		context.enemyId = enemyId;
		context.enemyCntToKill = enemyCntToKill;

		return context;
	}
}
