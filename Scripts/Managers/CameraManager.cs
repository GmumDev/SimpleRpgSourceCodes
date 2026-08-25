using Unity.Cinemachine;
using UnityEngine;

public class CameraManager: MonoBehaviour
{
    private static CameraManager instance;
    public static CameraManager Instance { get => instance; }


    [SerializeField]
    Camera builtinCamera;

    [SerializeField]
    CinemachineCamera playerFollowCamera;

    [SerializeField]
    CinemachineCamera focusCamera;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(focusCamera.gameObject);
            DontDestroyOnLoad(playerFollowCamera.gameObject);
            DontDestroyOnLoad(builtinCamera.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
			Destroy(focusCamera.gameObject);
			Destroy(playerFollowCamera.gameObject);
			Destroy(builtinCamera.gameObject);
		}
    }
    private void Start()
    {
        playerFollowCamera.Prioritize();
    }
	public void FocusTalker(Transform target)
	{
		focusCamera.Follow = target;
		focusCamera.Prioritize();
    }
    public void TogglePlayerCulling()
    {
        builtinCamera.cullingMask ^= 1 << LayerMask.NameToLayer("PlayerMesh");
	}
	public void FollowTarget(Transform target)
	{
		playerFollowCamera.Follow = target;
		focusCamera.Follow = target;
		playerFollowCamera.Prioritize();
	}
	public void PrioritizeFollowCam()
    {
        playerFollowCamera.Prioritize();

	}
	public void PrioritizeFocusCam()
	{
		focusCamera.Prioritize();
	}
}
