using UnityEngine;

public class GlobalCanvasSystem_WorldSpace : MonoBehaviour
{
	static GlobalCanvasSystem_WorldSpace instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

}
