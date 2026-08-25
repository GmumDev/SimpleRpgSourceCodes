using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO: SORuntimeLoadable
{
    public Sprite icon;
    public string itemName;
}
