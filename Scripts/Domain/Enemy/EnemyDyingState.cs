using UnityEngine;

public class EnemyDyingState: StateMachineBehaviour
{
	Enemy enemy;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (enemy == null)
			enemy = animator.gameObject.GetComponent<Enemy>();
		enemy.agent.isStopped = true;
	}
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateExit(animator, stateInfo, layerIndex);
		animator.gameObject.GetComponent<Enemy>().DestroyThisEnemy();
	}
}
