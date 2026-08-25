using UnityEngine;

public interface IScenarioManager
{
    bool IsPlaying { get; }
    bool PlayScenario(string scenarioId);
    void NextNode();

    void SelectChoices(Vector2 navInput);
}
