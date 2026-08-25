using UnityEngine;

public class SceneLoadTriggerBox : MonoBehaviour
{
	[SerializeField]
	string sceneName;
	bool isTriggered;
	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && isTriggered == false)
		{
			isTriggered = true;
			AsyncSceneManager.Instance.LoadScene(sceneName);
		}
	}
}
