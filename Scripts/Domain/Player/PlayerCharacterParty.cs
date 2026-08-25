using UnityEngine;

public class PlayerCharacterParty : MonoBehaviour
{
	[Header("Characters In Party")]
	[SerializeField] 
	PlayableCharacter[] charactersParty;
	int curCharacterIdx;
	public PlayableCharacter CurCharacter { get => charactersParty[curCharacterIdx]; }


	public void ChangeChar(Player player)
	{
		InitChar(player, (curCharacterIdx + 1) % charactersParty.Length);
		EventBus.Publish(new PlayerCharacterChangedEvent(CurCharacter.data));
	}
	public void InitChar(Player player, int charIdx)
	{
		CurCharacter.gameObject.SetActive(false);
		
		var lastFWDCamTargetRotation = CurCharacter.FWDCamTarget.rotation;
		var lastBodyRotation = CurCharacter.body.rotation;

		curCharacterIdx = charIdx; // change cur char

		CurCharacter.gameObject.SetActive(true);

		// FWDCamTarget 교체 + Rotation유지 = 캐릭터 교체시 공격이 나가는 방향 초기화 방지
		CurCharacter.FWDCamTarget.rotation = lastFWDCamTargetRotation;

		// Body 교체(model parents) + Rotation 유지 = 캐릭터 보는 방향 초기화 방지
		CurCharacter.body.rotation = lastBodyRotation;

		// Animator 관련 교체
		player.animator.runtimeAnimatorController = CurCharacter.anim;
		player.animator.avatar = CurCharacter.avatar;


		player.AssignSkills();
	}
}
