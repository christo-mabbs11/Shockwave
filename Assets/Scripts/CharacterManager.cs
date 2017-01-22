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
	private GameManager GameManagerRef;
	private AudioSource LaserSoundRef;

	// Movement variables
	private Rigidbody2D Rigidbody2DRef;
	private float PlayerSpeed = 4000.0f;
	private float JumpForce = 2500.0f;
	private float AirMultiplier = 0.5f;
	private float Using_AirMultiplier = 1.0f;
	private int JumpCount = 2;
	private int NumberOfJumpsAllowed = 2;
	private bool JumpKeyReleased = true;
	private bool PlayerDirection = true; // left - false, right - true
	SpriteRenderer SpriteRendererRef;
	private AudioSource JumpSoundRef;

	// Gun Related Variables
	Transform Gunref;
	Vector2 GunMaxAngle = new Vector2( 80, -80 );
	private float GunFireRate = 0.12f;
	private float GunFireTimer = 0.0f;
	private bool GunCanFire = true;
	private int TotalNumberOfBullets = 12;
	public int NumberOfBullets = 12;
	private bool PlayerReloading = false;
	private float PlayerReloadingTime = 1.0f;
	private float PlayerReloadingTimer = 0.0f;
	private float GunDamage = 20.0f;
	public GameObject HitMark;
	public GameObject LazerStream;
	private float AimAngle = 0.0f;

	// Grenade related variables
	private bool CanThrowGrenade = true;
	private bool GrenadeCooldownOff = true;
	public GameObject Grenade;
	private float GrenadeThrowForce = 25.0f;
	private float GrenadeCooldownTime = 3.0f;
	private float GrenadeCooldownTimer = 0.0f;

	// Health related variables
	public GameObject SpawnPoint;
	private float MaxHealth = 100.0f;
	public float CurrentHealth = 0.0f;
	private bool PlayerAlive = true;
	private float PlayerRespawnTime = 3.0f;
	private float PlayerRespawnTimer = 0.0f;
	private float AddhealthAmount = 100.0f;
	private AudioSource DeathSoundRef;
	public GameObject GhostRef;

	// Bomb related mechanics
	private bool PlayerCarryingBomb = false;
	private float BombThrowForce = 500.0f;
	private GameObject BombRef;
	private float BombSlowMulitplier = 0.65f;

	// Grenade hit mechanics
	private bool GrenadeHit_MovementDisable = false;
	private float GrenadeHit_DisableTime = 0.85f;
	private float GrenadeHit_DisableTimer = 0.0f;

	void Awake() {

		// Start the player in some random position
		transform.position = new Vector3( 40000, 40000, 0 );

		Rigidbody2DRef = this.GetComponent<Rigidbody2D>();
		Using_AirMultiplier = AirMultiplier;
		SpriteRendererRef = this.GetComponent<SpriteRenderer> ();
		LaserSoundRef = this.GetComponent<AudioSource> ();
		JumpSoundRef = this.gameObject.transform.GetChild (1).GetComponent<AudioSource> ();
		DeathSoundRef = this.gameObject.transform.GetChild (2).GetComponent<AudioSource> ();

		// Gun related variables
		Gunref = this.gameObject.transform.GetChild(0);

		// Get reference to the bomb
		BombRef = GameObject.FindGameObjectWithTag("Bomb");

		GameManagerRef = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	// Update is called once per frame
	void Update () {

		// Update grenade player stun
		if ( GrenadeHit_MovementDisable ) {
			GrenadeHit_DisableTimer += Time.deltaTime;
			if ( GrenadeHit_DisableTimer >= GrenadeHit_DisableTime ) {
				GrenadeHit_MovementDisable = false;
				gameObject.GetComponent<Rigidbody2D> ().drag = 2.81f;
			}
		}

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

		// If player isn't stunned
		if ( !GrenadeHit_MovementDisable ) {
			
			// Horizontal movement
			MovePlayer(Input.GetAxis (Control_Horizontal) );

			if ( Input.GetAxis (Control_RightAimX) != 0.0f ) {
				SetPlayerDirection ( Input.GetAxis (Control_RightAimX) );
			} else if ( Input.GetAxis (Control_Horizontal) != 0.0f ) {
				SetPlayerDirection ( Input.GetAxis (Control_Horizontal) );
			}


			// Player jumping
			if ((Input.GetAxis (Control_Jump) == 1) && (NumberOfJumpsAllowed > JumpCount) && JumpKeyReleased) {
				JumpPlayer ();
			} else if (Input.GetAxis (Control_Jump) == 0) {
				JumpKeyReleased = true;
			}

			// Aiming (Find bomb, grenade and firing functions in function called here too...)
			if ( Input.GetAxis(Control_RightAimX)!=0 || Input.GetAxis(Control_RightAimY)!=0 ) {
				AimStuff ( -Mathf.Atan2(Input.GetAxis(Control_RightAimY), Input.GetAxis(Control_RightAimX)) * 180 / Mathf.PI );
			}

			bool TempRightTriggerTest = false;
			bool TempLeftTrigerTest = false;
			if ((Input.GetAxis(Control_RightTrigger)!= 0 && !RightTriggerPressed) || 
				(Input.GetAxis(Control_RightTrigger)!= 1 && RightTriggerPressed)) {
				TempRightTriggerTest = true;
			} else if ( (Input.GetAxis(Control_LeftTrigger)!= 0 && !LeftTriggerPressed) || 
				(Input.GetAxis(Control_LeftTrigger)!= 1 && LeftTriggerPressed) ) {
				TempLeftTrigerTest = true;
			}

			// if a trigger is being pressed
			if ( TempLeftTrigerTest || TempRightTriggerTest ) {

				// Sets the direction (as a vector 2d) of the firing direction
				Vector2 FireDirection;
				if (PlayerDirection) {
					FireDirection = (Vector2)(Quaternion.Euler(0,0,AimAngle) * Vector2.right);
				} else {
					FireDirection = (Vector2)(Quaternion.Euler(0,0,AimAngle) * Vector2.left);
				}

				// If the player is not carrying the bomb
				if ( !PlayerCarryingBomb ) {

					// Gun Fire (player can only fire gun if they're aiming)
					if ( TempRightTriggerTest ) {

						RightTriggerPressed = true;
						FireGun ( new Vector2( FireDirection.x, FireDirection.y ), AimAngle );
					}

					// Grenade Throw
					if ( TempLeftTrigerTest ) {

						LeftTriggerPressed = true;
						ThrowGrenade ( new Vector2( FireDirection.x, FireDirection.y ) );
					}

					// If the player is carrying the bomb
				} else {

					// Bomb Throw
					if (TempLeftTrigerTest) {

						LeftTriggerPressed = true;
						ThrowBomb ( new Vector2( FireDirection.x, FireDirection.y ) );
					}
				}
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

		// Play Sound
		JumpSoundRef.Play();
	}

	void OnTriggerEnter2D( Collider2D other ) {

		// Allows player to jump
		if (other.gameObject.tag == "Ground") {
			Using_AirMultiplier = 1.0f;
			JumpCount = 0;
		}

		// Kills player if they're out of the boundary
		if ( other.gameObject.tag == "DieBox" ) {
			TakeDamage (100.0f);
		}
	}

	void OnTriggerStay2D ( Collider2D other ) {
		// if the player has the bomb and walks over the bomb zone
		if ( other.gameObject.tag == "BombPlant" && PlayerCarryingBomb  ) {

			// Can only plant the bomb in enemy bomb zones
			if ( other.gameObject.GetComponent<BombPlantManager>().TeamName != TeamName ) {
				PlantTheBomb ( other.gameObject );
				if ( TeamName == "Red" ) {
					GameManagerRef.WinningTeam = 1;
				} else if ( TeamName == "Blue" ) {
					GameManagerRef.WinningTeam = 2;
				}
			}
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

		// Set the angle of the gun/aimer
		Gunref.eulerAngles = new Vector3(0, 0, AimAngle);

		AimAngle = Arg_GunAngle;
	}

	void SetPlayerDirection ( float Arg_MoveScale ) {

		// If the player should be facing left
		if ( Arg_MoveScale < 0 && PlayerDirection ) {

			PlayerDirection = false;
			SpriteRendererRef.flipX = true;
			Gunref.localScale = new Vector3 (-1, 1, 1);
			Gunref.rotation = Quaternion.identity;

		// If the player should be facing right
		} else if ( Arg_MoveScale > 0 && !PlayerDirection ) {

			PlayerDirection = true;
			SpriteRendererRef.flipX = false;
			Gunref.localScale = new Vector3 (1, 1, 1);
			Gunref.rotation = Quaternion.identity;
		}
	}

	void FireGun( Vector2 Arg_GunAngle, float Arg_GunAngleFloat ) {

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

				// Adds hit point on hit point
				// Adds a little randomness to the hit point
				Vector3 TempHitPoint = hit.point;
				TempHitPoint.x += ((float)Random.Range (-100,100))/300.0f;
				TempHitPoint.y += ((float)Random.Range (-100,100))/300.0f;
				GameObject TempGunMark = (GameObject) Instantiate(HitMark, TempHitPoint, Quaternion.identity);

				// Put lazer beam at the shooting point
				GameObject TempLazerStream = (GameObject) Instantiate(LazerStream, transform.GetChild(0).GetChild(1).position, Quaternion.identity);

				// Make lazer beam go backwards if player is facing backwards
				if ( !PlayerDirection ) {
					TempLazerStream.transform.localScale = new Vector3 ( TempLazerStream.transform.localScale.x*-1, 1.0f, 1.0f );
				}

				// Make the lazer go the distance
				float LazerDistance = Vector2.Distance ( hit.point, new Vector2(this.transform.position.x, this.transform.position.y) );
				TempLazerStream.transform.localScale = new Vector3 ( TempLazerStream.transform.localScale.x*LazerDistance, 1.0f, 1.0f );

				// Rotate laser beam top aim at target
				TempLazerStream.transform.Rotate( new Vector3( 0, 0, Arg_GunAngleFloat) );

				// If colliding object is a player
				if ( hit.collider.tag == "Player" ) {

					// Turns off friendly (ignores if the other object is on the same team)
					if ( hit.collider.GetComponent<CharacterManager>().TeamName != TeamName ) {
						hit.collider.GetComponent<CharacterManager> ().TakeDamage ( GunDamage );
					}
				}
			}

			// Play laser gun noise
			LaserSoundRef.Play();

			// Reload the gun if player tries to shoot and has no bullets
		} else if ( NumberOfBullets <= 0 && !PlayerReloading ) {
			ReloadGun ();
		}
	}

	void ThrowGrenade( Vector2 Arg_GrenadeAngle ) {
		// Player can't throw a grenade while reloading
		if ( CanThrowGrenade && GrenadeCooldownOff && !PlayerReloading ) {

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

		// Player can't reload gun while holding the bomb
		if ( !PlayerCarryingBomb ) {
			PlayerReloading = true;
			PlayerReloadingTimer = 0.0f;
		}
	}

	public void TakeDamage( float Arg_DamageAmount ) {

		if (CurrentHealth > 0) {
			CurrentHealth -= Arg_DamageAmount;

			if ( CurrentHealth <= 0 ) {
				CurrentHealth = 0;
				KillPlayer ();

				// Play death sound
				DeathSoundRef.Play();
			}
		}
	}

	public void KillPlayer () {

		// Drop the bomb if they have it
		if ( PlayerCarryingBomb ) {
			DropBomb ();
		}

		// Spawn a ghost
		Instantiate(GhostRef, transform.position, Quaternion.identity);

		PlayerRespawnTimer = 0.0f;
		PlayerAlive = false;
		transform.position = new Vector3( 4000.0f, 4000.0f, 0 );
	}

	void RespawnPlayer () {
		PlayerAlive = true;
		transform.position = SpawnPoint.transform.position;
		CurrentHealth = MaxHealth;
		NumberOfBullets = TotalNumberOfBullets;
	}

	void PickUpBomb ( GameObject Arg_TheBomb ) {

		// Indicate the player is carrying the bomb
		PlayerCarryingBomb = true;

		// Disable the collider and rigidbody
		Arg_TheBomb.GetComponent<Rigidbody2D>().isKinematic = true;
		Arg_TheBomb.GetComponent<BoxCollider2D>().enabled = false;

		// Set this transform as the parent of the bomb
		Arg_TheBomb.transform.SetParent( this.transform );

		// Put the bomb into a position
		Arg_TheBomb.transform.localPosition = Vector3.zero;

		// Hide the gun object
		transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

		// Make hand bomb appear
		BombRef.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
		BombRef.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;

		// Set the bomb to have no rotation
		BombRef.transform.rotation = Quaternion.identity;
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

		// Make normal bomb appear
		BombRef.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
		BombRef.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
	}

	void ThrowBomb( Vector2 Arg_BombAngle ) {

		// Indicate player is no longer holding the bomb
		PlayerCarryingBomb = false;

		// Indicate the player cannot throw grenades now either
		CanThrowGrenade = false;

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
		BombRef.GetComponent<BoxCollider2D>().enabled = true;

		// Add force to the bomb
		BombRef.GetComponent<Rigidbody2D>().AddForce( BombThrowForce * Arg_BombAngle, ForceMode2D.Impulse );

		// Make the gun re-appear
		transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

		// Make the normal bomb appear
		BombRef.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
		BombRef.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
	}

	void DropBomb( ) {

		// Indicate player is no longer holding the bomb
		PlayerCarryingBomb = false;

		// Remove this object as the bombs parent
		BombRef.transform.parent = null;

		// Enable Rigidbody and collider
		BombRef.GetComponent<Rigidbody2D>().isKinematic = false;
		BombRef.GetComponent<BoxCollider2D>().enabled = true;

		// Make the gun re-appear
		transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

		// Make the normal bomb appear
		BombRef.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
		BombRef.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
	}

	public void GrenadeDisableMovement (  ) {
		GrenadeHit_MovementDisable = true;
		GrenadeHit_DisableTimer = 0.0f;
		gameObject.GetComponent<Rigidbody2D> ().drag = 0.0f;
	}

	public void AddHealth() {
		CurrentHealth += AddhealthAmount;
		if ( CurrentHealth > 100 ) {
			CurrentHealth = 100;
		}
	}
}
