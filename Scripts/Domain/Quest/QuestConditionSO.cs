using UnityEngine;

public abstract class QuestConditionSO : ScriptableObject
{
	protected QuestConditionType type;

	public abstract QuestConditionContext ToContext();
}
