using System.Collections.Generic;
using UnityEngine;

public class EnemyDataContainer : MonoBehaviour
{
    [SerializeField]
    EnemyContainerSO container;
    Dictionary<string, EnemySO> enemyDict;

    static EnemyDataContainer instance;
    public static EnemyDataContainer Instance { get => instance; }
    public static EnemySO GetEnemySOWithId(string id)
    {
        if(instance.enemyDict.ContainsKey(id))
        {
            return instance.enemyDict[id];
        }

        throw new System.Exception("그런 id를 가진 enemy는 없다");
    }
	private void Awake()
	{
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        enemyDict = new Dictionary<string, EnemySO>();
		foreach (var enemy in container.enemySOs)
        {
            enemyDict.Add(enemy.id, enemy);
		}
	}
}
