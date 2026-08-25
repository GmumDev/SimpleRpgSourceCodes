using UnityEngine.InputSystem;

public class PlayerInputReciever
{
	static PlayerInputReciever instance;
	public static PlayerInputReciever Instance
	{
		get
		{
			if (instance != null)
			{
				return instance;
			}
			instance = new PlayerInputReciever();
			return instance;
		}
	}
	// Attack Actions
	public InputAction[] AttackActions { get; private set; }
	private InputAction AttackAction;
	private InputAction AttackAction2;
	private InputAction AttackAction3;
	private InputAction AttackAction4;

	// Player Actions
	public InputAction InteractAction { get; private set; }
	public InputAction ChangeAction { get; private set; }
	public InputAction LookAction { get; private set; }
	public InputAction MoveAction { get; private set; }
	public InputAction DashAction { get; private set; }
	// UI Actions
	public InputAction OpenInventoryAction { get; private set; }
	public InputAction OpenQuestBookAction { get; private set; }
	public InputAction NavigateAction { get; private set; }
	public InputAction ESCAction { get; private set; }
	
	PlayerInputReciever()
	{
		AttackAction = InputSystem.actions.FindAction("Attack");
		AttackAction2 = InputSystem.actions.FindAction("Attack2");
		AttackAction3 = InputSystem.actions.FindAction("Attack3");
		AttackAction4 = InputSystem.actions.FindAction("Attack4");
		AttackActions = new InputAction[] {
			AttackAction,
			AttackAction2,
			AttackAction3,
			AttackAction4
		};

		InteractAction = InputSystem.actions.FindAction("Interact");
		NavigateAction = InputSystem.actions.FindAction("Navigate");
		OpenInventoryAction = InputSystem.actions.FindAction("OpenInventory");
		ChangeAction = InputSystem.actions.FindAction("Change");
		LookAction = InputSystem.actions.FindAction("Look");
		MoveAction = InputSystem.actions.FindAction("Move");
		DashAction = InputSystem.actions.FindAction("Dash");
		OpenQuestBookAction = InputSystem.actions.FindAction("OpenQuestBook");
		ESCAction = InputSystem.actions.FindAction("ESC");
	}
}
