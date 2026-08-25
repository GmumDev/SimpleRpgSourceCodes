using UnityEngine;

public class ProjectileChaseAddon : ProjectileAddon
{
	Transform target;
	public ProjectileChaseAddon(ProjectileByPool projectile, Transform target) : base(projectile)
	{
		this.target = target;
	}

	public override void Update()
	{
		projectile.dir = (target.position - projectile.transform.position).normalized;
	}

}
