using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "TimelineSO", menuName = "Scriptable Objects/TimelineSO")]
public class TimelineSO : SORuntimeLoadable
{
    public PlayableAsset playable;
}
