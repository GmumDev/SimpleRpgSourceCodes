using UnityEngine;
using UnityEngine.Pool;

public class MageAttack2: AttackSkill
{
	[SerializeField]
	float AttackCooldown = .4f;
	[SerializeField]
	float speed = 4f;
	[SerializeField]
	int damage = 1;
	[SerializeField]
    protected GameObject projectile;

	IObjectPool<ProjectileByPool> pool;

	public override float MaxCooldown { get => AttackCooldown; }

	public override void OnAssigned()
	{
		if(pool == null)
		{
			pool = new ObjectPool<ProjectileByPool>(
				createFunc: CreateItem,
				actionOnGet: OnGet,
				actionOnRelease: OnRelease,
				actionOnDestroy: OnDestroyItem,
				collectionCheck: true,   // helps catch double-release mistakes
				defaultCapacity: 10,
				maxSize: 20
			);
		}
	}

	public override void Attack()
	{
		base.Attack();
		var obj = pool.Get();
		if (AsyncSceneManager.IsDontDestroyOnLoad(gameObject))
			DontDestroyOnLoad(obj.gameObject);
		obj.Init(
			ReleaseAction: pool.Release, 
			pos: transform.position, 
			dir: transform.forward, 
			speed: speed, 
			damage: damage,
			targetLayerMask: targetLayerMask
		);
    }

	public override bool CanAttack()
	{
		return Time.time - lastAttackT > AttackCooldown;
	}

	#region ObjectPool
	private ProjectileByPool CreateItem()
	{
		GameObject gameObject = Instantiate(projectile);
		var proj = gameObject.GetComponent<ProjectileByPool>();
		gameObject.SetActive(false);
		return proj;
	}

	// Called when an item is taken from the pool.
	private void OnGet(ProjectileByPool obj)
	{
		obj.gameObject.SetActive(true);
	}

	// Called when an item is returned to the pool.
	private void OnRelease(ProjectileByPool obj)
	{
		obj.gameObject.SetActive(false);
	}

	// Called when the pool decides to destroy an item (e.g., above max size).
	private void OnDestroyItem(ProjectileByPool obj)
	{
		Destroy(obj.gameObject);
	}
	#endregion
}
