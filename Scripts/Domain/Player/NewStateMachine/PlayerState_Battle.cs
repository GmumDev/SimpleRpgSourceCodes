
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Battle : PlayerState_Base
{
	float battleStateUpdatedTime;
	PlayerSubState_Base[] subStates;

	InputAction[] attackActions = PlayerInputReciever.Instance.AttackActions;

	public PlayerState_Battle(PlayerStateMachine machine, Player player, PlayerSubState_Base[] subStates) : base(machine, player)
	{
		this.subStates = subStates;
	}
	public override void OnEnter()
	{
		UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.battleState);
		player.animator.SetBool("IsBattle", true);
		battleStateUpdatedTime = Time.time;

		for (int i = 0; i < attackActions.Length; i++)
		{
			attackActions[i].performed += OnAttack;
		}
		foreach (var state in subStates)
			state.OnEnter(this, player);
	}
	public override void OnUpdate()
	{
		if (Time.time - battleStateUpdatedTime > 5f)
		{
			machine.ChangeState(machine.normalState);
		}

		foreach (var state in subStates)
			state.OnUpdate(this, player);
	}
	public override void OnExit()
	{
		player.animator.SetBool("IsBattle", false);

		for (int i = 0; i < attackActions.Length; i++)
		{
			attackActions[i].performed -= OnAttack;
		}
		foreach (var state in subStates)
			state.OnExit(this, player);
	}
	void OnAttack(InputAction.CallbackContext obj)
	{
		// 어떤 스킬 쓴건지 찾는다.
		int i = -1;
		for (int k = 0; k < attackActions.Length; k++)
		{
			if (attackActions[k].WasPressedThisFrame())
			{
				i = k;
				break;
			}
		}
		// 버튼 누른게 아니라 뗀거에서 날아온 이벤트다
		if (i == -1) return;

		// 쓸 수 있는 스킬인지 확인한다(null 체크, 쿨타임 체크 등)
		if (player.AttackSkill[i] == null) return;
		if (player.AttackSkill[i].CanAttack() == false) return;

		// 방향 바라보고 때린다.
		var dir = player.fwdCamTarget.forward;
		dir.y = 0;
		Quaternion targetRot = Quaternion.LookRotation(dir);
		player.playerBody.localRotation = targetRot;
		player.AttackSkill[i].Attack();
		EventBus.Publish(new PlayerAttackedEvent(i, player.AttackSkill[i].MaxCooldown));

		// 애니메이션 넘어간다
		player.animator.SetFloat("AttackStep", player.AttackSkill[i].CurStep);
		player.animator.SetTrigger("OnAttack");

		// State 바뀌고, 거기서 애니메이션 transition 감지해서 되돌아온다. 
		if(i == 3)
		{
			// i == 3은 태그스킬로 캐릭터 변경 요청임. 현재 캐릭터의 공격 애니메이션 끝나면 TagAttackOccuredState로 감.
			// TagAttackOccuredState를 보면, 먼저 캐릭 바꾸고, 그 캐릭에서 태그 수신 어택 애니메이션을 재생하고 battle로 감.
			machine.WaitForAnim("Attack", "Battle", machine.tagAttackOccuredState);
		}
		else
		{
			machine.WaitForAnim("Attack", "Battle", this);
		}
		
	}

}
