using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncSceneManager : MonoBehaviour
{
    private static AsyncSceneManager instance;
    public static AsyncSceneManager Instance { get => instance; }
	private static GameObject canvasInstance;
	[SerializeField]
	GameObject canvas;
	[SerializeField]
	FadeInoutPanel panel;
	public bool IsLoading { get => panel.IsActive; }
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
		if(canvasInstance == null)
		{
			canvasInstance = canvas;
			DontDestroyOnLoad(canvas);
		}
		else
		{
			Destroy(canvas);
		}
	}
	public async void LoadScene(string scenename)
    {
        Debug.Log("[AsyncSceneManager] IsActive~ " + panel.IsActive);
        while (IsLoading)
            await Awaitable.NextFrameAsync();

        Debug.Log("[AsyncSceneManager] Fadein~ ");
        await panel.Fadein();
        Debug.Log("[AsyncSceneManager] LoadSceneAsync~ " + scenename);
        await SceneManager.LoadSceneAsync(scenename);
        Debug.Log("[AsyncSceneManager] Fadeout~ ");
        await panel.Fadeout();
        Debug.Log("[AsyncSceneManager] IsActive after fadeout: " + panel.IsActive);

    }
	public void QuitGame()
	{
		Application.Quit();
	}
	public static bool IsDontDestroyOnLoad(GameObject obj)
	{
		return obj.scene.buildIndex == -1;
	}
}
