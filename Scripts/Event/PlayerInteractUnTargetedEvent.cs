
public class PlayerInteractUnTargetedEvent
{
    public IInteractable target;

    public PlayerInteractUnTargetedEvent(IInteractable target)
    {
        this.target = target;
    }
}
