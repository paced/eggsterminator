using UnityEngine;

/**
 * Controls the behaviour of the Sun
 */
public class Sun : MonoBehaviour {
	// Movement speed of sun
	public float speed = 10;
	// Pause sun
	private bool sunMoveEnabled = true;
	// Radius of orbit
	public float radius = 1000f;
	// Sun colour
	public Vector4 sunColor = Color.white;

	// Centre of orbit
	private Vector3 origin;

	private void Start() {
		// Set centre of rotation to middle of terrain, place sun above origin
		origin = Vector3.zero;
		this.transform.position = origin + radius * Vector3.up;
	}
	// Update is called once per frame
	void Update() {
		// Orbit Sun around the centre of orbit
		if (sunMoveEnabled) {
			transform.RotateAround(origin, Vector3.right, Time.deltaTime * speed);
		}
	}
}
