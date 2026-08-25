using System.Threading;
using UnityEngine;

public class Gatherable : MonoBehaviour, IInteractable
{
	[SerializeField]
	ItemSO itemSO;
	[SerializeField]
	int amount = 1;
	[SerializeField]
	float interactDuration = 5f;
	[SerializeField]
	bool destroyOnGathered;
	public string GetInteractableID() => itemSO.id;
	public string GetInteractableType() => "Gatherable";

	CancellationTokenSource cts;
	SubscriptionToken subsciptiontoken;
	public void OnInteract()
	{
		if (cts != null) return;
		cts = new CancellationTokenSource();
		EventBus.Unsubscribe(subsciptiontoken);
		subsciptiontoken = EventBus.Subscribe<GatheringCancelledEvent>(Cancel);
		_ = Gather(cts.Token);
	}
	async Awaitable Gather(CancellationToken token)
	{
		await Awaitable.WaitForSecondsAsync(interactDuration, token);
		if (token.IsCancellationRequested) return;
		cts?.Dispose();
		cts = null;

		EventBus.Publish(new GatheringFinishedEvent());
		InventorySystem.Instance.ObtainItem(itemSO.id, amount);
		if(destroyOnGathered) Destroy(gameObject);
	}
	void Cancel(GatheringCancelledEvent ev)
	{
		CancelSafe();
	}
	void CancelSafe()
	{
		if (cts != null)
		{
			cts.Cancel();
			cts.Dispose();
			cts = null;
		}
		EventBus.Unsubscribe(subsciptiontoken);
	}
	private void OnDestroy()
	{
		CancelSafe();
	}
}
