using UnityEngine;

[CreateAssetMenu(fileName = "DungeonDataSO", menuName = "Scriptable Objects/DungeonDataSO")]
public class DungeonDataSO : ScriptableObject
{
	public GameObject enemyPrefab;
	public GameObject enemyDieParticlePrefab;
	public GameObject bossPrefab;
	public GameObject bossDieParticlePrefab;
}
