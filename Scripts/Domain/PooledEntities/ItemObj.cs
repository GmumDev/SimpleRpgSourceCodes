using System;
using UnityEngine;

public class ItemObj : MonoBehaviour
{
	[SerializeField]
	SpriteRenderer img;

	Action<ItemObj> OnRelease;
    string itemId;
	int cnt;
	float spawnT;
	Vector3 origin;
	const float waveHeight = 0.2f;
	const float waveFreq = 2f;
	public void Init(Action<ItemObj> OnRelease, string id, int cnt)
	{
		this.OnRelease = OnRelease;
		this.itemId = id;
		img.sprite = ItemDataContainer.GetIconWithId(id);
		this.cnt = cnt;
		spawnT = Time.time - UnityEngine.Random.Range(0, 1f);
		transform.position += Vector3.up * waveHeight;
		transform.position += new Vector3(1, 0, 1) * UnityEngine.Random.Range(-.6f, .6f);
		origin = transform.position;
	}
	private void Update()
	{
		transform.Rotate(Vector3.up * 90f * Time.deltaTime);
		transform.position = origin + Vector3.up * Mathf.Sin((Time.time - spawnT) * waveFreq) * waveHeight;
	}
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			InventorySystem.Instance.ObtainItem(itemId, cnt);

			OnRelease(this);
		}
	}
}
