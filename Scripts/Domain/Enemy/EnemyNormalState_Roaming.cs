using System.Threading;
using UnityEngine;

public class EnemyNormalState_Roaming : StateMachineBehaviour
{
	[SerializeField]
	float speed;

	[SerializeField]
	float moveDuring = 2f;

	[SerializeField]
	float restDuring = 3f;


	[SerializeField]
	float duringOffsetMin = -1f;
	[SerializeField]
	float duringOffsetMax = 1f;

	Enemy enemy;
	CancellationTokenSource cts;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (enemy == null)
			enemy = animator.gameObject.GetComponent<Enemy>();
		enemy.agent.speed = speed;

		animator.ResetTrigger("OnSenced");

		cts = new CancellationTokenSource();
		_ = RoamingAsync(cts.Token);
	}
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);
		if (animator.GetBool("IsReturning")) return;

		if (enemy.agent.isStopped)
		{
			animator.SetFloat("WalkSpeed", 0f);
		}
		else
		{
			animator.SetFloat("WalkSpeed", 1.1f);
		}
	}
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		cts.Cancel();
		base.OnStateExit(animator, stateInfo, layerIndex);
	}
	async Awaitable RoamingAsync(CancellationToken token)
	{
		while(token.IsCancellationRequested == false)
		{
			enemy.agent.SetDestination(enemy.GetNewDestInSpawner());
			enemy.agent.isStopped = false;
			await Awaitable.WaitForSecondsAsync(moveDuring, token);
			if (token.IsCancellationRequested) return;
			enemy.agent.isStopped = true;
			await Awaitable.WaitForSecondsAsync(restDuring, token);
		}
	}

}
