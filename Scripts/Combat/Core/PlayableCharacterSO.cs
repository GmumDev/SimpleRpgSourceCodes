using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayableCharacterSO", menuName = "Scriptable Objects/PlayableCharacterSO")]
public class PlayableCharacterSO : ScriptableObject
{
    public string id;
    public string Name;
    public List<Sprite> SkillIcons;
}
