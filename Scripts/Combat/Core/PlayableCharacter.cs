using UnityEngine;

public class PlayableCharacter : MonoBehaviour
{
	[SerializeField]
	public Transform body;
	[SerializeField]
	public Transform FWDCamTarget;
	[SerializeField]
	public AnimatorOverrideController anim;
	[SerializeField]
	public Avatar avatar;
	[SerializeField]
	public AttackSkill[] skills;
	[SerializeField]
	public PlayableCharacterSO data;
}
