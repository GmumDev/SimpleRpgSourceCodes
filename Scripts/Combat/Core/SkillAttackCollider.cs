using System;
using UnityEngine;

public class SkillAttackCollider : MonoBehaviour
{
	[SerializeField]
	int damage;
	[SerializeField]
	bool isPlayer;

	int targetLayer;
	public int Damage
	{
		get => damage;
		set => damage = value;
	}
	BoxCollider col;
	Collider[] hitColliders = new Collider[30];
	private void Awake()
	{
		col = GetComponent<BoxCollider>();
		col.enabled = false;
	}
	public void Trigger(int layer, Action<Vector3> OnHittedCallback = null)
	{
		targetLayer = layer;

		Vector3 halfExtents = Vector3.Scale(col.size, transform.lossyScale) * 0.5f;
		Vector3 worldCenter = transform.TransformPoint(col.center);
		int n = Physics.OverlapBoxNonAlloc(worldCenter, halfExtents, hitColliders, transform.rotation, targetLayer);
		for(int i = 0; i < n; i++)
		{
			if (hitColliders[i].TryGetComponent<IDamageTakeable>(out var dt))
			{
				if (OnHittedCallback != null)
					OnHittedCallback(hitColliders[i].gameObject.transform.position);
				dt.TakeDamage(isPlayer ? Player.ApplyDamageCalc(damage) : damage);
			}
		}
	}
}
