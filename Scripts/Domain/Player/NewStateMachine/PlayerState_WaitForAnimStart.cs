using UnityEngine;

public class PlayerState_WaitForAnimStart : PlayerState_Base
{
	AnimatorStateInfo from;
	public PlayerState_WaitForAnimStart(PlayerStateMachine machine, Player player) : base(machine, player)
	{

	}

	public override void OnEnter()
	{
		from = player.animator.GetCurrentAnimatorStateInfo(0);
	}

	public override void OnUpdate()
	{
		var to = player.animator.GetCurrentAnimatorStateInfo(0);
		if (from.fullPathHash != to.fullPathHash)
		{
			EventBus.Publish(new PlayerStateChangeRequestedEvent(from, to));
		}
		from = to;
	}

	public override void OnExit()
	{

	}
}
