using UnityEngine.InputSystem;

public class PlayerSubState_ToggleQuestBook : PlayerSubState_Base
{
	InputAction openQuestBookAction = PlayerInputReciever.Instance.OpenQuestBookAction;
	public override void OnEnter(PlayerState_Base baseState, Player player)
	{

	}

	public override void OnExit(PlayerState_Base baseState, Player player)
	{

	}

	public override void OnUpdate(PlayerState_Base baseState, Player player)
	{
		if (openQuestBookAction.triggered)
		{
			QuestBookUISystem.Instance.ToggleBook();
		}
	}
}
