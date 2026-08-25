using UnityEngine;

public abstract class BaseUIState : MonoBehaviour
{
	public abstract void OnStateEnter(UIStateMachine machine);
	public abstract void OnStateUpdate(UIStateMachine machine);
	public abstract void OnStateExit(UIStateMachine machine);
}
