using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// 0 - menu, 1 - gametime
	private int GameState = 0;

	// 0 - nobody, 1 - red, 2 - blue
	private int WinningTeam = 0;

	// References various game objects
	GameObject TheBomb;
	GameObject BombSpawn;
	GameObject[] Players;

	void Awake() {

		// Get handy object references
		TheBomb = GameObject.FindGameObjectWithTag ("Bomb");
		BombSpawn = GameObject.FindGameObjectWithTag ("BombSpawn");
		Players = GameObject.FindGameObjectsWithTag("Player");

		// Set the bomb starting positon
		TheBomb.transform.position = BombSpawn.transform.position;

		// Get players to ignore each others collision
		for ( int i1 = 0 ; i1 < 4 ; i1++ ) {
			for ( int i2 = 0 ; i2 < 4 ; i2++ ) {
				if ( i1 != i2 ) {
					Physics2D.IgnoreCollision(Players[i1].GetComponent<BoxCollider2D>(), Players[i2].GetComponent<BoxCollider2D>());
				}
			}
		}
	}

	void OnGUI() {
		if ( GameState == 0 ) {
			GUI.Label (new Rect (10, 10, 150, 100), "Shockwave");
			GUI.Label (new Rect (10, 50, 150, 100), "Press Start to begin!");
			if ( WinningTeam == 1 ) {
				GUI.Label (new Rect (10, 30, 150, 100), "Red Team Wins");
			} else if ( WinningTeam == 2 ) {
				GUI.Label (new Rect (10, 30, 150, 100), "Blue Team Wins");
			}
		}
	}

	void Update () {

		if ( Input.GetAxis ("StartButton") == 1 && GameState == 0 ) {
			BeginGame ();
		}
	}
		
	void BeginGame() {

		// Set new game state
		GameState = 1;

		// Remove Bomb parent
		TheBomb.transform.parent = null;

		// Reset bomb position
		TheBomb.transform.position = BombSpawn.transform.position;

		// Enable Rigidbody and collider
		TheBomb.GetComponent<Rigidbody2D>().isKinematic = false;
		TheBomb.GetComponent<CircleCollider2D>().enabled = true;

		// Enable player control

		// Spawn all players
		foreach (GameObject player in Players) {
			player.GetComponent<CharacterManager> ().KillPlayer ();
		}
	}

	public void Endgame () {
		
		// Disable player control

		// Reset game state
		GameState = 0;

	}
}
