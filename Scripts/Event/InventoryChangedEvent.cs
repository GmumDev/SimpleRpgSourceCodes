
public class InventoryChangedEvent
{
    public string itemId;
	public int deltaCnt;

	public InventoryChangedEvent(
		string itemId,
		int deltaCnt)
	{
		this.itemId = itemId;
		this.deltaCnt = deltaCnt;
	}
}
