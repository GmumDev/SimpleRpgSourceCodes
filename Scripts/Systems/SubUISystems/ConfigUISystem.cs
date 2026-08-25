using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConfigUISystem : MonoBehaviour
{
	static ConfigUISystem instance;
	[SerializeField] GameObject panel;
	
	InputAction escAction = PlayerInputReciever.Instance.ESCAction;

	[SerializeField] GameObject[] contentsByTapIdx;
	int lastTapIdx;
	StringBuilder builder;
	[Header("Player Config Elements")]
	PlayerConfig playerConfig;
	[SerializeField] Slider mouseXSlider;
	[SerializeField] TextMeshProUGUI mouseXText;
	[SerializeField] Slider mouseYSlider;
	[SerializeField] TextMeshProUGUI mouseYText;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}
	private void OnEnable()
	{
		escAction.performed += TogglePanel;
	}
	private void Start()
	{
		playerConfig = Player.PlayerData.playerConfig;
		builder = new StringBuilder();

		// add listeners
		mouseXSlider.onValueChanged.AddListener((x) =>
		{ 
			playerConfig.mouseSenceX = PlayerConfig.MaxMouseSenceX * x;
			mouseXText.text = builder.Clear().Append(playerConfig.mouseSenceX).ToString(0, playerConfig.mouseSenceX < 10 ? 1 : 2);
		});
		mouseYSlider.onValueChanged.AddListener((x) =>
		{
			playerConfig.mouseSenceY = PlayerConfig.MaxMouseSenceY * x;
			mouseYText.text = builder.Clear().Append(playerConfig.mouseSenceY).ToString(0, playerConfig.mouseSenceY < 10 ? 1 : 2);
		});

		// initialize ui elems
		mouseXText.text = builder.Clear().Append(playerConfig.mouseSenceX).ToString(0, playerConfig.mouseSenceX < 10 ? 1 : 2);
		mouseYText.text = builder.Clear().Append(playerConfig.mouseSenceY).ToString(0, playerConfig.mouseSenceY < 10 ? 1 : 2);
	}
	public void SelectTap(int tapIdx)
	{
		contentsByTapIdx[lastTapIdx].SetActive(false);
		contentsByTapIdx[tapIdx].SetActive(true);
		lastTapIdx = tapIdx;
	}
	void TogglePanel(InputAction.CallbackContext obj) => TogglePanel();
	public void TogglePanel()
	{
		if (panel.activeSelf)
			panel.SetActive(false);
		else
			panel.SetActive(true);
	}
	public void QuitGame()
	{
		Application.Quit();
	}

	private void OnDisable()
	{
		if(escAction != null)
			escAction.performed -= TogglePanel;
	}
}
