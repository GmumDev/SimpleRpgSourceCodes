using System;
using TMPro;
using UnityEngine;

public class DamageSkin : MonoBehaviour
{
	Action<DamageSkin> OnRelease;
	[SerializeField]
	TextMeshProUGUI damageText;

	float spawnT;
	Vector3 origin;
	public void Init(Action<DamageSkin> OnRelease, int damage)
	{
		this.OnRelease = OnRelease;
		this.damageText.text = damage.ToString();
		spawnT = Time.time;
		origin = transform.position + Vector3.up * 1f;
	}

	private void Update()
	{
		transform.rotation = Quaternion.LookRotation(transform.position - Player.Position);
		transform.position = origin + Vector3.up * EaseOut(Time.time - spawnT);
		damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, EaseIn(1 - (Time.time - spawnT)));
		if(Time.time - spawnT > 1f)
		{
			OnRelease(this);
		}
	}
	float EaseOut(float t) => 1 - ((1 - t) * (1 - t));
	float EaseIn(float t) => t*t;
}
