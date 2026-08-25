using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Scenario/NodePlayedEventSO")]
public class ScenarioNodePlayedEventSO : ScriptableObject
{
    public ScenarioNodePlayedEventType eventType;

    public ScenarioNodePlayedEvent ToEvent()
    {
        var obj = new ScenarioNodePlayedEvent(
            eventType
        );

        return obj;
    }
}
