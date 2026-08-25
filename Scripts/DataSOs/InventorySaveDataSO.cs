using System.Collections.Generic;
using UnityEngine;
using static InventorySystem;

[CreateAssetMenu(fileName = "InventorySaveDataSO", menuName = "Scriptable Objects/InventorySaveDataSO")]
public class InventorySaveDataSO : ScriptableObject
{
	public List<InventoryCell> elements;
}
