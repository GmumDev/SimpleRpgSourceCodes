using System.Threading.Tasks;
using UnityEngine;

public abstract class NPC : MonoBehaviour, IInteractable
{
	[SerializeField] string npcSOKey;
    protected NPCSO npcSo;
	protected TaskCompletionSource<bool> awakeCompleted = new TaskCompletionSource<bool>();

	protected async virtual void Awake()
	{
		while (AddressableGroupLoader.Instance.IsLoaded == false)
		{
			await Task.Yield();
		}
		npcSo = AddressableGroupLoader.Instance.GetSaveData<NPCSO>(npcSOKey);
		
		npcSo.Deserialize();

		awakeCompleted.SetResult(true);
	}
	public abstract void OnInteract();
    public string GetInteractableID() => npcSo.id;
	public string GetInteractableType() => "NPC";
	private async void OnDestroy()
	{
		var timeout = Task.Delay(5000);
		var completed = await Task.WhenAny(awakeCompleted.Task, timeout);
		if (npcSo == null) return;
		npcSo.Serialize();
		SaveDataOverwriter.Instance.SaveToFile(npcSo);
	}

	protected virtual void OnApplicationQuit()
    {
        npcSo.Serialize();
		SaveDataOverwriter.Instance.SaveToFile(npcSo);
	}
}
