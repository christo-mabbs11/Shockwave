using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour {
	
	// Movement variables
	Rigidbody2D Rigidbody2DRef;
	public float PlayerSpeed = 30.0f;
	public float JumpForce = 10.0f;
	public float AirMultiplier = 0.5f;
	private float Using_AirMultiplier = 1.0f;
	bool PlayerGrounded = false;

	void Awake() {

		Rigidbody2DRef = this.GetComponent<Rigidbody2D>();
		PlayerGrounded = false;
		Using_AirMultiplier = AirMultiplier;
	}

	// Update is called once per frame
	void Update () {

		// Basic player movement
		MovePlayer(Input.GetAxis ("Horizontal") );
		if (Input.GetKeyDown(KeyCode.JoystickButton16) && PlayerGrounded) {
			JumpPlayer ();
		}
	}

	void MovePlayer ( float Arg_MoveScale ) {
		Rigidbody2DRef.AddForce ( new Vector2( PlayerSpeed * Arg_MoveScale * Using_AirMultiplier, 0 ) );
	}

	void JumpPlayer () {
		Rigidbody2DRef.AddForce ( new Vector2( 0, JumpForce ), ForceMode2D.Impulse );
		PlayerGrounded = false;
		Using_AirMultiplier = AirMultiplier;
	}

	void OnTriggerEnter2D( Collider2D other ) {
		if ( other.gameObject.tag == "Ground" ) {
			PlayerGrounded = true;
			Using_AirMultiplier = 1.0f;
		}
	}

}
