using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour {

	public float speed = 2f;
	public float rotateSpeed = 1;
	private CharacterController cc;

	// Use this for initialization
	void Start() {
		// get character controller
		cc = GetComponent<CharacterController>();
		cc.detectCollisions = true;
        
	}
	
	// Update is called once per frame
	void Update() {
		if (LevelController.CurrentLevelState == LevelController.LevelState.PAUSED && !LevelController.DialogActive)
			return;

		if (!cc.isGrounded) {
			/* Apply gravity. */
			cc.Move(transform.up * -9.81f * Time.deltaTime);
		}

		// move forward, back, left, right respectively
		if (Input.GetKey(KeyCode.W)) {
			cc.Move(transform.forward * speed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.S)) {
			cc.Move(transform.forward * -1 * speed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.A)) {
			cc.Move(transform.right * -1 * speed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.D)) {
			cc.Move(transform.right * speed * Time.deltaTime);
		}

		// rotate to look left/right. both axis rotation would be quite involved
		transform.Rotate(0, rotateSpeed * Input.GetAxis("Mouse X"), 0);
	}
}
