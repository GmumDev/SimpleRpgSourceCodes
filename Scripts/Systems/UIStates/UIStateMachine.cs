using UnityEngine;

public class UIStateMachine : MonoBehaviour
{
    static UIStateMachine instance;
    public static UIStateMachine Instance { get => instance; }

    BaseUIState curState;

	public BaseUIState normalState;
	public BaseUIState battleState;
	public BaseUIState watchingState;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
	{
		if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
	}
	void Start()
    {
        curState = normalState;
        curState.OnStateEnter(this);
	}

    // Update is called once per frame
    void Update()
    {
        curState?.OnStateUpdate(this);
    }
    public void ChangeState(BaseUIState state)
    {
        curState?.OnStateExit(this);
        curState = state;
        curState?.OnStateEnter(this);
    }
	private void OnDestroy()
	{
        curState?.OnStateExit(this);
	}
}
