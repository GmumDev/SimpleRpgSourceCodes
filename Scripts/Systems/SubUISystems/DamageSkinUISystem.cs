using System.Collections.Generic;
using UnityEngine;

public class DamageSkinUISystem : MonoBehaviour
{
	[SerializeField]
    DamageSkinPoolManager poolManager;

    List<SubscriptionToken> tokens;

    void Awake()
    {
        tokens = new List<SubscriptionToken>();

		EventBus.Subscribe<EnemyHittedEvent>(OnEnemyHitted);
    }

    void OnEnemyHitted(EnemyHittedEvent ev)
    {
        poolManager.Generate(ev.hitpos, ev.damage);
    }
	private void OnDestroy()
	{
        foreach (var token in tokens)
			EventBus.Unsubscribe(token);
	}
}
