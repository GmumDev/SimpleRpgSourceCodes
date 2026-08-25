using UnityEngine;

public class EnemyReturningState: StateMachineBehaviour
{
	Enemy enemy;

	[SerializeField]
	float speed;


	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (enemy == null)
			enemy = animator.gameObject.GetComponent<Enemy>();
		enemy.agent.speed = speed;

		enemy.agent.SetDestination(enemy.GetNewDestInSpawner());
		enemy.agent.isStopped = false;
	}
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);

		if (!enemy.agent.pathPending && enemy.agent.remainingDistance < 0.5f)
		{
			animator.SetBool("IsReturning", false);
		}
	}
}
