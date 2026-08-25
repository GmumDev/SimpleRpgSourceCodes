using UnityEngine;

public interface IInventory
{
    public bool CheckHasItem(string id);
    public int GetItemCount(string id);
    public void ObtainItem(string id, int cnt);
    public void StashItem(Vector2Int cellPos);
    public void RemoveItem(string id, int cnt);
    public void SwapItem(Vector2Int cellPos1, Vector2Int cellPos2);
	public void ToggleInventory();
    public bool IsOpened { get; }
    public Vector2Int GetCellPos(Vector2 pos, out bool isInViewport);

}
