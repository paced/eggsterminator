using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShotgun : Weapon {
	/* Warning: can be overridden in the Unity Scene Editor. */
	public GameObject projectile;
	public float projectileSpawnOffset = 1.0f;
	public float projectileSpeed = 2500.0f;

	private GameObject newEgg;

	public WeaponShotgun() : base(2, "Carton Shotgun", 32, 4, 4, 700, 900, 2000, true, "shotgun") {
		projectile = (GameObject)Resources.Load("Prefabs/Egg", typeof(GameObject));
	}

	public override void applyLevel1Upgrade() {
		this.Burst += 3;
		this.Deviation /= 2;
	}

	public override void applyLevel2Upgrade() {
		this.Burst += 2;
		this.ReloadTime /= 2;
	}

	public override void applyLevel3Upgrade() {
		this.Burst += 1;
		this.IsSemiAuto = false;
	}

	public override void attack() {
		/* Throw an egg at the mouse position. */
		throwProjectile(projectile, projectileSpawnOffset, projectileSpeed);
	}
}