using UnityEngine;
using System.Collections;

public class eggSoundScript : MonoBehaviour {

	public AudioSource audioSource;

	void Start() {
		audioSource = this.gameObject.GetComponent<AudioSource>();
		audioSource.pitch = Random.Range(0.5f, 1.5f);
		audioSource.Play();
	}
}

