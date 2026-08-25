using System.Threading;
using UnityEngine;

public class EnemyPattern_SimpleAttack : EnemyPattern
{
	[SerializeField]
	LayerMask targetLayer;
	[SerializeField]
	SkillAttackCollider atkCollider;
	[SerializeField]
	int frameThatActivateColliderAfter;


	CancellationTokenSource cts;

	public override void CancelPattern()
	{
		if(cts != null)
			cts.Cancel();
	}

	public override void DoPattern()
	{
		// token assign
		cts = new CancellationTokenSource();
		_ = TriggerAfterFrames(frameThatActivateColliderAfter, cts.Token);
	}
	async Awaitable TriggerAfterFrames(int frames, CancellationToken token)
	{
		while (frames-- > 0)
			await Awaitable.NextFrameAsync(token);

		if (token.IsCancellationRequested) return;

		atkCollider?.Trigger(targetLayer);

		// token release
		cts.Cancel();
		cts.Dispose();
		cts = null;
	}
}
