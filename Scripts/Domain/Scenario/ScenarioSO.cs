using UnityEngine;



[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Scenario/ScenarioSO")]

public class ScenarioSO: SORuntimeLoadable
{
    public ScenarioNodeSO startNode;

    public ScenarioContext ToContext()
    {
        var context = new ScenarioContext();
        context.id = id;

        return context;
    }
}
