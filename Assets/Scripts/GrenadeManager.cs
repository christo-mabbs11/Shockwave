using UnityEngine;
using System.Collections;

public class GrenadeManager : MonoBehaviour {

	public float GrenadePush = 45.0f;
	public float GrenadeRadius = 10.0f;


	// If the grenade collides with something
	void OnCollisionEnter2D(Collision2D coll) {

		// Loop through all the player objects whithin a certain radius
		GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in Players) {
			float PlayerDistance = Vector3.Distance (transform.position, player.transform.position);
			if ( PlayerDistance <= GrenadeRadius ) {

				// Find the angle of the blast
				Vector2 BlastAngle = new Vector2( player.transform.position.x - this.transform.position.x, player.transform.position.y - this.transform.position.y );

				// Apply blast, scaled for distance
				player.GetComponent<Rigidbody2D>().AddForce( GrenadePush * (1.0f - PlayerDistance / GrenadeRadius) * BlastAngle, ForceMode2D.Impulse );

			}
		}

		// Destroy this grenade
		Destroy (this.gameObject);
	}
}
