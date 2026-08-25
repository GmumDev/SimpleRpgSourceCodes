
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour, IDamageTakeable
{
	[SerializeField]
	EnemySO enemySO;

	[SerializeField]
	PatternList[] patterns;
	[Serializable]
	class PatternList
	{
		[SerializeField]
		List<EnemyPattern> list;
		public void DoPattern()
		{
			foreach(var elem in list)
			{
				elem.DoPattern();
			}
		}
		public void CancelPattern()
		{
			foreach(var elem in list)
			{
				elem.CancelPattern();
			}
		}
	}

	protected EnemyKilledEvent ev;

	Action<Enemy> ReleaseActionOfSpawner;


	Vector3 spawnerCenter;
	public Vector3 SpawnerCenter { get => spawnerCenter; }


	float rangeFromCenter;
	public float RangeFromCenter { get => rangeFromCenter; }
	public int DropExpAmount { get => enemySO.dropExp; }
	Animator anim;

	int hp;
	int instanceID;

	public NavMeshAgent agent { get; private set; }
	private void Awake()
	{
		ev = new EnemyKilledEvent(enemySO.id, enemyKilledCnt: 1, dropExp: enemySO.dropExp);
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		hp = enemySO.hp;
		instanceID = GetInstanceID();
	}

	// object pool in enemyspawner
	public void Init(Action<Enemy> Release, Vector3 center, float range)
	{
		this.ReleaseActionOfSpawner = Release;
		spawnerCenter = center;
		rangeFromCenter = range;
		hp = enemySO.hp;
	}
	public void TakeDamage(int damage)
	{
		if (hp <= 0) return;
		hp -= damage;
		EventBus.Publish(new EnemyHittedEvent(enemySO, transform.position, damage));
		if (hp <= 0)
		{
			EnemyHPUISystem.Instance.OnEnemyDied(instanceID);
			Die();
		}
		else
		{
			EnemyHPUISystem.Instance.OnEnemyHit(instanceID, hp, enemySO.hp, enemySO.Name);
			anim.SetTrigger("OnHit");
		}
	}
	public void DoPattern(int idx)
	{
		if(patterns.Length > idx - 1 && patterns[idx] != null)
			patterns[idx].DoPattern();
	}
	public void CancelPattern(int idx)
	{
		if (patterns.Length > idx - 1 && patterns[idx] != null)
			patterns[idx].CancelPattern();
	}
	protected void Die()
	{
		DropItemManager.Instance.Generate(transform.position, enemySO.dropTable);
		anim.SetTrigger("OnDie");
		EventBus.Publish<EnemyKilledEvent>(ev);
	}
	public void DestroyThisEnemy()
	{
		ReleaseActionOfSpawner(this);
	}
	public Vector3 GetNewDestInSpawner()
	{
		var dest = SpawnerCenter
			+ new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized
			* UnityEngine.Random.Range(0, rangeFromCenter);
		dest.y = transform.position.y;
		return dest;
	}
}
