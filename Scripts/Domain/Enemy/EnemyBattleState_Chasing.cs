using UnityEngine;

public class EnemyBattleState_Chasing : StateMachineBehaviour
{
	Enemy enemy;
	[SerializeField]
	float minDistToPlayer = 1f;
	[SerializeField]
	float maxDistToPlayer = 10f;
	[SerializeField]
	float speed;

	float lastDestCheckT;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (enemy == null)
			enemy = animator.gameObject.GetComponent<Enemy>();
		enemy.agent.speed = speed;
		enemy.agent.SetDestination(Player.Position);
		enemy.agent.isStopped = false;
		lastDestCheckT = Time.time;
	}
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);
		if (animator.GetBool("IsReturning")) return;

		if(!enemy.agent.pathPending)
		{
			// 1초에 1번 경로 계산
			if (Time.time - lastDestCheckT > 1f)
			{
				lastDestCheckT = Time.time;
				enemy.agent.SetDestination(Player.Position);
			}
			// 경로 계산 후, 거리가 지나치게 멀다면 귀환
			if (enemy.agent.remainingDistance > maxDistToPlayer)
			{
				animator.SetBool("IsReturning", true);
			}
			// 계산 후, 쫒아갈 거리라면 쫒아가
			else if (enemy.agent.remainingDistance > minDistToPlayer)
			{
				animator.SetFloat("WalkSpeed", speed);
				enemy.agent.isStopped = false;
			}
			// 계산 후, 너무 가까우면 멈춰
			else
			{
				animator.SetFloat("WalkSpeed", 0);
				enemy.agent.isStopped = true;
			}
		}
			

	}
}
