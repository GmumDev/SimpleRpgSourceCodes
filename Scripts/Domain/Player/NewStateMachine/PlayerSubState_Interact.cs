using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSubState_Interact : PlayerSubState_Base
{
	LayerMask npcLayerMask;
	LayerMask gatherableLayerMask;
	bool interactHovering;
	IInteractable target;

	InputAction interactAction = PlayerInputReciever.Instance.InteractAction;

	Collider[] overlapedColliders = new Collider[1];
	public PlayerSubState_Interact(LayerMask npcLayerMask
		, LayerMask gatherableLayerMask)
	{
		this.npcLayerMask = npcLayerMask;
		this.gatherableLayerMask = gatherableLayerMask;
	}
	public override void OnEnter(PlayerState_Base baseState, Player player)
	{
		interactHovering = false;
	}

	public override void OnExit(PlayerState_Base baseState, Player player)
	{
		if (baseState.machine.IsState<PlayerState_Gathering>() == false)
			InteractUISystem.Instance.InteractTargetedOff();
	}

	public override void OnUpdate(PlayerState_Base baseState, Player player)
	{
		Interact(baseState.machine, player);
	}
	private void Interact(PlayerStateMachine machine, Player player)
	{
		if (interactHovering && AsyncSceneManager.Instance.IsLoading == false && interactAction.WasPressedThisFrame())
		{
			if (IsInteractReady(npcLayerMask, player))
			{
				target?.OnInteract();
				machine.ChangeState(machine.watchState);
			}
			else if (IsInteractReady(gatherableLayerMask, player))
			{
				target?.OnInteract();
				machine.ChangeState(machine.gatheringState);
			}
		}
		else
		{
			if (IsInteractReady(npcLayerMask | gatherableLayerMask, player))
			{
				if (interactHovering == false)
				{
					InteractUISystem.Instance.InteractTargetedOn(target.GetInteractableType());
				}
				interactHovering = true;
			}
			else
			{
				if (interactHovering == true)
				{
					InteractUISystem.Instance.InteractTargetedOff();
				}
				interactHovering = false;
			}
		}
	}

	bool IsInteractReady(LayerMask layerMask, Player player)
	{
		int n = Physics.OverlapBoxNonAlloc(player.fwdCamTarget.position, new Vector3(2f, 2f, 2f), overlapedColliders, player.fwdCamTarget.rotation, layerMask);
		for(int i = 0; i < n; i++)
		{
			target = overlapedColliders[i].gameObject.GetComponent<IInteractable>();
		}
		if (n == 0)
			target = null;
		return target != null;
	}
}
