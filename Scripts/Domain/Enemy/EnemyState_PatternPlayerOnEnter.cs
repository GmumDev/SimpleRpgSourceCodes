using UnityEngine;

public class EnemyState_PatternPlayerOnEnter : StateMachineBehaviour
{
	Enemy enemy;
	[SerializeField]
	int patternIdx;


	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (enemy == null)
			enemy = animator.gameObject.GetComponent<Enemy>();

		enemy.DoPattern(patternIdx);
	}
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		enemy.CancelPattern(patternIdx);
	}
}