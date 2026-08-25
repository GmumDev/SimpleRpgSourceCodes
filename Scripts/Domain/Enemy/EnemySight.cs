using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [SerializeField]
    Animator enemyAnim;
	bool activeSight;
	private void Update()
	{
		// 플레이어를 감지하고 배틀 상태일 때도 계속해서 현재 state를 체크함ㅇㅅㅇ;;
		if (!activeSight && enemyAnim.GetCurrentAnimatorStateInfo(0).IsTag("Normal"))
		{
			activeSight = true;
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (!activeSight) return;
		if (other.CompareTag("Player"))
		{
			activeSight = false;
			enemyAnim.SetTrigger("OnSenced");

		}
	}
}
