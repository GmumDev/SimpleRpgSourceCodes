using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyContainerSO", menuName = "Scriptable Objects/EnemyContainerSO")]
public class EnemyContainerSO : ScriptableObject
{
    public List<EnemySO> enemySOs;
}