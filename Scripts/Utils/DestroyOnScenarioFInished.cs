using System.Threading.Tasks;
using UnityEngine;

public class DestroyOnScenarioFInished : MonoBehaviour
{
    [SerializeField] string scenarioId;
	[SerializeField] string memorizeActivationAssetKey;
	ActivationDataSO memorizeActivation;
	SubscriptionToken token;

	bool isSaved;
	private async void Awake()
	{
		while (AddressableGroupLoader.Instance.IsLoaded == false)
		{
			await Task.Yield();
		}
		memorizeActivation = AddressableGroupLoader.Instance.GetSaveData<ActivationDataSO>(memorizeActivationAssetKey);
		if (memorizeActivation.activation == false)
		{
			gameObject.SetActive(false);
		}
	}
	private void OnEnable()
	{
		token = EventBus.Subscribe<ScenarioFinishedEvent>(OnScenarioFinished);
	}
	void OnScenarioFinished(ScenarioFinishedEvent ev)
	{
		if (ev.id == scenarioId)
		{
			memorizeActivation.activation = false;
			SaveDataOverwriter.Instance.SaveToFile(memorizeActivation);
			isSaved = true;
			gameObject.SetActive(false);
		}
	}
	private void OnDisable()
	{
		EventBus.Unsubscribe(token);
    }
    private void OnApplicationQuit()
	{
		if (!isSaved)
			SaveDataOverwriter.Instance.SaveToFile(memorizeActivation);
    }
}
