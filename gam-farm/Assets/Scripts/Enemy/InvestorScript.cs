using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;

public class InvestorScript : MonoBehaviour {

	private GameObject player;
	public float accelerationForce = 25f;
	public float dampingFactor = 0.5f;
	public float speed = 1f;
	public float distToGround = 0.5f;

	Rigidbody rb;

	/* Health and render information. */

	public Stat hitpoints;

	/* Attack information. Set these in the inspector. */

	public float hurtDistance = 1f;
	public float houseHurtDistance = 15f;
	public int damagePerHit = 3;
	public int hitRate = 500;
	public GameObject[] targets;
	public GameObject chaser;

	private bool isCoroutineExecuting;

	/* Jump information. */
	Ray ray;

	void Start() {
		isCoroutineExecuting = false;
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody>();

		hitpoints = new Stat("Enemy Health", 120, 120);

		/* Set damage targets. */

		targets = new GameObject[2];
		targets[0] = player;
		targets[1] = GameObject.FindGameObjectWithTag("Building");

		/* Set type of enemy. */

		if (UnityEngine.Random.Range(0f, 1f) < 0.3f) {
			chaser = targets[0];
		} else {
			chaser = targets[1];
		}

		/* Set jump ray to down. */

		ray = new Ray(transform.position, -Vector3.up);

		/* Add upright constraint. */

		rb.constraints = (RigidbodyConstraints)80; //16 is freeze x rot, 64 is freeze z rot
	}

	void Awake() {
		/* Tick the attack clock. */
		StartCoroutine(toAttack(hitRate));
	}

	void Update() {
		if (LevelController.CurrentLevelState == LevelController.LevelState.PAUSED)
			return;

		if (transform.position.y <= 2.1f) {
			Vector3 diff = chaser.transform.position - transform.position;
			rb.velocity = Vector3.Normalize(new Vector3(diff.x, 0f, diff.z)) * speed;
		}

		/* Kill object if the health is too low. */
		if (hitpoints.Val <= 0) {
			Die();
		}

		/* Kill object if it falls off screen. */
		if (rb.position.y < -10)
			Die(); //magic
	}

	private void Die() {
		GameObject groanObject = (GameObject)Resources.Load("Prefabs/Groan");
		GameObject eggsplosionObject = (GameObject)Resources.Load("Prefabs/Eggsplosion");
		GameObject groan = GameObject.Instantiate<GameObject>(groanObject);
		GameObject eggsplosion = GameObject.Instantiate<GameObject>(eggsplosionObject);
		groan.transform.position = transform.position;
		eggsplosion.transform.position = transform.position;

		Destroy(this.gameObject);
		KillEvent(this, null);
	}

	private void Attack() {
		/* Does a set amount of damage per invocation to nearby targets. */
		foreach (GameObject each in targets) {
			/* House contact. */
			if (each.tag == "Player") {
				if (Vector3.Distance(this.transform.position, each.transform.position) < hurtDistance) {
					player.GetComponent<playerHealth>().hitpoints.modVal(-damagePerHit);
				}
			} else {
				if (Vector3.Distance(this.transform.position, each.transform.position) < houseHurtDistance) {
					player.GetComponent<playerHealth>().hitpoints.modVal(-damagePerHit);

					player.GetComponent<weaponScript>().importantText.text = "House is under attack!";
					StartCoroutine(player.GetComponent<weaponScript>().importantFadeout(500));
				}
			}
		}
	}

	public IEnumerator toAttack(int milliseconds) {
		if (isCoroutineExecuting) {
			yield break;
		}

		isCoroutineExecuting = true;

		yield return new WaitForSeconds(1.0f * milliseconds / 1000);

		this.Attack();

		/* Jump randomly if on ground. */

		if (Physics.Raycast(ray, distToGround) &&
		    UnityEngine.Random.Range(0.0f, 1.0f) < 0.2f) {
			rb.AddForce(new Vector3(0.0f, 40f, 0.0f), ForceMode.Impulse);
		}
			
		StartCoroutine(toAttack(hitRate));

		isCoroutineExecuting = false;
	}

	public event EventHandler KillEvent;
}
