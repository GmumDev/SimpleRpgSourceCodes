using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class QuestCompleteUISystem : MonoBehaviour
{
	static QuestCompleteUISystem instance;
	[Header("Quest Completed Panel")]
	[SerializeField]
	GameObject questCompletePanel;

	[SerializeField]
	Transform questRewardIconsLayout;

	[SerializeField]
	TextMeshProUGUI questTitleText;

	[SerializeField]
	GameObject questRewardIconPrefab;   // object pool, need ImageComponent



	IObjectPool<Image> questRewardCellPool;

	List<SubscriptionToken> tokens = new List<SubscriptionToken>();

	List<Image> cells = new List<Image>();
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
	}
	void Start()
	{
		questRewardCellPool = new ObjectPool<Image>(
			createFunc: CreateItem,
			actionOnGet: OnGet,
			actionOnRelease: OnRelease,
			actionOnDestroy: OnDestroyItem,
			collectionCheck: true,   // helps catch double-release mistakes
			defaultCapacity: 5,
			maxSize: 20
		);
		questCompletePanel.SetActive(false);
	}
	private void OnEnable()
	{
		tokens.Add(EventBus.Subscribe<QuestCompletedEvent>(OnQuestCompleted));
	}
	void OnQuestCompleted(QuestCompletedEvent ev)
	{
		questTitleText.text = ev.ctx.title;
		cells.ForEach(x => questRewardCellPool.Release(x));
		cells.Clear();

		int rewardLength = ev.ctx.rewardContexts.Length;

		for (int i = 0; i < rewardLength; i++)
		{
			var reward = ev.ctx.rewardContexts[i];

			Sprite sprite = ItemDataContainer.GetIconWithId(reward.rewardId);
			var cell = questRewardCellPool.Get();
			cell.sprite = sprite;
			cells.Add(cell);
		}
		questCompletePanel.SetActive(true);
	}
	private void OnDisable()
	{
		if(questRewardCellPool != null)
			foreach(var cell in cells)
			{
				if(cell != null)
					questRewardCellPool.Release(cell);
			}
		cells.Clear();
		if (tokens != null)
			foreach (var token in tokens)
				EventBus.Unsubscribe(token);
	}




	#region ObjectPool
	private Image CreateItem()
	{
		GameObject gameObject = Instantiate(questRewardIconPrefab, questRewardIconsLayout);

		gameObject.SetActive(false);
		return gameObject.GetComponent<Image>();
	}

	// Called when an item is taken from the pool.
	private void OnGet(Image image)
	{
		image.gameObject.SetActive(true);
	}

	// Called when an item is returned to the pool.
	private void OnRelease(Image image)
	{
		image.gameObject.SetActive(false);
	}

	// Called when the pool decides to destroy an item (e.g., above max size).
	private void OnDestroyItem(Image image)
	{
		Destroy(image.gameObject);
	}
	#endregion
}
