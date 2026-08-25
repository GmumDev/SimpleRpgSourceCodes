using System.Collections.Generic;
using UnityEngine;

public class ItemDataContainer : MonoBehaviour
{
	Dictionary<string, ItemSO> itemDatas;
	public static Sprite GetIconWithId(string id)
	{
		if (instance.itemDatas.ContainsKey(id))
		{
			return instance.itemDatas[id].icon;
		}

		throw new System.Exception("그런 id를 가진 item은 없다");
	}
	public static string GetNameWithId(string id)
	{
		if (instance.itemDatas.ContainsKey(id))
		{
			return instance.itemDatas[id].itemName;
		}

		throw new System.Exception("그런 id를 가진 item은 없다");
	}
	static ItemDataContainer instance;
	public static ItemDataContainer Instance
	{
		get
		{
			if (instance == null)
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

			itemDatas = new Dictionary<string, ItemSO>();

			SOLoader<ItemDataBaseSO> ItemDBLoader = SOLoader<ItemDataBaseSO>.Instance;
			await ItemDBLoader.LoadData("ItemDataBaseSO");
			ItemDataBaseSO datas = ItemDBLoader.GetSO("ItemDataBaseSO");
			foreach(var item in datas.items)
			{
				itemDatas.Add(item.id, item);
				Debug.Log($"Item Loaded:{item.id}, {item.itemName}");
			}
		}
		else
		{
			Destroy(this.gameObject);
		}
	}
}
