using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSkillUISystem : MonoBehaviour
{
	[SerializeField]
	Image[] FillBtns;
	List<SubscriptionToken> tokens;

	float[] maxColldowns;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
    {
		tokens = new List<SubscriptionToken>();
		tokens.Add(EventBus.Subscribe<PlayerAttackedEvent>(OnAttacked));
		tokens.Add(EventBus.Subscribe<PlayerCharacterChangedEvent>(OnChanged));
		maxColldowns = new float[FillBtns.Length];
	}

    // Update is called once per frame
    void Update()
    {
		for(int i = 0; i < FillBtns.Length; i++)
		{
			var fill = FillBtns[i];
			if (fill.fillAmount < 1)
				fill.fillAmount += Time.deltaTime / maxColldowns[i];
		}
    }
	void OnChanged(PlayerCharacterChangedEvent ev)
	{
		var data = ev.data;
		for(int i = 0; i< FillBtns.Length;i++)
		{
			FillBtns[i].sprite = data.SkillIcons[i];
			FillBtns[i].fillAmount = 1f;
		}
	}
	public void OnAttacked(PlayerAttackedEvent ev)
	{
		FillBtns[ev.AttackIdx].fillAmount = 0f;
		maxColldowns[ev.AttackIdx] = ev.MaxCooldown;
	}

	private void OnDestroy()
	{
		foreach(var token in tokens)
			EventBus.Unsubscribe(token);
	}
}
