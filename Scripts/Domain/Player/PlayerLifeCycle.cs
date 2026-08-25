
public class PlayerLifeCycle
{
	public void Hit(Player player)
	{
		player.animator.SetTrigger("OnHit");
		player.FSM.WaitForAnim(fromTag: "Hit", toTag: "Any", nextState: player.FSM.battleState);
	}
	public void Die(Player player)
	{
		player.animator.SetTrigger("OnDie");
		player.FSM.WaitForAnim(fromTag: "Revive", toTag: "Normal", nextState: player.FSM.normalState);

		EventBus.Publish(new PlayerDeadEvent());
	}
	// 참조 있다.... 부활 버튼이 참조한다....
	public void Revive(Player player)
	{
		player.controller.enabled = false;
		player.transform.position = PlayerSpawner.lastPlayerSpawnedPoint;
		player.controller.enabled = true;
		Player.PlayerData.InitOnRevive();
		player.animator.SetTrigger("OnRevive");

		EventBus.Publish(new PlayerRevivedEvent());
	}
}
