using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;


public class InventorySystem: MonoBehaviour, IInventory
{
	[Serializable]
	public class InventoryCell
	{
		public static int maxCnt = 64;
		public string id;
		public int cnt;
		public InventoryIcon icon;
		public InventoryCell(string id, int cnt, InventoryIcon icon)
		{
			this.id = id;
			this.cnt = cnt;
			this.icon = icon;
		}
		public void Swap(InventoryCell cell)
		{
			(this.id, cell.id)		= (cell.id, this.id)	;
			(this.cnt, cell.cnt)	= (cell.cnt, this.cnt)	;
			icon.textfield.text = cnt.ToString();
			icon.image.sprite = ItemDataContainer.GetIconWithId(id);
			cell.icon.textfield.text = cell.cnt.ToString();
			cell.icon.image.sprite = ItemDataContainer.GetIconWithId(cell.id);
		}
	}
	[SerializeField] string inventoryDataKey;
	InventorySaveDataSO inventory;

	/// <summary>
	/// [Flexible Fields]
	/// Padding: 40, 40, 40, 40
	/// CellSize: 100, 100
	/// Spacing: 20, 20
	/// 
	/// [Fixed Fields]
	/// StartCorner: UpperLeft
	/// StartAxis: Horizontal
	/// ChildAlignment: UpperLeft
	/// Constraint: FixedColumnCount
	///		ConstraintCount: 8
	/// </summary>
	GridLayoutGroup layout;
	static InventorySystem instance;
	public static IInventory Instance { get => instance; }
	public bool IsOpened { get => inventoryPanel.activeSelf; }


	[SerializeField]
	GameObject inventoryPanel;
	[SerializeField]
	RectTransform inventoryContent;
	[SerializeField]
	RectTransform inventoryViewport;
	[SerializeField]
	GameObject iconPrefab;
	[SerializeField]
	TextMeshProUGUI[] statTexts;
	
	IObjectPool<InventoryIcon> iconPool; 
   
	private async void Awake()
	{
		if (instance == null)
		{
			instance = this;
			while (AddressableGroupLoader.Instance.IsLoaded == false)
			{
				await Task.Yield();
			}
			inventory = AddressableGroupLoader.Instance.GetSaveData<InventorySaveDataSO>(inventoryDataKey);
			
			layout = inventoryContent.GetComponent<GridLayoutGroup>();
			iconPool = new ObjectPool<InventoryIcon>(
				createFunc: CreateItem,
				actionOnGet: OnGet,
				actionOnRelease: OnRelease,
				actionOnDestroy: OnDestroyItem,
				collectionCheck: true,   // helps catch double-release mistakes
				defaultCapacity: 5,
				maxSize: 20
			);

			// 저장된 데이터 있으면 초기화x
			if (inventory.elements == null)
			{
				inventory.elements = new List<InventoryCell>();
			}
		}
		else
		{
			Destroy(this.gameObject);
			return;
		}
	}

