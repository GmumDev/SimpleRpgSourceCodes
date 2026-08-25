using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
	public static Vector3 lastPlayerSpawnedPoint;
	[SerializeField]
	bool spawnOnStart;
	private void Start()
	{
		var obj = GameObject.FindGameObjectWithTag("Player");
		if (spawnOnStart)
			Spawn(obj);
	}
	public void Spawn(GameObject player)
    {
		player.GetComponent<CharacterController>().enabled = false;
		player.transform.position = transform.position;
		player.GetComponent<CharacterController>().enabled = true;
		var target = player.GetComponent<Player>().cameraTarget;
		CameraManager.Instance.FollowTarget(target);
		PlayerSpawner.lastPlayerSpawnedPoint = transform.position;
	}

}
