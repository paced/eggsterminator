using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggShakeScript : MonoBehaviour {
	public float maxDistance = 200f;
	public float shakeTime = 0.3f;

	void Start() {
		float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
		float duration = shakeTime - shakeTime * distance / maxDistance;

		if (duration < maxDistance) {
			Camera.main.gameObject.GetComponent<screenShake>().shakeDuration = duration;
			Camera.main.gameObject.GetComponent<screenShake>().shakeAmount = duration;
		}
	}
}
