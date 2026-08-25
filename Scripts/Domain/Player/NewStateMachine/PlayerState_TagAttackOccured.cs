using UnityEngine;

public class PlayerState_TagAttackOccured : PlayerState_Base
{
	public PlayerState_TagAttackOccured(PlayerStateMachine machine, Player player) : base(machine, player)
	{
	}

	public override void OnEnter()
	{
		player.ChangeChar();
		player.animator.SetTrigger("OnTagEnter");
		DoAttack();
		machine.WaitForAnim("TagEnter", "Battle", machine.battleState);
	}
	public override void OnUpdate()
	{
	}
	public override void OnExit()
	{
	}
	void DoAttack()
	{
		// 태그스킬은 4번 고정(0, 1, 2, 3, 4인데 3은 TagOut, 4가 TagIn임. 여기는 TagIn이고)
		int i = 4;

		// 쓸 수 있는 스킬인지 확인한다(null 체크, 쿨타임 체크 등)
		if (player.AttackSkill[i] == null) return;
		if (player.AttackSkill[i].CanAttack() == false) return;

		// 방향 바라보고 때린다.
		var dir = player.fwdCamTarget.forward;
		dir.y = 0;
		Quaternion targetRot = Quaternion.LookRotation(dir);
		player.playerBody.localRotation = targetRot;
		player.AttackSkill[i].Attack();
		
	}
}
