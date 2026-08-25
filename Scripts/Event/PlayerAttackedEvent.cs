
public class PlayerAttackedEvent
{
    public int AttackIdx;
	public float MaxCooldown;
	public PlayerAttackedEvent(int attackIdx, float maxCooldown)
	{
		AttackIdx = attackIdx;
		MaxCooldown = maxCooldown;
	}
}
