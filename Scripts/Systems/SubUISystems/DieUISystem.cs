using System.Collections.Generic;
using UnityEngine;

public class DieUISystem : MonoBehaviour
{
	static DieUISystem instance;
	public static DieUISystem Instance { get => instance; }

	[SerializeField]
	FadeInoutPanel panel;

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
		tokens.Add(EventBus.Subscribe<PlayerRevivedEvent>(OnPlayerRevived));
		tokens.Add(EventBus.Subscribe<PlayerDeadEvent>(OnPlayerDead));
	}
	void OnPlayerDead(PlayerDeadEvent ev)
	{
		_ = panel.Fadein();
	}
	void OnPlayerRevived(PlayerRevivedEvent ev)
	{
		_ = panel.Fadeout();
	}
	private void OnDestroy()
	{
		foreach (var token in tokens)
			EventBus.Unsubscribe(token);
	}
}
