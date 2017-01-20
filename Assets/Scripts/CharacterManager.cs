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
	private bool JumpKeyReleased = true;
	private bool PlayerDirection = true; // left - false, right - true
	SpriteRenderer SpriteRendererRef;
	public string Control_Horizontal = "Horizontal";
	public string Control_RightAimX = "RightAimX";
	public string Control_RightAimY = "RightAimY";
	public string Control_RightTrigger = "RightTrigger";
	public string Control_Jump = "Jump";
	public string Control_Reload = "Reload";

	// Gun Related Variables
	Transform Gunref;
	Vector2 GunMaxAngle = new Vector2( 80, -80 );
	private bool TriggerPressed = false;
	public float GunFireRate = 0.3f;
	public float GunFireTimer = 0.0f;
	private bool GunCanFire = true;
	public int TotalNumberOfBullets = 15;
	public int NumberOfBullets = 15;
	private bool PlayerReloading = false;
	public float PlayerReloadingTime = 2.0f;
	public float PlayerReloadingTimer = 0.0f;

	void Awake() {

		// Players must always be spawned in the air (and drop down)
		// Movement related variables
		Rigidbody2DRef = this.GetComponent<Rigidbody2D>();
		Using_AirMultiplier = AirMultiplier;
		SpriteRendererRef = this.GetComponent<SpriteRenderer> ();

		// Gun related variables
		Gunref = this.gameObject.transform.GetChild(0);
	}

	// Update is called once per frame
	void Update () {

		// Update gun firing
		if ( !GunCanFire ) {
			GunFireTimer += Time.deltaTime;
			if ( GunFireTimer >= GunFireRate ) {
				GunCanFire = true;
			}
		}

		// Update gun reloading
		if ( PlayerReloading ) {
			PlayerReloadingTimer += Time.deltaTime;
			if ( PlayerReloadingTimer >= PlayerReloadingTime ) {
				PlayerReloading = false;
				NumberOfBullets = TotalNumberOfBullets;
			}
		}

		// Horizontal movement
		MovePlayer(Input.GetAxis (Control_Horizontal) );

		SetPlayerDirection ( Input.GetAxis (Control_Horizontal) );

		// Player jumping
		if ((Input.GetAxis (Control_Jump) == 1) && (NumberOfJumpsAllowed > JumpCount) && JumpKeyReleased) {
			JumpPlayer ();
		} else if (Input.GetAxis (Control_Jump) == 0) {
			JumpKeyReleased = true;
		}

		// Gun aim (and firing, through the function)
		if ( Input.GetAxis(Control_RightAimX)!=0 || Input.GetAxis(Control_RightAimY)!=0 ) {
			float GunAngle = -Mathf.Atan2(Input.GetAxis(Control_RightAimY), Input.GetAxis(Control_RightAimX)) * 180 / Mathf.PI;
			AimGun ( GunAngle );
		}

		// Gun reload button hit
		if ((Input.GetAxis (Control_Reload) == 1) && (NumberOfBullets < TotalNumberOfBullets)) {
			ReloadGun ();
		}
	}

	void MovePlayer ( float Arg_MoveScale ) {
		Rigidbody2DRef.AddForce ( new Vector2( PlayerSpeed * Arg_MoveScale * Using_AirMultiplier, 0 ) );
	}

	void JumpPlayer () {

		JumpKeyReleased = false;

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
		if ( (Input.GetAxis(Control_RightTrigger)!= 0 && !TriggerPressed) || 
			(Input.GetAxis(Control_RightTrigger)!= 1 && TriggerPressed)) {

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

		if ( GunCanFire && !PlayerReloading && ( NumberOfBullets > 0) ) {

			NumberOfBullets--;
			GunCanFire = false;
			GunFireTimer = 0.0f;

			// Use raycasting to determine hits
			RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).GetChild(1).position, Arg_GunAngle);
			if (hit.collider != null) {
				Debug.DrawLine( new Vector3( transform.GetChild(0).GetChild(1).position.x, transform.GetChild(0).GetChild(1).position.y, 0),
					new Vector3( hit.point.x, hit.point.y, 0 ), 
					Color.red, GunFireRate);
			}

		// Reload the gun if player tries to shoot and has no bullets
		} else if ( NumberOfBullets <= 0 && !PlayerReloading ) {
			ReloadGun ();
		}
	}

	void ReloadGun() {
		PlayerReloading = true;
		PlayerReloadingTimer = 0.0f;
	}
}
