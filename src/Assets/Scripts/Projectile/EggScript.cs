using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour {
	/* Note that these values can be overridden by the Unity Scene editor. */
	public int attackPower = 40;
	public int splashPower = 30;
	public float explosionRadius = 4f;
	public float explosionForce = 50f;
	public float preExplosionForce = 50f;
	public Vector3 preExplosionYOffset = Vector3.down;
	public GameObject explosionPartSys;
	public int timeToLive = 2000;
	public int armTime = 100;

	private bool isCoroutineExecuting;
	private bool armFlag;
	private bool hasHit;

	public IEnumerator count(int milliseconds) {
		/* Create a clock for n milliseconds to reset shoot status. */
		if (isCoroutineExecuting) {
			yield break;
		}

		isCoroutineExecuting = true;

		yield return new WaitForSeconds(1.0f * milliseconds / 1000);

		/* Destroy self, resulting in explosion. */
		armFlag = true;
		explode();

		Destroy(this.gameObject);

		isCoroutineExecuting = false;
	}

	public IEnumerator arm(int milliseconds) {
		/* Create a clock for n milliseconds to reset shoot status. */
		if (isCoroutineExecuting) {
			yield break;
		}

		isCoroutineExecuting = true;

		yield return new WaitForSeconds(1.0f * milliseconds / 1000);

		/* Arm the "grenade." */
		this.armFlag = true;

		isCoroutineExecuting = false;
	}

	void Start() {
		/* Ensure eggs die after some time. */
		isCoroutineExecuting = false;
		StartCoroutine(count(timeToLive));

		/* Ensure eggs can only explode after some time. */
		armFlag = false;
		StartCoroutine(arm(armTime));

		/* If an egg has hit an enemy, only the explosion should do damage. */
		hasHit = false;
	}

	void explode() {
		Vector3 explosionPos = transform.position;
		Collider[] cols = Physics.OverlapSphere(explosionPos, explosionRadius);

		foreach (Collider hit in cols) {
			Rigidbody rb = hit.GetComponent<Rigidbody>();
			GameObject other = hit.gameObject;

			if (rb != null && armFlag) {
				/* Only explode if we have reached that point in the counter. */

				rb.constraints = RigidbodyConstraints.None;
				rb.AddExplosionForce(preExplosionForce, explosionPos + preExplosionYOffset, explosionRadius);
				rb.AddExplosionForce(explosionForce, explosionPos + preExplosionYOffset, explosionRadius);

				if (other.tag == "Investor") {
					other.GetComponent<InvestorScript>().hitpoints.modVal(-splashPower);
				}
			}
		}

		if (armFlag) {
			GameObject exp = Instantiate<GameObject>(explosionPartSys);
			exp.transform.position = transform.position;
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		GameObject go = other.gameObject;

		if (go.tag == "Investor" && !hasHit) {
			/* If the eggplant hits the enemy base object, that object should take damage. */
			go.GetComponent<InvestorScript>().hitpoints.modVal(-attackPower);
			hasHit = true;
		}

		this.explode();
	}

}
