using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour {

	// General variables
	public string PlayerName = "";
	public string TeamName = "";
	public string Control_Horizontal = "Horizontal";
	public string Control_RightAimX = "RightAimX";
	public string Control_RightAimY = "RightAimY";
	public string Control_RightTrigger = "RightTrigger";
	public string Control_LeftTrigger = "LeftTrigger";
	public string Control_Jump = "Jump";
	public string Control_Reload = "Reload";
	private bool LeftTriggerPressed = false;
	private bool RightTriggerPressed = false;

	// Movement variables
	private Rigidbody2D Rigidbody2DRef;
	private float PlayerSpeed = 4000.0f;
	private float JumpForce = 2500.0f;
	private float AirMultiplier = 0.5f;
	private float Using_AirMultiplier = 1.0f;
	private int JumpCount = 0;
	private int NumberOfJumpsAllowed = 2;
	private bool JumpKeyReleased = true;
	private bool PlayerDirection = true; // left - false, right - true
	SpriteRenderer SpriteRendererRef;

	// Gun Related Variables
	Transform Gunref;
	Vector2 GunMaxAngle = new Vector2( 80, -80 );
	private float GunFireRate = 0.3f;
	private float GunFireTimer = 0.0f;
	private bool GunCanFire = true;
	private int TotalNumberOfBullets = 15;
	private int NumberOfBullets = 15;
	private bool PlayerReloading = false;
	private float PlayerReloadingTime = 2.0f;
	private float PlayerReloadingTimer = 0.0f;
	private float GunDamage = 25.0f;

	// Grenade related variables
	private bool CanThrowGrenade = true;
	private bool GrenadeCooldownOff = true;
	public GameObject Grenade;
	private float GrenadeThrowForce = 10.0f;
	private float GrenadeCooldownTime = 3.0f;
	private float GrenadeCooldownTimer = 0.0f;

	// Health related variables
	public GameObject SpawnPoint;
	private float MaxHealth = 100.0f;
	private float CurrentHealth = 0.0f;
	private bool PlayerAlive = true;
	private float PlayerRespawnTime = 3.0f;
	private float PlayerRespawnTimer = 0.0f;

	// Bomb related mechanics
	private bool PlayerCarryingBomb = false;
	private float BombThrowForce = 10.0f;
	private GameObject BombRef;
	private float BombSlowMulitplier = 0.65f;

	void Awake() {

		// Start the player in some random position
		transform.position = new Vector3( 40000, 40000, 0 );

		// Debug - start the player in their spawn zone
//		transform.position = SpawnPoint.transform.position;

		// Players must always be spawned in the air (and drop down)
		// Movement related variables
		Rigidbody2DRef = this.GetComponent<Rigidbody2D>();
		Using_AirMultiplier = AirMultiplier;
		SpriteRendererRef = this.GetComponent<SpriteRenderer> ();

		// Gun related variables
		Gunref = this.gameObject.transform.GetChild(0);

		// Get reference to the bomb
		BombRef = GameObject.FindGameObjectWithTag("Bomb");
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

		// Update respawning
		if ( !PlayerAlive ) {
			PlayerRespawnTimer += Time.deltaTime;
			if ( PlayerRespawnTimer >= PlayerRespawnTime ) {
				RespawnPlayer ();
			}
		}

		// Update grenade throwing
		if ( !GrenadeCooldownOff ) {
			GrenadeCooldownTimer += Time.deltaTime;
			if ( GrenadeCooldownTimer >= GrenadeCooldownTime ) {
				GrenadeCooldownOff = true;
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

		// Aiming (Find bomb, grenade and firing functions in function called here too...)
		if ( Input.GetAxis(Control_RightAimX)!=0 || Input.GetAxis(Control_RightAimY)!=0 ) {
			float AimAngle = -Mathf.Atan2(Input.GetAxis(Control_RightAimY), Input.GetAxis(Control_RightAimX)) * 180 / Mathf.PI;
			AimStuff ( AimAngle );
		}

		// Gun reload button hit
		if ((Input.GetAxis (Control_Reload) == 1) && (NumberOfBullets < TotalNumberOfBullets)) {
			ReloadGun ();
		}

		// Grenade trigger release
		if ( (Input.GetAxis(Control_LeftTrigger)== 0 && !LeftTriggerPressed) || 
			(Input.GetAxis(Control_LeftTrigger) == 1 && LeftTriggerPressed)) {
			CanThrowGrenade = true;
		}
	}
		
	void MovePlayer ( float Arg_MoveScale ) {
		
		if ( PlayerCarryingBomb )  {
			
			// player moves more slowly if they have the bomb
			Rigidbody2DRef.AddForce ( new Vector2( PlayerSpeed * Arg_MoveScale * Using_AirMultiplier * BombSlowMulitplier, 0 ) );

		} else {
			Rigidbody2DRef.AddForce ( new Vector2( PlayerSpeed * Arg_MoveScale * Using_AirMultiplier, 0 ) );
		}
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

		// Allows player to jump
		if ( other.gameObject.tag == "Ground" ) {
			Using_AirMultiplier = 1.0f;
			JumpCount = 0;
		}
		// if the player has the bomb and walks over the bomb zone
		if ( other.gameObject.tag == "BombPlant" && PlayerCarryingBomb  ) {

			// Can only plant the bomb in enemy bomb zones
			if ( other.gameObject.GetComponent<BombPlantManager>().TeamName != TeamName ) {
				PlantTheBomb ( other.gameObject );
			}
		}

		// Kills player if they're out of the boundary
		if ( other.gameObject.tag == "DieBox" ) {
			KillPlayer ();
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {

		// if the player walks over the bomb
		if (coll.gameObject.tag == "Bomb") {
			PickUpBomb ( coll.gameObject );
		}
	}

	void AimStuff ( float Arg_GunAngle ) {

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

		// Sets the direction (as a vector 2d) of the firing direction
		Vector2 FireDirection;
		if (PlayerDirection) {
			FireDirection = (Vector2)(Quaternion.Euler(0,0,Arg_GunAngle) * Vector2.right);
		} else {
			FireDirection = (Vector2)(Quaternion.Euler(0,0,Arg_GunAngle) * Vector2.left);
		}

		// Set the angle of the gun/aimer
		Gunref.eulerAngles = new Vector3(0, 0, Arg_GunAngle);

		// If the player is not carrying the bomb
		if ( !PlayerCarryingBomb ) {

			// Gun Fire (player can only fire gun if they're aiming)
			if ( (Input.GetAxis(Control_RightTrigger)!= 0 && !RightTriggerPressed) || 
				(Input.GetAxis(Control_RightTrigger)!= 1 && RightTriggerPressed)) {

				RightTriggerPressed = true;
				FireGun ( new Vector2( FireDirection.x, FireDirection.y ) );
			}

			// Grenade Throw
			if ( (Input.GetAxis(Control_LeftTrigger)!= 0 && !LeftTriggerPressed) || 
				(Input.GetAxis(Control_LeftTrigger)!= 1 && LeftTriggerPressed)) {

				LeftTriggerPressed = true;
				ThrowGrenade ( new Vector2( FireDirection.x, FireDirection.y ) );
			}
		
		// If the player is carrying the bomb
		} else {

			// Bomb Throw
			if ( (Input.GetAxis(Control_LeftTrigger)!= 0 && !LeftTriggerPressed) || 
				(Input.GetAxis(Control_LeftTrigger)!= 1 && LeftTriggerPressed)) {

				LeftTriggerPressed = true;
				ThrowBomb ( new Vector2( FireDirection.x, FireDirection.y ) );
			}
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

				// If colliding object is a player
				if ( hit.collider.tag == "Player" ) {

					// Turns off friendly (ignores if the other object is on the same team)
					if ( hit.collider.GetComponent<CharacterManager>().TeamName != TeamName ) {
						hit.collider.GetComponent<CharacterManager> ().TakeDamage ( GunDamage );
					}
				}
			}

			// Reload the gun if player tries to shoot and has no bullets
		} else if ( NumberOfBullets <= 0 && !PlayerReloading ) {
			ReloadGun ();
		}
	}

	void ThrowGrenade( Vector2 Arg_GrenadeAngle ) {
		if ( CanThrowGrenade && GrenadeCooldownOff ) {

			// Indicate grenade has been thrown
			CanThrowGrenade = false;
			GrenadeCooldownOff = false;
			GrenadeCooldownTimer = 0.0f;

			// Find the grenade starting point
			Vector3 GrenadeStartingPoint;
			if ( PlayerDirection ) {
				GrenadeStartingPoint = transform.GetChild (0).GetChild (1).position + Vector3.right * 2.0f;
			} else {
				GrenadeStartingPoint = transform.GetChild(0).GetChild(1).position + Vector3.left * 2.0f;

			}

			// Create the grenade object
			GameObject TempGrenade = (GameObject) Instantiate(Grenade, GrenadeStartingPoint, Quaternion.identity);

			// Add make it thrown
			TempGrenade.GetComponent<Rigidbody2D>().AddForce( GrenadeThrowForce * Arg_GrenadeAngle, ForceMode2D.Impulse );
		}
	}

	void ReloadGun() {
		PlayerReloading = true;
		PlayerReloadingTimer = 0.0f;
	}

	public void TakeDamage( float Arg_DamageAmount ) {

		if (CurrentHealth > 0) {
			CurrentHealth -= Arg_DamageAmount;

			if ( CurrentHealth <= 0 ) {
				CurrentHealth = 0;
				KillPlayer ();
			}
		}
	}

	public void KillPlayer () {

		// Drop the bomb if they have it
		if ( PlayerCarryingBomb ) {
			DropBomb ();
		}

		PlayerRespawnTimer = 0.0f;
		PlayerAlive = false;
		transform.position = new Vector3( 4000.0f, 4000.0f, 0 );
	}

	void RespawnPlayer () {
		PlayerAlive = true;
		transform.position = SpawnPoint.transform.position;
		CurrentHealth = MaxHealth;
	}

	void PickUpBomb ( GameObject Arg_TheBomb ) {

		// Indicate the player is carrying the bomb
		PlayerCarryingBomb = true;

		// Disable the collider and rigidbody
		Arg_TheBomb.GetComponent<Rigidbody2D>().isKinematic = true;
		Arg_TheBomb.GetComponent<CircleCollider2D>().enabled = false;

		// Set this transform as the parent of the bomb
		Arg_TheBomb.transform.SetParent( this.transform );

		// Put the bomb into a position
		Arg_TheBomb.transform.localPosition = Vector3.zero;

		// Hide the gun object
		transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
	}

	void PlantTheBomb ( GameObject Arg_BombPlant ) {

		// Indicate player is no longer holding the bomb
		PlayerCarryingBomb = false;

		// Set the bomb's parent as the bomb planting point
		BombRef.transform.parent = Arg_BombPlant.transform;

		// Put the bomb into a position
		BombRef.transform.localPosition = new Vector3( -0.08f, 0.98f, 0 );

		// Indicate the bomb has been planted
		Arg_BombPlant.gameObject.GetComponent<BombPlantManager>().BombPlanted();

		// Indicate the player is not carrying the bomb
		PlayerCarryingBomb = false;

		// Make the player gun re-appear
		transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
	}

	void ThrowBomb( Vector2 Arg_BombAngle ) {

		// Indicate player is no longer holding the bomb
		PlayerCarryingBomb = false;

		// Remove this object as the bombs parent
		BombRef.transform.parent = null;

		// Set the bomb starting point
		Vector3 BombStartingPoint;
		if ( PlayerDirection ) {
			BombStartingPoint = transform.GetChild (0).GetChild(1).position + Vector3.right * 2.0f;
		} else {
			BombStartingPoint = transform.GetChild(0).GetChild(1).position + Vector3.left * 2.0f;
		}
		BombRef.transform.position = BombStartingPoint;

		// Enable Rigidbody and collider
		BombRef.GetComponent<Rigidbody2D>().isKinematic = false;
		BombRef.GetComponent<CircleCollider2D>().enabled = true;

		// Add force to the bomb
		BombRef.GetComponent<Rigidbody2D>().AddForce( BombThrowForce * Arg_BombAngle, ForceMode2D.Impulse );

		// Make the gun re-appear
		transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
	}

	void DropBomb( ) {

		// Indicate player is no longer holding the bomb
		PlayerCarryingBomb = false;

		// Remove this object as the bombs parent
		BombRef.transform.parent = null;

		// Enable Rigidbody and collider
		BombRef.GetComponent<Rigidbody2D>().isKinematic = false;
		BombRef.GetComponent<CircleCollider2D>().enabled = true;

		// Make the gun re-appear
		transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
	}
}
