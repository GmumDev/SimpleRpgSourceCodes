using System.Threading.Tasks;
using UnityEngine;

public class PlayScenarioOnTriggerEnter : MonoBehaviour
{
    [SerializeField] string scenarioId;
	[SerializeField] bool destroyThisOnTriggered;
	[SerializeField] string memorizeActivationAssetKey;
	ActivationDataSO memorizeActivation;
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
	private void OnApplicationQuit()
	{
		if(!isSaved)
			SaveDataOverwriter.Instance.SaveToFile(memorizeActivation);
	}

	private void OnTriggerEnter(Collider other)
	{
		ScenarioManager.Instance.PlayScenario(scenarioId);
		if (destroyThisOnTriggered)
		{
			memorizeActivation.activation = false;
			SaveDataOverwriter.Instance.SaveToFile(memorizeActivation);
			isSaved = true;
			gameObject.SetActive(false);
		}
	}
}
