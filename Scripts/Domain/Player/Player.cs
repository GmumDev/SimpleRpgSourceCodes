using System;
using System.Threading.Tasks;
using UnityEngine;


[RequireComponent(typeof(Animator), typeof(CharacterController), typeof(PlayerCharacterParty))]
public class Player : MonoBehaviour, IDamageTakeable
{
	static Player instance;

	[Header("Components")]
	public Transform cameraTarget;

	[Header("Character Data")]
	public Transform playerBody { get => party.CurCharacter.body; }
	public Transform fwdCamTarget { get => party.CurCharacter.FWDCamTarget; }
	public AttackSkill[] AttackSkill { get => party.CurCharacter.skills; }

	[Header("Player Data")]
	[SerializeField] string playerSOKey;
	PlayerSO playerSO;

	[Header("Ochestration")]
	PlayerGrowSystem growSystem;
	PlayerLifeCycle lifeCycle;
	PlayerCharacterParty party;

	public PlayerStateMachine FSM { get; private set; }
	public static void ForceWatchState() { instance.FSM.ChangeState(instance.FSM.watchState); }

	[Header("Components Caching")]
	public CharacterController controller { get; private set; }
	public Animator animator { get; private set; }

	// Properties & Simple Get Func
	public static Vector3 Position { get => instance.transform.position; }
	public static PlayerSO PlayerData { get => instance.playerSO; }
	public static int ApplyDamageCalc(int x) => (int)(x * PlayerData.AttackMultiplier);
	public static void GainExp(int amount) => instance.growSystem.GainExp(instance.playerSO, amount);


	// Private Fields
	private async void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		while (AddressableGroupLoader.Instance.IsLoaded == false)
		{
			await Task.Yield();
		}
		playerSO = AddressableGroupLoader.Instance.GetSaveData<PlayerSO>(playerSOKey);
		
		
		growSystem = new PlayerGrowSystem();
		lifeCycle = new PlayerLifeCycle();
		party = GetComponent<PlayerCharacterParty>();
		animator = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();

		party.InitChar(this, 0);

		playerSO.InitOnRevive();

		FSM = new PlayerStateMachine(this);

	}
	private void Start()
	{
		EventBus.Publish(new PlayerCharacterChangedEvent(party.CurCharacter.data));
	}
	private void Update()
	{
		FSM.UpdateState();
	}
	public void AssignSkills()
	{
		for (int i = 0; i < party.CurCharacter.skills.Length; i++)
			if (party.CurCharacter.skills[i] != null)
				party.CurCharacter.skills[i].OnAssigned();
	}
	public void ChangeChar() => party.ChangeChar(this); // change to next char

	void OnHitted() => lifeCycle.Hit(this);
	void OnDead() => lifeCycle.Die(this);
	public void Revive() => lifeCycle.Revive(this); // 참조 있다.... 부활 버튼이 참조한다....

	void IDamageTakeable.TakeDamage(int damage)
	{
		if (playerSO.playerStats.hp <= 0) return;
		var calcDmg = (1 / playerSO.DefenceMultiplier) * damage;
		playerSO.playerStats.hp -= (int)calcDmg;

		EventBus.Publish(new PlayerHittedEvent(playerSO));

		if (playerSO.playerStats.hp <= 0)
			OnDead();
		else
			OnHitted();
	}
	private void OnApplicationQuit()
	{
		SaveDataOverwriter.Instance.SaveToFile(playerSO);
	}

}
