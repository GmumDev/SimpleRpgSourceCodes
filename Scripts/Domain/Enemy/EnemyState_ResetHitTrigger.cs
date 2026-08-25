using UnityEngine;

public class EnemyState_ResetHitTrigger : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("OnHit");
	}
}
