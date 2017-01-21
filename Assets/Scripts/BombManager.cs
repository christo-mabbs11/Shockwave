using UnityEngine;
using System.Collections;

public class BombManager : MonoBehaviour {

	private bool BombResetting = false;
	private float BombResetTime = 1.2f;
	private float BombResetTimer = 0.0f;

	private GameObject BombSpawn;

	void Awake () {
		BombSpawn = GameObject.FindGameObjectWithTag ("BombSpawn");
	}

	void Update () {

		// Reset bomb location after timer
		if ( BombResetting ) {
			BombResetTimer += Time.deltaTime;
			if ( BombResetTimer > BombResetTime ) {
				this.transform.position = BombSpawn.transform.position;
				BombResetting = false;
			}
		}
	}

	void OnTriggerEnter2D( Collider2D other ) {

		// Kills player if they're out of the boundary
		if ( other.gameObject.tag == "DieBox" ) {
			BombResetting = true;
			BombResetTimer = 0.0f;
		}
	}
}
