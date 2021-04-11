using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class weaponScript : MonoBehaviour {
	public Weapon[] arsenal;
	public Weapon weapon;

	Weapon chicken;
	Weapon shotgun;

	private GameObject viewmodel;
	private Vector3 hipfire;
	private Vector3 aimDownChicken;
	private bool isCoroutineExecuting;

	/* Set in Editor. */
	public Text weaponName;
	public Text weaponAmmo;
	public Text importantText;

	string weaponStateString;

	void Start() {
		/* Create the user's complete arsenal. */
		chicken = new WeaponChicken();
		shotgun = new WeaponShotgun();

		/* Assign these references to the arsenal. */
		arsenal = new Weapon[10];
		arsenal[0] = chicken;
		arsenal[1] = shotgun;

		/* Set the current weapon. */
		weapon = arsenal[0];

		/* Set viewmodel. */
		viewmodel = GameObject.Find("chicken-rigged");
		hipfire = new Vector3(-0.65f, -0.52f, 0.54f);
		aimDownChicken = new Vector3(0f, -0.54f, 0.50f);

		isCoroutineExecuting = false;
		importantText.text = "";
	}

	void Update() {
		if (LevelController.CurrentLevelState == LevelController.LevelState.PAUSED)
			return;

		/* Semiautomatic. */
		if (weapon.IsSemiAuto & Input.GetButtonDown("Fire1") && !weapon.Reloading) {
			weapon.shoot();
		} else if (!weapon.IsSemiAuto & Input.GetButton("Fire1") && !weapon.Reloading) {
			weapon.shoot();
		}

		if (!weapon.Reloading && weapon.canShoot && Input.GetButtonDown("Weapon0")) {
			weapon = arsenal[0];

			weapon.canShoot = true;
			weapon.Reloading = false;
		} else if (!weapon.Reloading && weapon.canShoot && Input.GetButtonDown("Weapon1")) {
			weapon = arsenal[1];

			weapon.canShoot = true;
			weapon.Reloading = false;
		}

		if (Input.GetButtonDown("Fire2")) {
			viewmodel.transform.localPosition = aimDownChicken;

			viewmodel.GetComponent<Animator>().Play("Stand Idle", -1, 0f);
			viewmodel.GetComponent<Animator>().speed = 0f;

			Camera.main.fieldOfView = 70f;
		}

		if (Input.GetButtonUp("Fire2")) {
			viewmodel.transform.localPosition = hipfire;

			// viewmodel.GetComponent<Animator>().PlayInFixedTime("Sit Freeze", -1, 0f);
			viewmodel.GetComponent<Animator>().speed = 1f;

			Camera.main.fieldOfView = 90f;
		}

		if (!weapon.Reloading && Input.GetButton("Reload")) {
			/* General reload. */
			weapon.reload();
		}
		
		if (!weapon.Reloading && weapon.canShoot && weapon.Ammo == 0 && Input.GetButton("Fire1")) {
			/* Reload on dry fire if possible. */
			weapon.reload();
		}

		UpdateHUD();
	}

	private void UpdateHUD() {
		if (this.weapon.Reloading) {
			weaponStateString = "Reloading...";
		} else {
			if (weapon.Ammo == 0) {
				if (weapon.MaxAmmo == 0) {
					weaponStateString = "Out of ammo.";
				} else {
					weaponStateString = "Press R to reload.";
				}
			} else {
				weaponStateString = "Clip: " + weapon.Ammo + ", Reserve: " + weapon.MaxAmmo;
			}
		}

		weaponName.text = weapon.Name;
		weaponAmmo.text = weaponStateString;
	}

	public IEnumerator importantFadeout(int milliseconds) {
		/* Create a clock for n milliseconds to reset shoot status. */

		if (isCoroutineExecuting) {
			yield break;
		}

		isCoroutineExecuting = true;

		yield return new WaitForSeconds(1.0f * milliseconds / 1000);

		/* Reset shoot status. */
		importantText.text = "";

		isCoroutineExecuting = false;
	}
}
