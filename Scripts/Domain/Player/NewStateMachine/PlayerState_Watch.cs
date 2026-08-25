using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Watch : PlayerState_Base
{
	SubscriptionToken token;
	InputAction navigateAction = PlayerInputReciever.Instance.NavigateAction;
	InputAction interactAction = PlayerInputReciever.Instance.InteractAction;

	public PlayerState_Watch(PlayerStateMachine machine, Player player) : base(machine, player)
	{

	}
	public override void OnEnter()
	{
		token = EventBus.Subscribe<ScenarioFinishedEvent>(HandleWatchingFinished);
		UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.watchingState);
		player.animator.SetBool("IsWatching", true);
	}
	public override void OnUpdate()
	{
		// nav
		Vector2 navInput = navigateAction.ReadValue<Vector2>();
		if (navInput != Vector2.zero)
			ScenarioManager.Instance.SelectChoices(navInput);

		// next page
		if (interactAction.WasPressedThisFrame() && !TimelinePlayer.Instance.IsPlaying)
		{
			ScenarioManager.Instance.NextNode();
		}
	}
	public override void OnExit()
	{
		EventBus.Unsubscribe(token);
		player.animator.SetBool("IsWatching", false);
	}
	void HandleWatchingFinished(ScenarioFinishedEvent ev)
	{
		machine.ChangeState(machine.normalState);
	}
}
