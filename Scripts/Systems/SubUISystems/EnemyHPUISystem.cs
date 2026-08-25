using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPUISystem : MonoBehaviour
{
	static EnemyHPUISystem instance;
	public static EnemyHPUISystem Instance { get => instance; }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		panel.SetActive(false);
	}
	[SerializeField]
	GameObject panel;
	[SerializeField]
	TextMeshProUGUI nameText;
	[SerializeField]
	Slider hpbar;
	[SerializeField]
	TextMeshProUGUI hpText;
	StringBuilder sb = new StringBuilder();

	int lastEnemyHash;
	public void OnEnemyHit(int hash, int hp, int mhp, string _name)
	{
		lastEnemyHash = hash;
		panel.SetActive(true);
		hpbar.value = (hp * 1.0f) / mhp;
		sb.Clear();
		hpText.text = sb.Append(hp).Append('/').Append(mhp).ToString();
		nameText.text = _name;
	}
	public void OnEnemyDied(int hash)
	{
		if(lastEnemyHash == hash)
			panel.SetActive(false);
	}
}
