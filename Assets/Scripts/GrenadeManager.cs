using UnityEngine;
using System.Collections;

public class GrenadeManager : MonoBehaviour {

	public float GrenadePush = 45.0f;
	public float GrenadeRadius = 10.0f;


	// If the grenade collides with something
	void OnCollisionEnter2D(Collision2D coll) {

		// Ignores players
		if ( coll.gameObject.tag != "Player" ) {

			// Loop through all the player objects whithin a certain radius
			GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject player in Players) {

				float PlayerDistance = Mathf.Abs(Vector3.Distance (transform.position, player.transform.position));
				Debug.Log ( PlayerDistance );

				if ( PlayerDistance <= GrenadeRadius ) {

					// Find the angle of the blast
					Vector2 BlastAngle = new Vector2( player.transform.position.x - this.transform.position.x, player.transform.position.y - this.transform.position.y );

					Vector3 BlastAngleNormalized = Vector3.Normalize ( new Vector3( BlastAngle.x, BlastAngle.y, 0.0f) );
					Vector2 BlastAngleNormalized_2D = new Vector2( BlastAngleNormalized.x, BlastAngleNormalized.y );

					// Set the players current velocity to 0
					player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

					// Apply blast, scaled for distance
					player.GetComponent<Rigidbody2D>().AddForce( GrenadePush * (1.0f - PlayerDistance / GrenadeRadius) * BlastAngleNormalized_2D, ForceMode2D.Impulse );

					// Disable the movement of the player being hit
					player.GetComponent<CharacterManager> ().GrenadeDisableMovement ();

				}
			}

			// Destroy this grenade
			Destroy (this.gameObject);
		}
	}
}
