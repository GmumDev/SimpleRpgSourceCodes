using UnityEngine;

public class PlayerStateChangeRequestedEvent
{
	public AnimatorStateInfo animFrom;
	public AnimatorStateInfo animTo;
	public PlayerStateChangeRequestedEvent(AnimatorStateInfo animFrom, AnimatorStateInfo animTo)
	{
		this.animFrom = animFrom;
		this.animTo = animTo;
	}
}