	// ------------------ Public APIs --------------
	void IInventory.StashItem(Vector2Int cellPos)
	{
		var idx = GetCellIdxWithCellPos(cellPos);
		if (IsValidCellIdx(idx))
			RemoveCell(inventory.elements[idx]);
	}
	void IInventory.SwapItem(Vector2Int cellPos1, Vector2Int cellPos2)
	{
		var idx1 = GetCellIdxWithCellPos(cellPos1);
		var idx2 = GetCellIdxWithCellPos(cellPos2);
		if (IsValidCellIdx(idx1) == false || IsValidCellIdx(idx2) == false)
			return;

		// deepcopy swaping
		inventory.elements[idx1].Swap(inventory.elements[idx2]);
	}
	void IInventory.RemoveItem(string id, int cnt)
	{
		throw new NotImplementedException();
	}
	Vector2Int IInventory.GetCellPos(Vector2 pos, out bool isStashing)
	{
		Vector2 origin = RectTransformUtility.WorldToScreenPoint(null, inventoryViewport.position);
		int Tpadding = layout.padding.top;
		int Lpadding = layout.padding.left;
		Vector2 size = layout.cellSize;
		Vector2 spacing = layout.spacing;

		// lefttop pivot
		Vector2Int result = Vector2Int.FloorToInt(new Vector2(pos.x - origin.x + Lpadding, origin.y - pos.y + Tpadding) / (size + spacing));
		isStashing =
			result.x < 0
			|| result.y < 0
			|| result.x >= 8;
		return result;
	}
	public void ToggleInventory()
	{
		if (inventoryPanel.activeSelf)
			inventoryPanel.SetActive(false);
		else
		{
			UpdateUI();
			inventoryPanel.SetActive(true);
		}
	}
	void IInventory.ObtainItem(string id, int cnt)
	{
		List<InventoryCell> targetCells = new List<InventoryCell>();
		for(int i = 0; i < inventory.elements.Count;i++)
		{
			if (inventory.elements[i].id == id && inventory.elements[i].cnt < InventoryCell.maxCnt)
			{
				targetCells.Add(inventory.elements[i]);
			}
		}
		int targetCellCnt = targetCells.Count;
		int totalcnt = cnt;

        while (cnt > 0)
		{
			int addAmount = 0;
			if (targetCellCnt == 0)
			{
				addAmount = Mathf.Min(cnt, InventoryCell.maxCnt);
				inventory.elements.Add(new InventoryCell(id, addAmount, null));
			}
			else
			{
				addAmount = Mathf.Min(cnt, InventoryCell.maxCnt - targetCells[targetCells.Count - targetCellCnt].cnt);
				targetCells[targetCells.Count - targetCellCnt].cnt += addAmount;
				targetCellCnt--;
			}
			cnt -= addAmount;
		}

		if(inventoryPanel.activeSelf)
			UpdateUI();
		var ev = new InventoryChangedEvent(
			itemId: id,
			deltaCnt: totalcnt
            );


		EventBus.Publish(ev);
    }
	bool IInventory.CheckHasItem(string id)
	{
		for (int i = 0; i < inventory.elements.Count; i++)
		{
			if (inventory.elements[i].id == id)
			{
				return true;
			}
		}
		return false;
	}
	int IInventory.GetItemCount(string id)
	{
		int sum = 0;
		for (int i = 0; i < inventory.elements.Count; i++)
		{
			if (inventory.elements[i].id == id)
			{
				sum += inventory.elements[i].cnt;
			}
		}
		return sum;
	}

	#region ObjectPool
	private InventoryIcon CreateItem()
	{
		GameObject gameObject = Instantiate(iconPrefab, inventoryContent);
		gameObject.SetActive(false);
		return gameObject.GetComponent<InventoryIcon>();
	}

	// Called when an item is taken from the pool.
	private void OnGet(InventoryIcon obj)
	{
		obj.gameObject.SetActive(true);
	}

	// Called when an item is returned to the pool.
	private void OnRelease(InventoryIcon obj)
	{
		obj.gameObject.SetActive(false);
	}

	// Called when the pool decides to destroy an item (e.g., above max size).
	private void OnDestroyItem(InventoryIcon obj)
	{
		Destroy(obj.gameObject);
	}
	#endregion


	// ------------------ Privates --------------
	bool IsValidCellIdx(int idx) => idx >= 0 && idx < inventory.elements.Count;
	void UpdateUI()
	{
		for(int i = 0; i < inventory.elements.Count; i++)
		{
			int cnt = inventory.elements[i].cnt;
			if(cnt == 0)
			{
				RemoveCell(inventory.elements[i]);
			}
			else if (cnt > 0)
			{
				if (inventory.elements[i].icon == null)
				{
					InventoryIcon obj = iconPool.Get();
					inventory.elements[i].icon = obj;
					inventory.elements[i].icon.image.sprite = ItemDataContainer.GetIconWithId(inventory.elements[i].id);
				}
				inventory.elements[i].icon.textfield.text = cnt.ToString();
			}
		}

		statTexts[0].text = Player.PlayerData.playerStats.hp.ToString();
		statTexts[1].text = Player.PlayerData.MaxHp.ToString();
		statTexts[2].text = Player.PlayerData.playerStats.exp.ToString();
		statTexts[3].text = Player.PlayerData.MaxExp.ToString();
		statTexts[4].text = Player.PlayerData.AttackMultiplier.ToString();
		statTexts[5].text = Player.PlayerData.DefenceMultiplier.ToString();
	}
	int GetCellIdxWithCellPos(Vector2Int cellPos)
	{
		int idx = cellPos.x + cellPos.y * layout.constraintCount;
		return idx;
	}
	void RemoveCell(InventoryCell cell)
	{
		iconPool.Release(cell.icon);
		inventory.elements.Remove(cell);
	}
	private void OnApplicationQuit()
	{
		SaveDataOverwriter.Instance.SaveToFile(inventory);
	}
}
