using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyOnHit : MonoBehaviour {
	public const int timeToLive = 1500;
	private bool isCoroutineExecuting;

	public IEnumerator count(int milliseconds) {
		/* Create a clock for n milliseconds to reset shoot status. */

		if (isCoroutineExecuting) {
			yield break;
		}

		isCoroutineExecuting = true;

		yield return new WaitForSeconds(milliseconds / 1000);

		/* Reset shoot status. */
		Destroy(this.gameObject);

		isCoroutineExecuting = false;
	}

	void Start() {
		/* Set a destroy timer for this particle system. */
		StartCoroutine(count(timeToLive));
	}
}
