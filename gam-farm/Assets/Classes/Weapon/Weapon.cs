using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Weapon {
	/* weaponScript is a little hack to access StartCoroutine. */
	private weaponScript clock;
	private bool isCoroutineExecuting;
	private bool isCoroutineExecutingReload;
	private bool isCoroutineExecutingRegen;
	private GameObject newEgg;

	public GameObject player;

	private int weaponId;

	public int WeaponID {
		get { return weaponId; }
		set { weaponId = value; }
	}

	private string name;

	public string Name {
		get { return name; }
		set { name = value; }
	}

	private int ammo;
	private int clipsize;

	public int Ammo {
		/* Current ammo in magazine. */

		get { return ammo; }
		set { ammo = value; }
	}

	private int maxAmmo;

	public int MaxAmmo {
		/* Size of the reserve ammo. */

		get { return maxAmmo; }
		set { maxAmmo = value; }
	}

	private int burst;

	public int Burst {
		/* Ammo used per shot. */

		get { return burst; }
		set { burst = value; }
	}

	private int rate;

	public int Rate {
		/* Fire rate allowed. */

		get { return rate; }
		set { rate = value; }
	}

	private float deviation;

	public float Deviation {
		/* Fire rate allowed. */

		get { return deviation; }
		set { deviation = value; }
	}

	private int reloadTime;

	public int ReloadTime {
		/* Fire rate allowed. */

		get { return reloadTime; }
		set { reloadTime = value; }
	}

	private bool reloading;

	public bool Reloading {
		get { return reloading; }
		set { reloading = value; }
	}

	private bool isSemiAuto;

	public bool IsSemiAuto {
		/* Is this weapon semiautomatic/single fire? */

		get { return isSemiAuto; }
		set { isSemiAuto = value; }
	}

	public AudioSource shootSource;
	public AudioClip shootSound;
	public bool canShoot;
	private Vector3 randSpawn;
	public AudioClip reloadingSound;

	public Weapon(int weaponID, string name, int maxAmmo, int initAmmo, int burst, 
	              int rate, float deviation, int reloadTime, bool semi, string soundString) {
		/* Instantiate this abstract class. */
		this.weaponId = weaponID;
		this.name = name;
		this.maxAmmo = maxAmmo;
		this.ammo = initAmmo;
		this.clipsize = this.ammo;
		this.burst = burst;
		this.rate = rate;
		this.deviation = deviation;
		this.reloadTime = reloadTime;
		this.isSemiAuto = semi;
		this.reloading = false;
		this.reloadingSound = Resources.Load<AudioClip>("Sounds/reload");

		/* Internals. */

		this.canShoot = true;
		this.player = GameObject.Find("Player");
		this.clock = this.player.GetComponent<weaponScript>();
		this.isCoroutineExecuting = false;
		this.isCoroutineExecutingReload = false;

		/* Sound stuff. */

		this.shootSource = player.GetComponents<AudioSource>()[1];
		this.shootSound = Resources.Load<AudioClip>("Sounds/" + soundString);

		if (this.shootSound == null) {
			Debug.Log("sound not found");
		}

		/* Upgrade stuff. */

		applyUpgrade(GameController.gameProgress.GetWeaponLevel(weaponID));

		/* Start clock for regen. */
		clock.StartCoroutine(countRegen(this.reloadTime * 2));
		isCoroutineExecutingRegen = false;
	}

	public void applyUpgrade(int weaponLevel) {
		/* Upgrade a weapon. Do nothing if max level. */
		if (weaponLevel >= 1)
			applyLevel1Upgrade();
		if (weaponLevel >= 2)
			applyLevel2Upgrade();
		if (weaponLevel == 3)
			applyLevel3Upgrade();
	}

	public abstract void applyLevel1Upgrade();

	public abstract void applyLevel2Upgrade();

	public abstract void applyLevel3Upgrade();

	public abstract void attack();

	public void throwProjectile(GameObject projectile, float projectileSpawnOffset, float projectileSpeed) {
		/* Provide a throwable that allows projectiles to exist. */
		newEgg = GameObject.Instantiate<GameObject>(projectile);

		if (this.burst > 1) {
			randSpawn = new Vector3(
				Random.Range(-0.8f, 0.8f) / 2,
				Random.Range(-0.8f, 0.8f) / 2,
				Random.Range(-0.8f, 0.8f) / 2
			);
		} else {
			randSpawn = Vector3.zero;
		}

		/* Set object position. Add a bit to the side if the RMB is down. */
		if (Input.GetButton("Fire2")) {
			newEgg.transform.position = player.transform.position + projectileSpawnOffset *
			player.transform.TransformDirection(Vector3.forward + Vector3.up * 0.4f + randSpawn) + new Vector3(0, 1.15f, 0);
		} else {
			newEgg.transform.position = player.transform.position + projectileSpawnOffset *
			player.transform.TransformDirection(Vector3.forward + 0.8f * Vector3.left + Vector3.up * 0.2f +
			randSpawn) + new Vector3(0, 1.3f, 0);
		}

		/* Set the rotation of the new projectile. */
		newEgg.transform.rotation = player.transform.rotation;

		/* The new projectile is a rigid body. */
		Rigidbody eggRB = newEgg.GetComponent<Rigidbody>();

		/* Add force to the projectile based on the position of the mouse. */
		Vector3 torque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 10000f;
		Vector3 fwd = Camera.main.transform.forward;
		Vector3 up = Camera.main.transform.up;

		/* Add random spread if not ADS. */
		Vector3 randSpread;
		if (Input.GetButton("Fire2")) {
			randSpread = new Vector3(
				Random.Range(-this.deviation, this.deviation) / 6,
				Random.Range(-this.deviation, this.deviation) / 3, 
				Random.Range(-this.deviation, this.deviation) / 3
			);
		} else {
			randSpread = new Vector3(
				Random.Range(-this.deviation, this.deviation),
				Random.Range(-this.deviation, this.deviation), 
				Random.Range(-this.deviation, this.deviation)
			);
		}

		/* If player is moving, the velocity will be added to egg */
		if (Input.GetKey(KeyCode.A)) {
			eggRB.velocity += Camera.main.transform.right * -5f;
		}
		if (Input.GetKey(KeyCode.D)) {
			eggRB.velocity += Camera.main.transform.right * 5f;
		}
		if (Input.GetKey(KeyCode.W)) {
			eggRB.velocity += fwd * 5f;
		}
		if (Input.GetKey(KeyCode.D)) {
			eggRB.velocity += fwd * -5f;
		}

		/* Now push the object and add torque based on calculated values and add curvature to each throw. 
		 * If the RMB is down, don't add curvature. */
		if (Input.GetButton("Fire2")) {
			eggRB.AddForce(fwd * projectileSpeed + up * 0.1f * projectileSpeed);
		} else {
			eggRB.AddForce(fwd * projectileSpeed + up * 0.05f * projectileSpeed);
		}

		eggRB.AddRelativeForce(randSpread);
		eggRB.AddRelativeTorque(torque);
	}

	public void shoot() {
		/* Fire once from this weapon. */

		if (canShoot && this.ammo > 0 && !this.reloading) {
			this.ammo--;
			this.canShoot = false;

			for (int i = 0; i < this.burst; i++) {
				attack();
			}

			shootSource.PlayOneShot(shootSound, 0.3f);
		
			clock.StartCoroutine(count(this.rate));
		}
	}

	public void reload() {
		/* Reload the weapon. */
		if (this.maxAmmo > 0 && this.ammo < this.clipsize) {
			this.reloading = true;
			this.canShoot = false;
			this.shootSource.PlayOneShot(reloadingSound, 1.0f);

			isCoroutineExecutingReload = false;
			clock.StartCoroutine(countReload(this.reloadTime));
		}
	}

	public IEnumerator count(int milliseconds) {
		/* Create a clock for n milliseconds to reset shoot status. */

		if (isCoroutineExecuting) {
			yield break;
		}

		isCoroutineExecuting = true;

		yield return new WaitForSeconds(1.0f * milliseconds / 1000);

		/* Reset shoot status. */
		this.canShoot = true;

		isCoroutineExecuting = false;
	}

	public IEnumerator countReload(int milliseconds) {
		/* Create a clock for n milliseconds to reset shoot status. */

		if (isCoroutineExecutingReload) {
			yield break;
		}

		isCoroutineExecutingReload = true;

		yield return new WaitForSeconds(1.0f * milliseconds / 1000);

		/* Reset shoot status. */
		this.reloading = false;
		this.canShoot = true;
		int transfer = (int)Mathf.Min(this.maxAmmo, this.clipsize - this.ammo);
		this.ammo += transfer;
		this.maxAmmo -= transfer;
		this.shootSource.Stop();

		isCoroutineExecutingReload = false;
	}

	public IEnumerator countRegen(int milliseconds) {
		/* Create a clock for n milliseconds to reset shoot status. */

		if (isCoroutineExecutingRegen) {
			yield break;
		}

		isCoroutineExecutingRegen = true;

		yield return new WaitForSeconds(1.0f * milliseconds / 1000);

		/* Reset shoot status. */
		this.maxAmmo += 1;
		clock.StartCoroutine(countRegen(this.reloadTime / 2));

		isCoroutineExecutingRegen = false;
	}
}
