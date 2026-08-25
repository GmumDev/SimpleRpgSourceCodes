using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUISystem : MonoBehaviour
{
	static PlayerHpUISystem instance;
	public static PlayerHpUISystem Instance { get => instance; }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		tokens = new List<SubscriptionToken>();
		tokens.Add(EventBus.Subscribe<PlayerHittedEvent>(OnPlayerHitted));
		tokens.Add(EventBus.Subscribe<PlayerRevivedEvent>(OnPlayerRevived));
		tokens.Add(EventBus.Subscribe<PlayerLevelUpEvent>(OnLevelUp));
		FillHpBar();
	}
	[SerializeField]
    Slider hpbar;
	[SerializeField]
	TextMeshProUGUI hpText;
	List<SubscriptionToken> tokens;
	StringBuilder sb = new StringBuilder();
	void UpdatePlayerHpBar()
	{
		hpbar.value = (Player.PlayerData.playerStats.hp * 1.0f) / Player.PlayerData.MaxHp;
		sb.Clear();
		hpText.text = sb.Append(Player.PlayerData.playerStats.hp).Append('/').Append(Player.PlayerData.MaxHp).ToString();
	}
	void FillHpBar()
	{
		hpbar.value = 1f;
		hpText.text = "";
	}
	void OnPlayerHitted(PlayerHittedEvent ev)
	{
		UpdatePlayerHpBar();
	}
	void OnPlayerRevived(PlayerRevivedEvent ev)
	{
		FillHpBar();
	}
	void OnLevelUp(PlayerLevelUpEvent ev)
	{
		UpdatePlayerHpBar();
	}
	private void OnDestroy()
	{
		foreach(var token in tokens)
			EventBus.Unsubscribe(token);
	}
}
