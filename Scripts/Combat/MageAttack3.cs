using UnityEngine;

public class MageAttack3 : AttackSkill
{
	[SerializeField]
	float AttackCooldown = 2f;
	[SerializeField]
	int damage = 3;
	[SerializeField]
	protected SkillAttackCollider attackCollider;

	[SerializeField]
	ParticlePoolManager particlePoolManager;
	public override float MaxCooldown { get => AttackCooldown; }
	public override void OnAssigned()
	{
		attackCollider.Damage = damage;
	}

	public override void Attack()
	{
		base.Attack();
		attackCollider.Trigger(targetLayerMask, OnHitted);
    }
	void OnHitted(Vector3 hitpos)
	{
		particlePoolManager.Generate(hitpos);
	}
	public override bool CanAttack()
	{
		return Time.time - lastAttackT > AttackCooldown;
	}
}
