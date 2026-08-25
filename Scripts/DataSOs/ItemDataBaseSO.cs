using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBaseSO", menuName = "ScriptableObjects/DataBase/ItemDataBaseSO")]
public class ItemDataBaseSO : SORuntimeLoadable
{
    public ItemSO[] items;
}
