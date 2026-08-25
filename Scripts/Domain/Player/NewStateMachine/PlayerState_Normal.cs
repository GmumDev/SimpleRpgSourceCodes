
using UnityEngine.InputSystem;

public class PlayerState_Normal : PlayerState_Base
{

	PlayerSubState_Base[] subStates;
	InputAction[] attackActions = PlayerInputReciever.Instance.AttackActions;
	InputAction changeAction = PlayerInputReciever.Instance.ChangeAction;
	int atkPatternCnt;
	public PlayerState_Normal(PlayerStateMachine machine, Player player
		, PlayerSubState_Base[] subStates) : base(machine, player)
	{
		atkPatternCnt = attackActions.Length;
		this.subStates = subStates;
	}
	public override void OnEnter()
	{
		UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.normalState);
		
		changeAction.performed += ChangeAction_performed;
		foreach (var action in attackActions)
			action.performed += AttackAction_performed;
		foreach (var state in subStates)
			state.OnEnter(this, player);
	}
	public override void OnUpdate()
	{
		foreach (var state in subStates)
			state.OnUpdate(this, player);
	}
	public override void OnExit()
	{
		changeAction.performed -= ChangeAction_performed;
		foreach (var action in attackActions)
			action.performed -= AttackAction_performed;
		foreach (var state in subStates)
			state.OnExit(this, player);
	}

	// ------------ Listener ------------
	private void AttackAction_performed(InputAction.CallbackContext obj)
	{
		for (int i = 0; i < atkPatternCnt; i++)
			if (obj.action == attackActions[i])
			{
				player.AssignSkills();
				machine.ChangeState(machine.battleState);
				break;
			}
	}
	private void ChangeAction_performed(InputAction.CallbackContext obj)
	{
		player.ChangeChar();
	}

}
