using UnityEngine;

public class DungeonLoopManager : MonoBehaviour
{
	[SerializeField]
    GameObject DungeonClearNPC;

    [SerializeField]
    EnemySpawner bossSpawner;
	[SerializeField]
	ParticlePoolManager bossSpawnerParticleManager;
	[SerializeField]
	EnemySpawner enemySpawner;
	[SerializeField]
	ParticlePoolManager enemySpawnerParticleManager;

	public static DungeonDataSO dungeonData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
		bossSpawner.SetEnemyPrefab(dungeonData.bossPrefab);
		bossSpawnerParticleManager.SetParticle(dungeonData.bossDieParticlePrefab);
		bossSpawner.gameObject.SetActive(true);

		enemySpawner.SetEnemyPrefab(dungeonData.enemyPrefab);
		enemySpawnerParticleManager.SetParticle(dungeonData.enemyDieParticlePrefab);
		enemySpawner.gameObject.SetActive(true);

		DungeonClearNPC.SetActive(false);
		bossSpawner.OnEnemyKilled += SpawnNPC;
	}

    void SpawnNPC()
	{
		DungeonClearNPC.SetActive(true);
		bossSpawner.OnEnemyKilled -= SpawnNPC;
	}

	private void OnDestroy()
	{
		if(bossSpawner != null)
			bossSpawner.OnEnemyKilled -= SpawnNPC;
	}
}