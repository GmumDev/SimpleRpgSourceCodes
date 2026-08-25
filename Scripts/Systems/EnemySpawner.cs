using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject enemyPrefab;

    [Header("SpawnInfo")]
    [SerializeField]
    int maxEnemyCnt;
    [SerializeField]
    float spawnRadius;
	[SerializeField]
	float maxDistanceFromCenter;
    [SerializeField]
    float respawnCooldown;
	[SerializeField]
	bool enableOnStart;
	[SerializeField]
	bool spawnOnce;
	public System.Action OnEnemyKilled;

	IObjectPool<Enemy> enemyPool;
	int curEnemyCnt;
	[SerializeField]
	ParticlePoolManager dieParticleGenerator;
	public void SetEnemyPrefab(GameObject enemy)
	{
		this.enemyPrefab = enemy;
	}
	private void OnDrawGizmos()
	{
#if UNITY_EDITOR
		Handles.color = Color.red;
		Handles.DrawWireDisc(transform.position, Vector3.up, spawnRadius);
		Handles.color = Color.yellow;
		Handles.DrawWireDisc(transform.position, Vector3.up, maxDistanceFromCenter);
#endif
	}
    private void Awake()
    {
        enemyPool = new ObjectPool<Enemy>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,   // helps catch double-release mistakes
            defaultCapacity: maxEnemyCnt,
            maxSize: maxEnemyCnt * 2
        );
    }
    void Start()
	{
		if (enableOnStart)
		{
			if(spawnOnce)
			{
				Spawn();
			}
			else
				StartCoroutine(SpawnCoroutine());
		}
	}

	#region ObjectPool
	private Enemy CreateItem()
	{
		GameObject gameObject = Instantiate(enemyPrefab, transform);

		gameObject.SetActive(false);
		return gameObject.GetComponent<Enemy>();
	}

	// Called when an item is taken from the pool.
	private void OnGet(Enemy enemy)
	{
		enemy.gameObject.SetActive(true);
	}

	// Called when an item is returned to the pool.
	private void OnRelease(Enemy enemy)
	{
		enemy.gameObject.SetActive(false);
	}

	// Called when the pool decides to destroy an item (e.g., above max size).
	private void OnDestroyItem(Enemy enemy)
	{
		Destroy(enemy.gameObject);
	}
	#endregion
	void Spawn()
    {
		while(curEnemyCnt < maxEnemyCnt)
		{
			var enemy = enemyPool.Get();
			if (AsyncSceneManager.IsDontDestroyOnLoad(gameObject))
				DontDestroyOnLoad(enemy.gameObject);
			enemy.Init(Release, transform.position, maxDistanceFromCenter);
			enemy.transform.position = Vector3.zero;
			enemy.transform.localPosition = new Vector3(1, 0, 1) * Random.Range(-spawnRadius, spawnRadius);

			curEnemyCnt++;
		}
	}
	public void Release(Enemy enemy)
	{
		dieParticleGenerator.Generate(enemy.gameObject.transform.position);
		ExpObjPoolManager.Instance.Generate(enemy.gameObject.transform.position, enemy.DropExpAmount);
		enemyPool.Release(enemy);
		curEnemyCnt--;
		OnEnemyKilled?.Invoke();
	}
    IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(respawnCooldown);
        }
    }
}
