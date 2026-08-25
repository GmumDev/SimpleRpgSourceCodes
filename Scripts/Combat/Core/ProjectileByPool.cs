using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileByPool : MonoBehaviour
{
	public Action<ProjectileByPool> ReleaseAction;
	public List<ProjectileAddon> addons;
	public Vector3 origin;
	public Vector3 dir;
	public float speed;
	public int targetLayerMask;
	public int damage;

	float spawnedT;
	bool released;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public void Init(
		Action<ProjectileByPool> ReleaseAction,
		Vector3 pos,
		Vector3 dir,
		float speed,
		int damage,
		int targetLayerMask)
	{
		addons = new List<ProjectileAddon>();

		this.ReleaseAction = ReleaseAction;
		this.origin = pos;
		this.dir = dir;
		this.speed = speed;
		this.damage = damage;
		this.targetLayerMask = targetLayerMask;

		spawnedT = Time.time;
		transform.position = origin;
		released = false;
	}
	public void AddAddon(ProjectileAddon addon)
	{
		addons.Add(addon);
	}
    // Update is called once per frame
    void Update()
    {
		foreach (var addon in addons)
			addon.Update();

		transform.position += speed * dir * Time.deltaTime;
		if (Time.time - spawnedT > 10f)
			DestroyThis();
	}
	public void DestroyThis()
	{
		ReleaseAction(this);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy") && released == false)
		{
			other.gameObject.GetComponent<IDamageTakeable>()
				.TakeDamage(Player.ApplyDamageCalc(damage));
			DestroyThis();
			released = true;
		}
	}
}
