using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUISystem : MonoBehaviour
{
	static PlayerLevelUISystem instance;
	public static PlayerLevelUISystem Instance { get => instance; }

	[SerializeField]
	TextMeshProUGUI levelText;
	[SerializeField]
	Slider expSlider;
	List<SubscriptionToken> tokens;
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
		tokens.Add(EventBus.Subscribe<PlayerLevelUpEvent>(OnLevelUp));
		tokens.Add(EventBus.Subscribe<PlayerExpGainedEvent>(OnExpGained));
	}
    private void Start()
    {
        SetLevelText(Player.PlayerData.playerStats.level);
        SetExpSliderValue(Player.PlayerData.playerStats.exp);
    }
    void SetLevelText(int level) => levelText.text = "Lv " + (level + 1);
	void SetExpSliderValue(int exp) => expSlider.value = Mathf.Clamp(exp * 1.0f / Player.PlayerData.MaxExp, 0, 1);
	void OnLevelUp(PlayerLevelUpEvent ev)
	{
		SetLevelText(ev.level);
		SetExpSliderValue(Player.PlayerData.playerStats.exp);
	}
	void OnExpGained(PlayerExpGainedEvent ev)
	{
		SetExpSliderValue(Player.PlayerData.playerStats.exp);
	}
	
	private void OnDestroy()
	{
		foreach(var token in tokens)
			EventBus.Unsubscribe(token);
	}
}
