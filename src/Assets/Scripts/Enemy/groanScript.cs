using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groanScript : MonoBehaviour {
	public AudioSource audiosource;
	public AudioClip deathsound;
	private bool isCoroutineExecuting;

	void Start() {
		audiosource = this.gameObject.GetComponent<AudioSource>();
		deathsound = Resources.Load<AudioClip>("Sounds/death-0" + UnityEngine.Random.Range(0, 5));
		audiosource.PlayOneShot(deathsound);
		isCoroutineExecuting = false;

		StartCoroutine(toDie(2000));
	}

	public IEnumerator toDie(int milliseconds) {
		if (isCoroutineExecuting) {
			yield break;
		}

		isCoroutineExecuting = true;

		yield return new WaitForSeconds(1.0f * milliseconds / 1000);

		Destroy(this.gameObject);

		isCoroutineExecuting = false;
	}
}
