using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
	public string id;
	public string Name;
	public int hp;
	public int dropExp;
	public DropTableSO dropTable;
}
