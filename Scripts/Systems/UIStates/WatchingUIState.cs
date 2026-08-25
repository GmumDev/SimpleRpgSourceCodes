using UnityEngine;

public class WatchingUIState : BaseUIState
{
	static WatchingUIState instance;
	public static WatchingUIState Instance { get => instance; }
	[SerializeField]
	GameObject NotWatchUIPanel;
	[SerializeField]
	GameObject WatchUIPanel;

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
		WatchUIPanel.SetActive(true);
		NotWatchUIPanel.SetActive(false);

		this.machine = machine;
	}
	public override void OnStateUpdate(UIStateMachine machine)
	{

	}
	public override void OnStateExit(UIStateMachine machine)
	{
		if(WatchUIPanel != null)
			WatchUIPanel.SetActive(false);
		if(NotWatchUIPanel != null)
			NotWatchUIPanel.SetActive(true);
	}
}
