using UnityEngine.InputSystem;

public class PlayerState_Gathering : PlayerState_Base
{
	SubscriptionToken token;
	PlayerSubState_Base[] subStates;

	InputAction interactAction = PlayerInputReciever.Instance.InteractAction;

	public PlayerState_Gathering(PlayerStateMachine machine, Player player, PlayerSubState_Base[] subStates) : base(machine, player)
	{
		this.subStates = subStates;
	}
	public override void OnEnter()
	{
		token = EventBus.Subscribe<GatheringFinishedEvent>(HandleGatheringFinished);

		InteractUISystem.Instance.InteractTargetedOn("Gathering");
		player.animator.SetBool("IsGathering", true);

		foreach (var state in subStates)
			state.OnEnter(this, player);
	}

	public override void OnUpdate()
	{
		if (interactAction.WasPressedThisFrame())
		{
			EventBus.Publish(new GatheringCancelledEvent());
			machine.ChangeState(machine.normalState);
		}
		foreach (var state in subStates)
			state.OnUpdate(this, player);
	}

	public override void OnExit()
	{
		EventBus.Unsubscribe(token);

		if (machine.IsState<PlayerState_Normal>() == false)
			InteractUISystem.Instance.InteractTargetedOff();

		player.animator.SetBool("IsGathering", false);
		foreach (var state in subStates)
			state.OnExit(this, player);
	}
	void HandleGatheringFinished(GatheringFinishedEvent ev)
	{
		player.animator.SetTrigger("OnGathered");
		player.animator.SetBool("IsGathering", false);
		machine.WaitForAnim("Bow", "Normal", machine.normalState);
	}
}
