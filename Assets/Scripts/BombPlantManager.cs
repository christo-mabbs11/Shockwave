using UnityEngine;
using System.Collections;

public class BombPlantManager : MonoBehaviour {

	public string TeamName = "";
	private float PlantPush = 15000.0f;
	private float PlantRadius = 100.0f;

	private bool BombTimeActivated = false;
	private float BombTime = 2.0f;
	private float BombTimer = 0.0f;

	GameObject GameManagerRef;
	GameObject[] Players;

	void Awake (  ) {
		GameManagerRef = GameObject.FindGameObjectWithTag ("GameManager");
		Players = GameObject.FindGameObjectsWithTag("Player");
	}

	void Update () {
		if ( BombTimeActivated ) {
			BombTimer += Time.deltaTime;
			if ( BombTimer >= BombTime ) {
				BombTimeActivated = false;
				ExplodePeeps ();
				GameManagerRef.GetComponent<GameManager> ().Endgame ();
			}
		}
	}

	public void BombPlanted () {
		BombTimeActivated = true;
		BombTimer = 0.0f;
	}

	void ExplodePeeps () {

		// Loop through all the player objects whithin a certain radius
		foreach (GameObject player in Players) {

			float PlayerDistance = Mathf.Abs(Vector3.Distance (transform.position, player.transform.position));

			if ( PlayerDistance <= PlantRadius ) {

				// Find the angle of the blast
				Vector2 BlastAngle = new Vector2( player.transform.position.x - this.transform.position.x, player.transform.position.y - this.transform.position.y );

				Vector3 BlastAngleNormalized = Vector3.Normalize ( new Vector3( BlastAngle.x, BlastAngle.y, 0.0f) );
				Vector2 BlastAngleNormalized_2D = new Vector2( BlastAngleNormalized.x, BlastAngleNormalized.y );

				// Set the players current velocity to 0
				player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

				// Apply blast, scaled for distance
				player.GetComponent<Rigidbody2D>().AddForce( PlantPush * (1.0f - PlayerDistance / PlantRadius) * BlastAngleNormalized_2D, ForceMode2D.Impulse );

				// Disable the movement of the player being hit
				player.GetComponent<CharacterManager> ().GrenadeDisableMovement ();

			}
		}
	}
}
