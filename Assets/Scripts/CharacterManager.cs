using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour {
	
	// Movement variables
	Rigidbody2D Rigidbody2DRef;
	public float PlayerSpeed = 40.0f;
	public float JumpForce = 30.0f;
	public float AirMultiplier = 0.5f;
	private float Using_AirMultiplier = 1.0f;
	private int JumpCount = 0;
	private int NumberOfJumpsAllowed = 2;
	private bool PlayerDirection = true; // left - false, right - true
	SpriteRenderer SpriteRendererRef;

	// Gun Related Variables
	Transform Gunref;
	Vector2 GunMaxAngle = new Vector2( 80, -80 );
	private bool TriggerPressed = false;

	void Awake() {

		// Movement related variables
		Rigidbody2DRef = this.GetComponent<Rigidbody2D>();
		Using_AirMultiplier = AirMultiplier;
		JumpCount = 0;
		NumberOfJumpsAllowed = 2;
		SpriteRendererRef = this.GetComponent<SpriteRenderer> ();

		// Gun related variables
		Gunref = this.gameObject.transform.GetChild(0);
		TriggerPressed = false;
	}

	// Update is called once per frame
	void Update () {

		// Horizontal movement
		MovePlayer(Input.GetAxis ("Horizontal") );

		SetPlayerDirection ( Input.GetAxis ("Horizontal") );

		// Player jumping
		if (Input.GetKeyDown(KeyCode.JoystickButton16) && (NumberOfJumpsAllowed > JumpCount)) {
			JumpPlayer ();
		}

		// Gun aim
		if ( Input.GetAxis("RightAimX")!=0 || Input.GetAxis("RightAimY")!=0 ) {
			float GunAngle = -Mathf.Atan2(Input.GetAxis("RightAimY"), Input.GetAxis("RightAimX")) * 180 / Mathf.PI;
			AimGun ( GunAngle );
		}
	}

	void MovePlayer ( float Arg_MoveScale ) {
		Rigidbody2DRef.AddForce ( new Vector2( PlayerSpeed * Arg_MoveScale * Using_AirMultiplier, 0 ) );
	}

	void JumpPlayer () {

		// Indicate player has jumped
		JumpCount++;

		// Apple the jump force
		Rigidbody2DRef.AddForce ( new Vector2( 0, JumpForce ), ForceMode2D.Impulse );

		// Applies less horizontal force to player as they are jumping
		Using_AirMultiplier = AirMultiplier;
	}

	void OnTriggerEnter2D( Collider2D other ) {
		if ( other.gameObject.tag == "Ground" ) {
			Using_AirMultiplier = 1.0f;
			JumpCount = 0;
		}
	}

	void AimGun ( float Arg_GunAngle ) {

		// If player has switched direction
		if ( !PlayerDirection ) {
			Arg_GunAngle += 180;
			if ( Arg_GunAngle >= 180 ) {
				Arg_GunAngle -= 360;
			}
		}

//		Vector2 FireDirection = new Vector2 ( Input.GetAxis("RightAimX"), -Input.GetAxis("RightAimY") );

		// Limits the angle player can point gun
		if ( Arg_GunAngle > GunMaxAngle.x ) {
			Arg_GunAngle = GunMaxAngle.x;
		} else if ( Arg_GunAngle < GunMaxAngle.y ) {
			Arg_GunAngle = GunMaxAngle.y;
		}

		Vector2 FireDirection;
		if (PlayerDirection) {
			FireDirection = (Vector2)(Quaternion.Euler(0,0,Arg_GunAngle) * Vector2.right);
		} else {
			FireDirection = (Vector2)(Quaternion.Euler(0,0,Arg_GunAngle) * Vector2.left);
		}

		// Set the angle of the gun
		Gunref.eulerAngles = new Vector3(0, 0, Arg_GunAngle);

		// Gun Fire (player can only fire gun if they're aiming)
		if ( (FireDirection.x!= 0 && !TriggerPressed) || 
			(FireDirection.y!= 1 && TriggerPressed)) {

			TriggerPressed = true;
			FireGun ( new Vector2( FireDirection.x, FireDirection.y ) );
		}
	}

	void SetPlayerDirection ( float Arg_MoveScale ) {

		// If the player should be facing left
		if ( Arg_MoveScale < 0 && PlayerDirection ) {

			PlayerDirection = false;
			SpriteRendererRef.flipX = true;
			Gunref.localScale = new Vector3 (-1, 1, 1);

		// If the player should be facing right
		} else if ( Arg_MoveScale > 0 && !PlayerDirection ) {

			PlayerDirection = true;
			SpriteRendererRef.flipX = false;
			Gunref.localScale = new Vector3 (1, 1, 1);
		}
	}

	void FireGun( Vector2 Arg_GunAngle ) {
		RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).GetChild(1).position, Arg_GunAngle);
		if (hit.collider != null) {
			Debug.DrawLine( new Vector3( transform.GetChild(0).GetChild(1).position.x, transform.GetChild(0).GetChild(1).position.y, 0 ),
				new Vector3( hit.point.x, hit.point.y, 0 ), 
				Color.red);
		}
	}
}
