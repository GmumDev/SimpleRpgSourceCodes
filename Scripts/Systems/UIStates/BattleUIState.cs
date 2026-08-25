using TMPro;
using UnityEngine;

public class BattleUIState : BaseUIState
{
	static BattleUIState instance;
	public static BattleUIState Instance { get => instance; }
	[SerializeField]
	GameObject UIPanel;
	[SerializeField]
	TextMeshProUGUI[] btns;

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
	}
	public override void OnStateUpdate(UIStateMachine machine)
	{

	}
	public override void OnStateExit(UIStateMachine machine)
	{
		if (UIPanel != null)
			UIPanel.SetActive(false);
	}
}