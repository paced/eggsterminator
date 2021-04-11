using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class crosshairHider : MonoBehaviour {
	void Update() {
		if (Input.GetButton("Fire2")) {
			this.gameObject.GetComponent<Image>().enabled = false;
		} else {
			this.gameObject.GetComponent<Image>().enabled = true;
		}
	}
}
