
public class PlayerInteractTargetedEvent
{
    public IInteractable target;

    public PlayerInteractTargetedEvent(IInteractable target)
    {
        this.target = target;
    }
}
