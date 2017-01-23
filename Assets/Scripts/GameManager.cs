using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// 0 - menu, 1 - gametime
	private int GameState = 0;

	// 0 - nobody, 1 - red, 2 - blue
	public int WinningTeam = 0;

	// References various game objects
	GameObject TheBomb;
	GameObject BombSpawn;
	GameObject[] Players;

	GUIStyle HealthGUIStyle;
	GUIStyle TitleGUIStyle;
	GUIStyle RedGUIStyle;
	GUIStyle BlueGUIStyle;
	GUIStyle StartGUIStyle;
	GUIStyle NumberGUIStyle;
	GUIStyle AmmoGUIStyle;

	private bool GameStartTimerActive = false;
	private bool GameBeginCalled = false;
	private float GameStartTime = 4.0f;
	private float GameHaltTime = 1.0f;
	private float GameStartTimer = 0.0f;

	private GameObject[] HealthBoxManagerRef;
	private GameObject[] DieBoxRef;

	private AudioSource AudioSourceRef;
	private AudioSource SecondSoundRef;
	private bool SecondSound = false;
	private bool ThirdSound = false;
	private bool FourthSound = false;

	void Awake() {

		AudioSourceRef = this.GetComponent<AudioSource> ();
		SecondSoundRef = transform.GetChild (0).GetComponent<AudioSource> ();

		// Get handy object references
		TheBomb = GameObject.FindGameObjectWithTag ("Bomb");
		BombSpawn = GameObject.FindGameObjectWithTag ("BombSpawn");
		Players = GameObject.FindGameObjectsWithTag("Player");
		HealthBoxManagerRef = GameObject.FindGameObjectsWithTag("HealthBox");
		DieBoxRef = GameObject.FindGameObjectsWithTag("DieBox");

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
		TitleGUIStyle.normal.textColor = new Color (230.0f/255.0f,230.0f/255.0f,230.0f/255.0f, 1.0f);
		TitleGUIStyle.font = (Font)Resources.Load<Font>("Fonts/knewave");

		StartGUIStyle = new GUIStyle();
		StartGUIStyle.fontSize = (int) (Screen.width*0.04f);
		StartGUIStyle.normal.textColor = Color.white;
		StartGUIStyle.font = (Font)Resources.Load<Font>("Fonts/playtime");

		RedGUIStyle = new GUIStyle();
		RedGUIStyle.fontSize = (int) (Screen.width*0.08f);
		RedGUIStyle.normal.textColor = new Color (198.0f/255.0f,40.0f/255.0f,40.0f/255.0f, 1.0f);
		RedGUIStyle.font = (Font)Resources.Load<Font>("Fonts/knewave");

		BlueGUIStyle = new GUIStyle();
		BlueGUIStyle.fontSize = (int) (Screen.width*0.08f);
		BlueGUIStyle.normal.textColor =  new Color (21.0f/255.0f,101.0f/255.0f,192.0f/255.0f, 1.0f);
		BlueGUIStyle.font = (Font)Resources.Load<Font>("Fonts/knewave");

		NumberGUIStyle = new GUIStyle();
		NumberGUIStyle.fontSize = (int) (Screen.width*0.18f);
		NumberGUIStyle.normal.textColor = new Color (230.0f/255.0f,230.0f/255.0f,230.0f/255.0f, 1.0f);
		NumberGUIStyle.font = (Font)Resources.Load<Font>("Fonts/knewave");

		AmmoGUIStyle = new GUIStyle();
		AmmoGUIStyle.fontSize = (int) (Screen.width*0.025f);
		AmmoGUIStyle.normal.textColor = Color.white;
		AmmoGUIStyle.font = (Font)Resources.Load<Font>("Fonts/playtime");

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

			if ( GameStartTimerActive ) {
				if (GameStartTimer > 3.0f) {
					GUI.Label (new Rect (Screen.width*0.25f, Screen.height*0.2f, 0, 0), "Fight!", NumberGUIStyle);
				} else if (GameStartTimer > 2.0f) {
					GUI.Label (new Rect (Screen.width*0.46f, Screen.height*0.2f, 0, 0), "1", NumberGUIStyle);
				} else if (GameStartTimer > 1.0f) {
					GUI.Label (new Rect (Screen.width*0.445f, Screen.height*0.2f, 0, 0), "2", NumberGUIStyle);
				} else {
					GUI.Label (new Rect (Screen.width*0.44f, Screen.height*0.2f, 0, 0), "3", NumberGUIStyle);
				}
			}

			for (int i1 = 0; i1 < 4; i1++) {
				GUI.Label (new Rect (Screen.width*0.2f + Screen.width*0.2f*i1, Screen.height*0.86f, 0.0f, 0.0f), Players [i1].GetComponent<CharacterManager> ().CurrentHealth.ToString () + "%", HealthGUIStyle);
				GUI.Label (new Rect (Screen.width*0.2f + Screen.width*0.2f*i1 + Screen.width*0.015f, Screen.height*0.93f, 0.0f, 0.0f), Players [i1].GetComponent<CharacterManager> ().NumberOfBullets.ToString (), AmmoGUIStyle);
			}
		}
	}

	void Update () {

		if ( Input.GetAxis ("StartButton") == 1 && GameState == 0 ) {
			StartGame ();
		}

		if ( GameStartTimerActive ) {
			GameStartTimer += Time.deltaTime;


			if ( !SecondSound && GameStartTimer > (GameStartTime*1.0f/4.0f)) {
				SecondSound = true;
				AudioSourceRef.Play ();
			}

			if ( !ThirdSound && GameStartTimer > (GameStartTime*2.0f/4.0f) ) {
				ThirdSound = true;
				AudioSourceRef.Play ();
			}

			if ( !FourthSound && GameStartTimer > (GameStartTime*3.0f/4.0f) ) {
				FourthSound = true;
				SecondSoundRef.Play ();
			}

			if ( GameStartTimer >= GameHaltTime && !GameBeginCalled ) {
				GameBeginCalled = true;
				BeginGame ();
			}
			if ( GameStartTimer >= GameStartTime ) {
				GameStartTimerActive = false;
			}
		}
	}

	void StartGame(  ) {

		// Set new game state
		GameState = 1;

		// Remove Bomb parent
		TheBomb.transform.parent = null;

		// Reset bomb position
		TheBomb.transform.position = BombSpawn.transform.position;

		// Enable Rigidbody and collider
		TheBomb.GetComponent<Rigidbody2D>().isKinematic = false;
		TheBomb.GetComponent<CircleCollider2D>().enabled = true;

		GameStartTimerActive = true;
		GameBeginCalled = false;
		GameStartTimer = 0.0f;

		// Play noise
		SecondSound = false;
		ThirdSound = false;
		FourthSound = false;
		AudioSourceRef.Play ();
	}

	void BeginGame() {

		// Spawn all players
		foreach (GameObject player in Players) {
			player.GetComponent<CharacterManager> ().KillPlayer ();
		}

		// Spawn health boxes
		foreach (GameObject HPBox in HealthBoxManagerRef) {
			HPBox.GetComponent<HealthBoxManager> ().ResetBoxes ();
		}

		// Enable Die Boxes
		foreach (GameObject DieBox in DieBoxRef) {
			DieBox.GetComponent<BoxCollider2D>().enabled = true;
		}

	}

	public void Endgame () {

		// Reset game state
		GameState = 0;

		// Disable Die Boxes
		foreach (GameObject DieBox in DieBoxRef) {
			DieBox.GetComponent<BoxCollider2D>().enabled = false;
		}
	}
}
