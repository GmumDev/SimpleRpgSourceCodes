using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLeveledStatTable", menuName = "Scriptable Objects/PlayerLeveledStatTable")]
public class PlayerLeveledStatTable : ScriptableObject
{
    public int level;
    public int maxExp;
    public int maxHp;
    public float attackMultiplier;
    public float defenceMultiplier;
}