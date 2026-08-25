using UnityEngine;

public class SwordAttack2: AttackSkill
{

	[SerializeField]
	float AttackCooldown = 5f;
	[SerializeField]
	int damage = 5;
	[SerializeField]
	protected SkillAttackCollider attackCollider;
	[SerializeField]
	ParticleSystem swordParticle;

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
		swordParticle.Play();
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
