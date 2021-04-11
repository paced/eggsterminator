using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camPitchScript : MonoBehaviour {

    private MoveScript ms;
    private float pitchSpeed;
    // The amount to lock rotation at
    private static float ROTATION_LOCK = 0.3f;

	// Use this for initialization
	void Start () {
        ms = this.GetComponentInParent<MoveScript>();
        pitchSpeed = ms.rotateSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        // Allow moving pause in inactive or when dialog's active
        if (LevelController.CurrentLevelState == LevelController.LevelState.PAUSED && !LevelController.DialogActive) return;

        // The amount to rotate by
        float rotateXAmount = -pitchSpeed * Input.GetAxis("Mouse Y");

        // Lock rotation
        if (rotateXAmount < 0 && transform.localRotation.x < -ROTATION_LOCK) {
            return;
        }
        else if (rotateXAmount > 0 && transform.localRotation.x > ROTATION_LOCK) {
            return;
        }

        // Perform rotation
        transform.Rotate(rotateXAmount, 0, 0);
    }
}
