
public interface IAttackSkill
{
    public float MaxCooldown { get; }
	public void OnAssigned();
    public bool CanAttack();
    public void Attack();
}
