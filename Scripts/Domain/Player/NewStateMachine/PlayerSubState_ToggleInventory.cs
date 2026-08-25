using UnityEngine.InputSystem;

public class PlayerSubState_ToggleInventory : PlayerSubState_Base
{
	InputAction openInventoryAction = PlayerInputReciever.Instance.OpenInventoryAction;
	public override void OnEnter(PlayerState_Base baseState, Player player)
	{

	}

	public override void OnExit(PlayerState_Base baseState, Player player)
	{

	}

	public override void OnUpdate(PlayerState_Base baseState, Player player)
	{
		if (openInventoryAction.triggered)
		{
			InventorySystem.Instance.ToggleInventory();
		}
	}
}
