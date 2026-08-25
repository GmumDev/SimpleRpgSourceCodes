
public abstract class PlayerState_Base
{
	public PlayerStateMachine machine;
	protected Player player;
	public PlayerState_Base(PlayerStateMachine machine, Player player)
	{
		this.machine = machine;
		this.player = player;
	}
	public abstract void OnEnter();
	public abstract void OnUpdate();
	public abstract void OnExit();
}
