using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestorSpawnerScript : MonoBehaviour {

	public GameObject investor;

	public void Spawn(int spawnAmount) {
		for (int _ = 0; _ < spawnAmount; _++) {
			GameObject inv = Instantiate<GameObject>(investor);
			inv.GetComponent<InvestorScript>().KillEvent += EnemyKilled;
			Vector3 scatter = new Vector3(UnityEngine.Random.Range(-10f, 0f), 0, UnityEngine.Random.Range(-10f, 0f));
			inv.transform.position = transform.position + scatter;
		}

		/* Each time this spawn function is called, there's a chance it will update the spawn point. */
		if (UnityEngine.Random.Range(0.0f, 2.0f) < 0.05f) {
			transform.position = new Vector3(0.0f, 3.0f, 50);
		} else if (UnityEngine.Random.Range(0.0f, 2.0f) < 0.05f) {
			transform.position = new Vector3(0.0f, 3.0f, -50);
		} else if (UnityEngine.Random.Range(0.0f, 2.0f) < 0.05f) {
			transform.position = new Vector3(25.0f, 3.0f, -20f);
		} else if (UnityEngine.Random.Range(0.0f, 2.0f) < 0.05f) {
			transform.position = new Vector3(25.0f, 3.0f, 28f);
		} else if (UnityEngine.Random.Range(0.0f, 2.0f) < 0.05f) {
			transform.position = new Vector3(-25.0f, 3.0f, -20f);
		} else if (UnityEngine.Random.Range(0.0f, 2.0f) < 0.05f) {
			transform.position = new Vector3(-25.0f, 3.0f, 28f);
		}
	}

	public void EnemyKilled(object sender, EventArgs e) {
		FindObjectOfType<LevelController>().EnemyKilled();
	}

}
