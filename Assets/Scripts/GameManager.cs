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

	GUIStyle HealthGUIStyle;
	GUIStyle TitleGUIStyle;
	GUIStyle RedGUIStyle;
	GUIStyle BlueGUIStyle;
	GUIStyle StartGUIStyle;

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

		TitleGUIStyle = new GUIStyle();
		TitleGUIStyle.fontSize = (int) (Screen.width*0.08f);
		TitleGUIStyle.normal.textColor = Color.white;
		TitleGUIStyle.font = (Font)Resources.Load<Font>("Fonts/knewave");

		StartGUIStyle = new GUIStyle();
		StartGUIStyle.fontSize = (int) (Screen.width*0.04f);
		StartGUIStyle.normal.textColor = Color.white;
		StartGUIStyle.font = (Font)Resources.Load<Font>("Fonts/playtime");

		RedGUIStyle = new GUIStyle();
		RedGUIStyle.fontSize = (int) (Screen.width*0.08f);
		RedGUIStyle.normal.textColor = Color.red;
		RedGUIStyle.font = (Font)Resources.Load<Font>("Fonts/knewave");

		BlueGUIStyle = new GUIStyle();
		BlueGUIStyle.fontSize = (int) (Screen.width*0.08f);
		BlueGUIStyle.normal.textColor = Color.blue;
		BlueGUIStyle.font = (Font)Resources.Load<Font>("Fonts/knewave");

		HealthGUIStyle = new GUIStyle();
		HealthGUIStyle.fontSize = (int) (Screen.width*0.045f);
		HealthGUIStyle.normal.textColor = Color.white;
		HealthGUIStyle.font = (Font)Resources.Load<Font>("Fonts/playtime");
	}

	void OnGUI() {
		if (GameState == 0) {
			GUI.Label (new Rect (Screen.width*0.28f, Screen.height*0.2f, 0, 0), "SHOCKWAVE!", TitleGUIStyle);
			GUI.Label (new Rect (Screen.width*0.33f, Screen.height*0.73f, 0, 0), "Press Start to begin...", StartGUIStyle);
			if (WinningTeam == 1) {
				GUI.Label (new Rect (Screen.width*0.24f, Screen.height*0.49f, 0, 0), "Red Team Wins", RedGUIStyle);
			} else if (WinningTeam == 2) {
				GUI.Label (new Rect (Screen.width*0.24f, Screen.height*0.49f, 0, 0), "Blue Team Wins", BlueGUIStyle);
			}
		} else {
			for (int i1 = 0; i1 < 4; i1++) {
				GUI.Label (new Rect (Screen.height*0.2f + Screen.height*0.4f*i1, Screen.height*0.85f, 0.0f, 0.0f), Players [i1].GetComponent<CharacterManager> ().CurrentHealth.ToString () + "%", HealthGUIStyle);
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
		TheBomb.GetComponent<BoxCollider2D>().enabled = true;

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
