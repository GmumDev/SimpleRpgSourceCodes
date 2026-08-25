using UnityEngine;

public class EnemyHittedEvent
{
	public EnemySO enemySO;
	public Vector3 hitpos;
	public int damage;
	public EnemyHittedEvent(EnemySO enemySO, Vector3 hitpos, int damage)
	{
		this.enemySO = enemySO;
		this.hitpos = hitpos;
		this.damage = damage;
	}
}
