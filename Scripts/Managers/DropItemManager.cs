using UnityEngine;
using UnityEngine.Pool;

public class DropItemManager : MonoBehaviour
{
	static DropItemManager instance;
	public static DropItemManager Instance { get => instance; }

	[SerializeField]
	GameObject dropItemPrefab;
	IObjectPool<ItemObj> pool;

	private void Awake()
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

		pool = new ObjectPool<ItemObj>(
			createFunc: CreateItem,
				actionOnGet: OnGet,
				actionOnRelease: OnRelease,
				actionOnDestroy: OnDestroyItem,
				collectionCheck: true,   // helps catch double-release mistakes
				defaultCapacity: 10,
				maxSize: 100
		);
	}

	public void Generate(Vector3 position, DropTableSO dropTableSO)
	{
		for(int i = 0; i < dropTableSO.rows.Count; i++)
		{
			var row = dropTableSO.rows[i];
			int amount = row.amount_value1;
			switch(row.dropType)
			{
				case DropItemAmountType.BtwTwoValue:
					amount = Random.Range(row.amount_value1, row.amount_value2 + 1);
					break;
				case DropItemAmountType.FixedValue:
					amount = row.amount_value1;
					break;
			}
			var obj = pool.Get();
			obj.gameObject.transform.position = position;
			obj.Init(OnRelease, row.itemID, amount);
		}
	}

	#region ObjectPool
	private ItemObj CreateItem()
	{
		GameObject gameObject = Instantiate(dropItemPrefab);
		var obj = gameObject.GetComponent<ItemObj>();
		return obj;
	}

	// Called when an item is taken from the pool.
	private void OnGet(ItemObj obj)
	{
		obj.gameObject.SetActive(true);
	}

	// Called when an item is returned to the pool.
	private void OnRelease(ItemObj obj)
	{
		obj.gameObject.SetActive(false);
	}

	// Called when the pool decides to destroy an item (e.g., above max size).
	private void OnDestroyItem(ItemObj obj)
	{
		Destroy(obj.gameObject);
	}
	#endregion
}
