using UnityEngine;

public abstract class AttackSkill : MonoBehaviour, IAttackSkill
{
	[SerializeField]
	protected LayerMask targetLayerMask;

	protected int curStep;
	public int CurStep 
	{ 
		get {

			if (Time.time - lastAttackT > attackComboDuration)
			{
				curStep = 0;
			}
			return curStep;
		} 
	}

	public abstract float MaxCooldown { get; }

	protected float lastAttackT;

	float attackComboDuration = 2f;

	public virtual void Attack()
	{
		lastAttackT = Time.time;
	}

    public abstract bool CanAttack();

	public virtual void OnAssigned()
	{
		curStep = 0;
	}
}
