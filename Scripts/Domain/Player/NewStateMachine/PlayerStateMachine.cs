using UnityEngine;

public class PlayerStateMachine
{
    PlayerState_Base curState;

    public PlayerState_Battle battleState;
	public PlayerState_Watch watchState;
	public PlayerState_Normal normalState;
	public PlayerState_Gathering gatheringState;
	public PlayerState_WaitForAnimStart waitForAnimState;
	public PlayerState_TagAttackOccured tagAttackOccuredState;

	public PlayerStateMachine(Player player)
	{
		var move = new PlayerSubState_Move(speed: 10f, rotSpeed: 10f);
		var look = new PlayerSubState_Look();
		var tglInventory = new PlayerSubState_ToggleInventory();
		var tglQuestbook = new PlayerSubState_ToggleQuestBook();
		var interact = new PlayerSubState_Interact(LayerMask.GetMask("NPC"), LayerMask.GetMask("Gatherable"));

		normalState = new PlayerState_Normal(this, player, new PlayerSubState_Base[]{ move, look, tglInventory, tglQuestbook, interact});
		battleState = new PlayerState_Battle(this, player, new PlayerSubState_Base[] { move, look, tglInventory, tglQuestbook });
		gatheringState = new PlayerState_Gathering(this, player, new PlayerSubState_Base[] { look, tglInventory, tglQuestbook });
		watchState = new PlayerState_Watch(this, player);
		waitForAnimState = new PlayerState_WaitForAnimStart(this, player);
		tagAttackOccuredState = new PlayerState_TagAttackOccured(this, player);

		curState = normalState;
		curState.OnEnter();
	}
	public bool IsState<T>() where T :PlayerState_Base => curState is T;
	public void ChangeState(PlayerState_Base state)
	{
		if (curState == state) return;
		curState.OnExit();
		curState = state;
		curState.OnEnter();
	}
	public void UpdateState()
	{
		curState.OnUpdate();
	}
	string fromTag, toTag;
	PlayerState_Base nextState;
	SubscriptionToken fsmChangeToken;


	// waitForAnimState는 특별한 State
	// 아무 것도 안하는데 현재 재생중인 animation만 추적한다.
	// 바뀔 때마다 Event를 쏜다(PlayerStateChangeRequestedEvent)
	// 그걸 OnFSMChangeRequested가 받는다
	// [언제 쓰냐?] Battle -> Hit -> Battle 에서 Hit하는 동안 경직됨(BattleState가 안 돌아가고 waitForAnimState가 돌아가므로)
	// [또 언제 쓰냐?] Battle -> Attack -> Battle 도 마찬가지 경직. 
	// [원?리] 사이에 낀 애니메이션 노드는 HasExitTime체크해야됨. 그래야 다른 애니메이션으로 바뀌지
	// [아닐 수도 있음] Die -> Revive -> Normal에서 Die는 HasExitTime이 아닌데, 대신 부활 버튼 누르면 Revive로 가고 그게 HasExitTime이라서 Normal로 되돌아옴. 
	public void WaitForAnim(string fromTag, string toTag, PlayerState_Base nextState)
	{
		ChangeState(waitForAnimState);
		EventBus.Unsubscribe(fsmChangeToken);
		fsmChangeToken = EventBus.Subscribe<PlayerStateChangeRequestedEvent>(OnFSMChangeRequested);
		this.fromTag = fromTag;
		this.toTag = toTag;
		this.nextState = nextState;
	}

	// 그걸 여기서 받아서, OnFSMChangeRequested를 실행시킨다
	// 그럼 미리 예약해뒀던 다음 state로 전환시킨다. 
	void OnFSMChangeRequested(PlayerStateChangeRequestedEvent ev)
	{
		if ((ev.animFrom.IsTag(fromTag) || fromTag == "Any") && (ev.animTo.IsTag(toTag) || toTag == "Any"))
		{
			EventBus.Unsubscribe(fsmChangeToken);

			ChangeState(nextState);
		}
	}
	~PlayerStateMachine()
	{
		EventBus.Unsubscribe(fsmChangeToken);
		curState.OnExit();
	}
}
