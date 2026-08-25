using UnityEngine;

public class PlayerGrowSystem
{
	PlayerExpGainedEvent playerExpGainedEvent;
	PlayerLevelUpEvent playerLevelUpEvent;
	public PlayerGrowSystem()
	{
		playerExpGainedEvent = new PlayerExpGainedEvent(0);
		playerLevelUpEvent = new PlayerLevelUpEvent(0);
	}
	public void GainExp(PlayerSO playerSO, int amount)
	{
		playerSO.playerStats.exp += amount;
		// already max level
		bool levelup = false;
		while (playerSO.MaxExp <= playerSO.playerStats.exp)
		{
			if (playerSO.playerStats.level == playerSO.playerLeveledStats.Count - 1)
			{
				playerSO.playerStats.exp = Mathf.Clamp(playerSO.playerStats.exp, 0, playerSO.MaxExp);
				return;
			}

			playerSO.playerStats.exp -= playerSO.MaxExp;
			playerSO.playerStats.level++;
			levelup = true;
			playerSO.InitOnRevive();
		}
		playerExpGainedEvent.newExp = playerSO.playerStats.exp;
		EventBus.Publish(playerExpGainedEvent);
		if (levelup)
		{
			playerLevelUpEvent.level = playerSO.playerStats.level;
			EventBus.Publish(playerLevelUpEvent);
		}
	}
}
