using System.Threading.Tasks;
using UnityEngine;

public class GameManager: MonoBehaviour
{
	[SerializeField] string gameDataSOKey;
	public GameDataSO gameData;
	public int LoadingProgress { get; private set; }
	static GameManager instance;
	public static GameManager Instance
	{
		get
		{
			if(instance == null)
			{
				return null;
			}
			return instance;
		}
	}
	private async void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
			return;
		}

		while (AddressableGroupLoader.Instance.IsLoaded == false)
		{
			await Task.Yield();
		}
		gameData = AddressableGroupLoader.Instance.GetSaveData<GameDataSO>(gameDataSOKey);
	}
	private async Awaitable Start()
	{
        SOLoader<ScenarioSO> ScenarioLoader = SOLoader<ScenarioSO>.Instance;
		LoadingProgress = 0;
		await ScenarioLoader.LoadData("1000000"); // NPC Archer
		await ScenarioLoader.LoadData("1000001");
		await ScenarioLoader.LoadData("1000002");
		await ScenarioLoader.LoadData("1000003");
		await ScenarioLoader.LoadData("1000004");

		await ScenarioLoader.LoadData("1000010"); // NPC Knight
		await ScenarioLoader.LoadData("1000011");
		await ScenarioLoader.LoadData("1000012");
		await ScenarioLoader.LoadData("1000013");
		await ScenarioLoader.LoadData("1000014");

		await ScenarioLoader.LoadData("1000020"); // Dungeon Gate

		await ScenarioLoader.LoadData("1000030"); // NPC MushroomBeer
		await ScenarioLoader.LoadData("1000031");
		await ScenarioLoader.LoadData("1000032");
		await ScenarioLoader.LoadData("1000033");

		await ScenarioLoader.LoadData("1000040"); // Dungeon Exit Gate

		await ScenarioLoader.LoadData("1000050"); // Dungeon Clear-Exit Gate

		await ScenarioLoader.LoadData("1000060"); // Watch orb
		await ScenarioLoader.LoadData("1000061"); // Watch orb
		await ScenarioLoader.LoadData("1000070"); // Tutorial trigger

		await ScenarioLoader.LoadData("1000080"); // NPC Babarian
		await ScenarioLoader.LoadData("1000081"); // 
		
		LoadingProgress = 5;
		SOLoader<TimelineSO> TimelineLoader = SOLoader<TimelineSO>.Instance;
		await TimelineLoader.LoadData("1100000");
		await TimelineLoader.LoadData("1100001");
		await TimelineLoader.LoadData("1100002");
		await TimelineLoader.LoadData("1100003");
		await TimelineLoader.LoadData("1100004");

		LoadingProgress = 8;
		SOLoader<QuestSO> QuestLoader = SOLoader<QuestSO>.Instance;

		await QuestLoader.LoadData("1500001"); // NPC RED
		await QuestLoader.LoadData("1500002"); // NPC GREEN
		await QuestLoader.LoadData("1500003"); // NPC MushroomBeer

		LoadingProgress = 10;
		Debug.Log("[GameManager] Request Load Scene~ " + gameData.lastPlayedSceneName);
        AsyncSceneManager.Instance.LoadScene(gameData.lastPlayedSceneName);
	}
	private void OnApplicationQuit()
	{
		SaveDataOverwriter.Instance.SaveToFile(gameData);
	}
}
