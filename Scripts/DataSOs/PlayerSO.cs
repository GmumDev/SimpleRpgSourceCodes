using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public string id;
    public string nickname;
    public PlayerStats playerStats;
    public PlayerConfig playerConfig;
    public List<PlayerLeveledStat> playerLeveledStats;
    public int MaxHp { get => playerLeveledStats[playerStats.level].maxHp; }
    public int MaxExp { get => playerLeveledStats[playerStats.level].maxExp; }
    public float AttackMultiplier { get => playerLeveledStats[playerStats.level].attackMultiplier; }
    public float DefenceMultiplier { get => playerLeveledStats[playerStats.level].defenceMultiplier; }
    public void InitOnRevive()
    {
        playerStats.hp = MaxHp;
    }
}

[Serializable]
public class PlayerStats
{
    public int level;
    public int exp;
    public int hp;
}

[Serializable]
public class PlayerLeveledStat
{
    public int maxExp;
    public int maxHp;
    public float attackMultiplier;
    public float defenceMultiplier;
}
[Serializable]
public class PlayerConfig
{
	public float mouseSenceX;
	public float mouseSenceY;
	public const float MaxMouseSenceX = 120f;
	public const float MaxMouseSenceY = 120f;
}