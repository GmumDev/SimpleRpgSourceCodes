using UnityEngine;

public abstract class ProjectileAddon
{
	protected ProjectileByPool projectile;
	public ProjectileAddon(ProjectileByPool projectile)
	{
		this.projectile = projectile;
	}
	public abstract void Update();
}
