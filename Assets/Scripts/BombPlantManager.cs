using UnityEngine;
using System.Collections;

public class BombPlantManager : MonoBehaviour {

	public string TeamName = "";
	private float PlantPush = 6000.0f;
	private float PlantRadius = 100.0f;

	private bool BombTimeActivated = false;
	private float BombTime = 2.0f;
	private float BombTimer = 0.0f;

	GameObject GameManagerRef;
	GameObject[] Players;
	private AudioSource AudioSourceRef;
	private AudioSource CountDownRef;

	private bool SecondSound = false;
	private bool ThirdSound = false;

	public GameObject ShockWaveEffect;

	void Awake (  ) {
		GameManagerRef = GameObject.FindGameObjectWithTag ("GameManager");
		Players = GameObject.FindGameObjectsWithTag("Player");
		AudioSourceRef = this.GetComponent<AudioSource> ();
		CountDownRef = transform.GetChild (1).GetComponent<AudioSource> ();
	}

	void Update () {
		if ( BombTimeActivated ) {
			BombTimer += Time.deltaTime;

			if ( !SecondSound && BombTimer > (BombTime*1.0f/3.0f)) {
				SecondSound = true;
				CountDownRef.Play ();
			}

			if ( !ThirdSound && BombTimer > (BombTime*2.0f/3.0f) ) {
				ThirdSound = true;
				CountDownRef.Play ();
			}

			if ( BombTimer >= BombTime ) {
				BombTimeActivated = false;
				ExplodePeeps ();
				GameManagerRef.GetComponent<GameManager> ().Endgame ();
			}
		}
	}


	void ResetTimer () {
		SecondSound = false;
		ThirdSound = false;
	}

	public void BombPlanted () {
		ResetTimer ();
		BombTimeActivated = true;
		BombTimer = 0.0f;
		CountDownRef.Play ();
	}

	void ExplodePeeps () {

		AudioSourceRef.Play ();

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

		// Create shockwave
		ShockwaveManager SchockwaveRef = ((GameObject) Instantiate(ShockWaveEffect, this.transform.position, Quaternion.identity)).GetComponent<ShockwaveManager>();
		SchockwaveRef.SetWaveTime (0.35f);
		SchockwaveRef.SetWaveSpeed (8.0f);
	}
}
