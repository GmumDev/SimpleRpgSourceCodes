using UnityEngine;

public class NPC_WalkAnimBlender : StateMachineBehaviour
{
	Vector3 lastPos = Vector3.zero;
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);

		var curPos = animator.gameObject.transform.position;
		animator.SetFloat("Blend", (lastPos - curPos).magnitude > 0 ? 1 : 0);
		lastPos = curPos;
	}
}
