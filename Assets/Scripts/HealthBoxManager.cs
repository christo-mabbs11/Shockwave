using UnityEngine;
using System.Collections;

public class HealthBoxManager : MonoBehaviour {

	private bool HealthBoxAvailable = true;
	public float HealthBoxtime = 10.0f;
	private float HealthBoxtimer = 0.0f;

	SpriteRenderer SpriteRendererRef;
	Transform ChildTransformRef;
	private AudioSource AudioSourceRef;

	// Sin Wave stuff
	private float amplitudeY = 1.0f;
	private float omegaY = 3.0f;
	private float index;
	float RandomOffset;

	void Awake () {
		SpriteRendererRef = this.gameObject.transform.GetChild (0).GetComponent<SpriteRenderer> ();
		ChildTransformRef = this.gameObject.transform.GetChild (0).GetComponent<Transform> ();

		AudioSourceRef = this.GetComponent<AudioSource> ();

		RandomOffset = (Random.Range(0, 100) * 1.0f)/100.0f;
	}

	void Update () {

		if ( !HealthBoxAvailable ) {
			HealthBoxtimer += Time.deltaTime;
			if ( HealthBoxtimer >= HealthBoxtime ) {
				HealthBoxAvailable = true;
				SpriteRendererRef.enabled = true;
			}
		}

		// Sin wave stuff
		index += Time.deltaTime;
		float y = Mathf.Abs (amplitudeY*Mathf.Sin (omegaY*index))+RandomOffset;
		ChildTransformRef.localPosition = new Vector3(0,y,0);
	}

	void OnTriggerEnter2D( Collider2D other ) {

		// Allows player to jump
		if ( other.gameObject.tag == "Player" && HealthBoxAvailable ) {
			other.gameObject.GetComponent<CharacterManager> ().AddHealth ();
			HealthBoxAvailable = false;
			SpriteRendererRef.enabled = false;
			HealthBoxtimer = 0.0f;
			AudioSourceRef.Play ();
		}
	}

	public void ResetBoxes () {
		HealthBoxtimer = HealthBoxtime;
	}
}
