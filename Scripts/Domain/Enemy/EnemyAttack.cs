using System.Threading;
using UnityEngine;

public class EnemyAttack: MonoBehaviour
{
	[SerializeField]
	Animator enemyAnim;

	[SerializeField]
	float atkCooldown = 3f;

	[SerializeField]
	LayerMask targetLayer;
	[SerializeField]
	SkillAttackCollider atkCollider;
	[SerializeField]
	int frameThatActivateColliderAfter;

	float lastAtkTime = 0f;
	CancellationTokenSource cts;

	private void Update()
	{
		if(cts != null && enemyAnim.GetCurrentAnimatorStateInfo(0).IsTag("Battle") == false)
		{
			cts.Cancel();
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if(other.CompareTag("Player") && Time.time - lastAtkTime > atkCooldown && enemyAnim.GetCurrentAnimatorStateInfo(0).IsTag("Battle"))
		{
			transform.rotation = Quaternion.LookRotation(other.transform.position - transform.position);
			lastAtkTime = Time.time;
			enemyAnim.SetTrigger("OnAttack");

			// token assign
			cts = new CancellationTokenSource();
			_ = TriggerAfterFrames(frameThatActivateColliderAfter, cts.Token);
		}
	}
	async Awaitable TriggerAfterFrames(int frames, CancellationToken token)
	{
		while(frames-->0)
			await Awaitable.NextFrameAsync(token);

		if (token.IsCancellationRequested) return;

		atkCollider?.Trigger(targetLayer);

		// token release
		cts.Cancel();
		cts.Dispose();
		cts = null;
	}
}
