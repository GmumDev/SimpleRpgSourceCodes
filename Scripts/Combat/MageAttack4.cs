using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MageAttack4: AttackSkill
{
	[SerializeField]
	float AttackCooldown = .4f;
	[SerializeField]
	float speed = 4f;
	[SerializeField]
	int damage = 1;
	[SerializeField]
    protected GameObject projectile;
	[SerializeField]
	int many = 12;
	[SerializeField]
	float roundDelta = 3f;
	[SerializeField]
	float roundOffsetY = 5f;

	IObjectPool<ProjectileByPool> pool;
	BoxCollider col;
	Collider[] hitColliders = new Collider[30];
	public override float MaxCooldown { get => AttackCooldown; }
	private void Awake()
	{
		col = GetComponent<BoxCollider>();
		col.enabled = false;
	}
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
		var targets = TriggerAndGetTarget();
		if (targets.Count < 1) return;
		for (int i = 0; i < many; i++)
		{
			var obj = pool.Get();
			obj.Init(
				ReleaseAction: pool.Release,
				pos: transform.position + transform.up * roundOffsetY + Quaternion.AngleAxis(i * 360f / many, transform.forward) * transform.right * roundDelta,
				dir: transform.forward,
				speed: speed,
				damage: damage,
				targetLayerMask: targetLayerMask
			);
			obj.AddAddon(new ProjectileChaseAddon(obj, targets[i % targets.Count]));
		}
    }

	public List<Transform> TriggerAndGetTarget()
	{
		Vector3 halfExtents = Vector3.Scale(col.size, transform.lossyScale) * 0.5f;
		Vector3 worldCenter = transform.TransformPoint(col.center);
		int n = Physics.OverlapBoxNonAlloc(worldCenter, halfExtents, hitColliders, transform.rotation, targetLayerMask);
		List<Transform> targets = new List<Transform>();
		for(int i = 0; i < n; i++)
		{
			if (hitColliders[i].TryGetComponent<IDamageTakeable>(out var dt))
			{
				targets.Add(hitColliders[i].transform);
			}
		}
		return targets;
	}
	public override bool CanAttack()
	{
		return TriggerAndGetTarget().Count > 0 && Time.time - lastAttackT > AttackCooldown;
	}

	#region ObjectPool
	private ProjectileByPool CreateItem()
	{
		GameObject gameObject = Instantiate(projectile);
		if (AsyncSceneManager.IsDontDestroyOnLoad(this.gameObject))
			DontDestroyOnLoad(gameObject);
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
