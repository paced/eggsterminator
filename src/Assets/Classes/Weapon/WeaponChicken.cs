using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChicken : Weapon {
	/* Warning: can be overridden in the Unity Scene Editor. */
	public GameObject projectile;
	public float projectileSpawnOffset = 1f;
	public float projectileSpeed = 2800.0f;

	private GameObject newEgg;

	public WeaponChicken() : base(1, "Chicken", 240, 24, 1, 1, 380, 1200, true, "eggshot") {
		projectile = (GameObject)Resources.Load("Prefabs/Egg", typeof(GameObject));
	}

	public override void applyLevel1Upgrade() {
		this.IsSemiAuto = false;
		this.Rate = 180;
	}

	public override void applyLevel2Upgrade() {
		this.Rate = (int)(this.Rate / 1.5);
		this.Deviation = this.Deviation / 2;
	}

	public override void applyLevel3Upgrade() {
		this.Rate = (int)(this.Rate / 1.5);
		this.ReloadTime = this.ReloadTime / 2;
	}

	public override void attack() {
		/* Throw an egg at the mouse position. */
		throwProjectile(projectile, projectileSpawnOffset, projectileSpeed);
	}
}