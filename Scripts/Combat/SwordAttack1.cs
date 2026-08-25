using UnityEngine;

public class SwordAttack1: AttackSkill
{
	[SerializeField]
	float AttackCooldown = .4f;
	[SerializeField]
    protected SkillAttackCollider[] attackColliders;
	public override float MaxCooldown { get => AttackCooldown; }

	public override void OnAssigned()
	{
		curStep = 0;
	}
	public override void Attack()
	{
		base.Attack();
        attackColliders[curStep].Trigger(targetLayerMask);
        curStep = (curStep + 1) % attackColliders.Length;
    }

	public override bool CanAttack()
	{
		return Time.time - lastAttackT > AttackCooldown;
	}
}
