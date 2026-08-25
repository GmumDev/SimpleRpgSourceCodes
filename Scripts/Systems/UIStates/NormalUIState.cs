using UnityEngine;

public class NormalUIState : BaseUIState
{
	static NormalUIState instance;
	public static NormalUIState Instance { get => instance; }
	[SerializeField]
	GameObject UIPanel;
	SubscriptionToken onAtktoken;

	UIStateMachine machine;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
			return;
		}
	}
	public override void OnStateEnter(UIStateMachine machine)
	{
		UIPanel.SetActive(true);
		onAtktoken = EventBus.Subscribe<PlayerAttackedEvent>(OnAttacked);

		this.machine = machine;
	}
	public override void OnStateUpdate(UIStateMachine machine)
	{

	}
	public override void OnStateExit(UIStateMachine machine)
	{
		if (UIPanel != null)
			UIPanel.SetActive(false);
		EventBus.Unsubscribe(onAtktoken);
	}
	public void OnAttacked(PlayerAttackedEvent ev)
	{
		machine.ChangeState(machine.battleState);
	}
}
