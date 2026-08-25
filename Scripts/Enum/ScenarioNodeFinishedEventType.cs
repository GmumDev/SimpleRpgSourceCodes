using System;

[Flags]
public enum ScenarioNodeFinishedEventType
{
    Nothing = 0,
    FollowUpQuest = 1,
    FollowUpScenario = 1 << 1,
    CompleteQuest = 1 << 2,
    LoadScene = 1 << 3,
    DoTimeline = 1 << 4,
}
