using UnityEngine;

public class EnemyState_CheckForReturning : StateMachineBehaviour
{
	Enemy enemy;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		enemy = animator.gameObject.GetComponent<Enemy>();
    }
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);

		if ((enemy.SpawnerCenter - enemy.transform.position).magnitude > enemy.RangeFromCenter)
		{
			animator.SetBool("IsReturning", true);
		}
		// chase
	}
}
