using UnityEngine;
using System.Collections;

public class GrenadeManager : MonoBehaviour {

	public float GrenadePush = 45.0f;
	public float GrenadeRadius = 10.0f;

	public GameObject GrenadeNoise;

	GameObject BombRef;

	void Awake () {

		BombRef = GameObject.FindGameObjectWithTag ("Bomb");

		// Grenade ignores collisions with the bomb
		Physics2D.IgnoreCollision(this.gameObject.GetComponent<CircleCollider2D>(), BombRef.GetComponent<BoxCollider2D>());
	}

	// If the grenade collides with something
	void OnCollisionEnter2D(Collision2D coll) {

		// Ignores players and the bomb
		if ( coll.gameObject.tag != "Player" && coll.gameObject.tag != "Bomb" ) {

			// Loop through all the player objects whithin a certain radius
			GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject player in Players) {
				BlowObject ( player, GrenadePush );
			}

			// Blow up the bomb
			BlowObject ( BombRef, GrenadePush * 0.14f );

			// Create noise
			Instantiate(GrenadeNoise, Vector3.zero, Quaternion.identity);

			// Destroy this grenade
			Destroy (this.gameObject);
		}
	}

	void BlowObject ( GameObject Arg_GameObject, float Arg_GrenadePush ) {
		
		float GameObjectDistance = Mathf.Abs(Vector3.Distance (transform.position, Arg_GameObject.transform.position));

		if ( GameObjectDistance <= GrenadeRadius ) {

			// Find the angle of the blast
			Vector2 BlastAngle = new Vector2( Arg_GameObject.transform.position.x - this.transform.position.x, Arg_GameObject.transform.position.y - this.transform.position.y );

			Vector3 BlastAngleNormalized = Vector3.Normalize ( new Vector3( BlastAngle.x, BlastAngle.y, 0.0f) );
			Vector2 BlastAngleNormalized_2D = new Vector2( BlastAngleNormalized.x, BlastAngleNormalized.y );

			// Set the players current velocity to 0
			Arg_GameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

			// Apply blast, scaled for distance
			Arg_GameObject.GetComponent<Rigidbody2D>().AddForce( Arg_GrenadePush * (1.0f - GameObjectDistance / GrenadeRadius) * BlastAngleNormalized_2D, ForceMode2D.Impulse );

			// Disable the movement if a player is being hit
			if ( Arg_GameObject.tag == "Player" ) {
				Arg_GameObject.GetComponent<CharacterManager> ().GrenadeDisableMovement ();
			}
		}
	}
}
