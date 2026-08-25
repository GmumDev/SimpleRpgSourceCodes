
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleState_ChosingPattern : StateMachineBehaviour
{
	Enemy enemy;

	Transform playerTransform;

	[SerializeField]
	List<float> patternWeight;
	[SerializeField]
	float patternTerm;
	float lastPatternTime;
	float sumOfPatternWeights;
	private int GetRandomPatternIdx()
	{
		var value = Random.Range(0, sumOfPatternWeights);
		float sum = 0;
		for(int i = 0; i < patternWeight.Count;i++)
		{
			if(value < (sum += patternWeight[i]))
			{
				return i;
			}
		}
		Debug.LogWarning("Random Logic Wrong!!!");
		return 0;
	}
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		sumOfPatternWeights = 0;
		patternWeight.ForEach((x) => sumOfPatternWeights += x);

		base.OnStateEnter(animator, stateInfo, layerIndex);
		if(enemy == null)
			enemy = animator.gameObject.GetComponent<Enemy>();
		playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
		animator.ResetTrigger("OnAttack");
		animator.SetBool("IsReturning", false);
	}
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);

		Vector3 dir = playerTransform.position - enemy.gameObject.transform.position;

		if (Time.time - lastPatternTime > patternTerm)
		{
			lastPatternTime = Time.time;
			animator.SetFloat("AttackType", GetRandomPatternIdx());
			animator.SetTrigger("OnAttack");
		}
	}
}
