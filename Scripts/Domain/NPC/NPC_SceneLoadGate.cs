
using UnityEngine;

public class NPC_SceneLoadGate : NPC
{
	[SerializeField]
	Transform camTarget;

	[SerializeField]
	DungeonDataSO dungeonData;
	public bool IsDungeonGate;
	private bool isTalking;
    private SubscriptionToken scenarioFinishedToken;
	private SubscriptionToken scenarioNodeFinishedToken;
	[SerializeField] string sceneName;

	// talking finished
	void OnScenarioFinished(ScenarioFinishedEvent ev)
	{
		EventBus.Unsubscribe(scenarioFinishedToken);
		EventBus.Unsubscribe(scenarioNodeFinishedToken);
		CameraManager.Instance.TogglePlayerCulling();
		isTalking = false;
	}

	void OnScenarioNodeFinished(ScenarioNodeFinishedEvent ev)
	{
		InteractUISystem.Instance.InteractTargetedOff();
		if ((ev.eventType & ScenarioNodeFinishedEventType.LoadScene) == ScenarioNodeFinishedEventType.LoadScene)
		{
			if(IsDungeonGate) DungeonLoopManager.dungeonData = this.dungeonData;
			AsyncSceneManager.Instance.LoadScene(sceneName);
		}
	}
	
    public override void OnInteract()
    {
        isTalking = ScenarioManager.Instance.PlayScenario(npcSo.scenarioId);

        if (isTalking == false) return;

		if (scenarioFinishedToken != null)
			EventBus.Unsubscribe(scenarioFinishedToken);
		if (scenarioNodeFinishedToken != null)
			EventBus.Unsubscribe(scenarioNodeFinishedToken);

		scenarioFinishedToken = EventBus.Subscribe<ScenarioFinishedEvent>(OnScenarioFinished);
        scenarioNodeFinishedToken = EventBus.Subscribe<ScenarioNodeFinishedEvent>(OnScenarioNodeFinished);
		

        CameraManager.Instance.FocusTalker(camTarget);
		CameraManager.Instance.TogglePlayerCulling();
	}

	protected void OnDisable()
	{
		if (scenarioFinishedToken != null)
			EventBus.Unsubscribe(scenarioFinishedToken);
		if (scenarioNodeFinishedToken != null)
			EventBus.Unsubscribe(scenarioNodeFinishedToken);
    }
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }
}
