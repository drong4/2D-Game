using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSwordsmanController : MonoBehaviour {

	[Header("Movement")]
	public float moveSpeed;
	private Animator anim;
	private Rigidbody2D myRigidBody;

	[Space]

	[Header("Tracking")]
	//variables for tracking
	private float euclideanDist;
	private Vector2 targetPosition;
	private Vector2 ourPosition;
	public float attackRange;
	public float dashAttackRange;
	private float xDist;
	private float yDist;

	[Space]

	[Header("Attacking")]
	//variables for attacking animation
	public float probOfAttack;//probability of attacking when in range
	private bool isAttacking;

	public float dashAttackCooldown;//time until can dashAttack again
	private float dashAttackCooldownCounter;
	private bool canDashAttack;

	[Space]

	[Header("Animation")]
	//variables for animation
	private bool isMoving;
	public Vector2 lastMove;
	private bool isSheathed;

	public float toggleSheatheCooldown;//time until can sheathe/unsheathe
	private float toggleSheatheCooldownCounter;
	private bool canToggleSheathe;
	private bool isTogglingSheathe;

	[Space]

	[Header("Audio")]
	//variables for audio
	public AudioClip dashSound;
	public AudioClip dashAttackSound;
	public AudioClip sheatheSound;
	public AudioClip unsheatheSound;
	public AudioClip swordSwingSound;

	AudioSource audiosource;

	[Space]

	[Header("Knockback")]
	//variables for knockback
	public bool canFlinch;
	public float flinchTimeWindow;//after this amount of time, we won't flinch for a bit
	private float flinchTimeCounter;
	private bool wasFlinched;
	public float superArmorWindow;//amount of time we can superarmor through flinches

	[Space]

	private EnemyInformation enemyInfo;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		myRigidBody = GetComponent<Rigidbody2D> ();
		audiosource = GetComponent<AudioSource> ();
		enemyInfo = GetComponent<EnemyInformation> ();

		isMoving = false;
		isAttacking = false;
		canFlinch = true;//initially can flinch
		wasFlinched = false;

		isSheathed = true;//start off with sword sheathed
		isTogglingSheathe = false;
		canToggleSheathe = false;//start off not being able to toggle
		toggleSheatheCooldownCounter = toggleSheatheCooldown;

		canDashAttack = false;//start off not being able to dash attack
		dashAttackCooldownCounter = dashAttackCooldown;
	}
	
	// Update is called once per frame
	void Update () {
		//Handle cooldowns
		if (!canDashAttack) {
			dashAttackCooldownCounter -= Time.deltaTime;
			if (dashAttackCooldownCounter <= 0f) {
				canDashAttack = true;//cooldown is over!
			}
		}
		if (!canToggleSheathe) {
			toggleSheatheCooldownCounter -= Time.deltaTime;
			if (toggleSheatheCooldownCounter <= 0f) {
				canToggleSheathe = true;//cooldown is over!
			}
		}

//		if (wasFlinched) {
//			flinchTimeCounter -= Time.deltaTime;
//			if (flinchTimeCounter <= 0f) {
//				wasFlinched = false;
//				canFlinch = false;//won't flinch anymore
//				flinchTimeCounter = superArmorWindow;
//				//stop flinching
//				anim.SetBool("Is_Flinched", false);
//			}
//		}
//		if (!canFlinch) {
//			//If currently can't flinch...
//			flinchTimeCounter -= Time.deltaTime;
//			if (flinchTimeCounter <= 0f) {
//				canFlinch = true;//can flinch again
//			}
//		}

//		//Handle knockback
//		if (enemyInfo.getIsKnockedback() && canFlinch) {
////			if (canFlinch) {
//			if (!anim.GetBool ("Is_Flinched")) {
//				//Start the flinching animation
//				turnOffAllAnimations ();
//				isMoving = false;
//				isAttacking = false;
//				isTogglingSheathe = false;
//				deactivateInvulnerability ();
//				
//				if (!wasFlinched) {
//					flinchTimeCounter = flinchTimeWindow;
//					wasFlinched = true;
//				}
//
//				anim.SetBool ("Is_Flinched", true);
//			} 
//			else {
//				float normTime = anim.GetCurrentAnimatorStateInfo (0).normalizedTime;//1.49 = looped animation once, 49% second time
//				if (normTime > 1f && (anim.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.Sheathed Flinch")
//				    || (anim.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.Unsheathed Flinch")))) {
//					//Finished the flinching animation
//					//set anim variable to false
//					anim.SetBool ("Is_Flinched", false);
//					enemyInfo.setIsKnockedBack (false);
//				} 
//			}
////			} 
////			else {
////				//can't flinch anymore
////				enemyInfo.setIsKnockedBack(false);
////			}
//		}
		//Handle toggling sheathe
		if (isTogglingSheathe) {
			if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 1f && 
				(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Unsheathe") || 
					anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Sheathe"))) {
				//done toggling
				anim.SetBool ("Sheathe", false);
				anim.SetBool ("Unsheathe", false); 

				isTogglingSheathe = false;
			}
		}
		//Handle attacks
		else if (isAttacking) {
			if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 1f &&
				(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Dash Attack") || 
					anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Unsheathed Attack"))) {
				//if we finished one loop of this animation..
				anim.SetBool("Unsheathed_Attack", false);
				anim.SetBool ("Dash_Attack", false);

				isAttacking = false;
			}
		}
		//Handle movement
		else{
			if (this.enemyInfo.isAlerted && this.enemyInfo.trackingTarget != null) {
				//calculate dist from trackingTarget
				targetPosition = this.enemyInfo.trackingTarget.transform.position;
				ourPosition = transform.position;

				xDist = targetPosition.x - ourPosition.x;//(-) if we need to go left, (+) if we need to go right

				float dir = xDist / Mathf.Abs (xDist);//-1 go left, +1 go right

				//Toggle Sheathe if possible
				if (canToggleSheathe) {
					//stop moving
					myRigidBody.velocity = Vector2.zero;
					isMoving = false;

					if (isSheathed) {
						anim.SetBool ("Unsheathe", true);
					} 
					else {
						anim.SetBool ("Sheathe", true);
					}
					isSheathed = !isSheathed;
					toggleSheatheCooldownCounter = toggleSheatheCooldown;
					isTogglingSheathe = true;
					canToggleSheathe = false;
					return;
				}


				bool canAttack = Random.Range (0f, 100f) < probOfAttack;

				if ((Mathf.Abs (xDist) <= dashAttackRange || Mathf.Abs (xDist) <= attackRange) && canAttack) {

					//if we are close enough to unsheathed attack...
					if(Mathf.Abs(xDist) <= attackRange && !isSheathed) {
						anim.SetBool ("Unsheathed_Attack", true);

						//stop moving
						myRigidBody.velocity = Vector2.zero;
						isMoving = false;
						isAttacking = true;
					}

					//else we can dash attack...
					else {
						if (canDashAttack && isSheathed) {
							anim.SetBool ("Dash_Attack", true);
							canDashAttack = false;
							dashAttackCooldownCounter = dashAttackCooldown;

							//stop moving
							myRigidBody.velocity = Vector2.zero;
							isMoving = false;
							isAttacking = true;
						} 
						else {
							/*If the only thing stopping the dash attack is the cooldown, 
							 * we don't want to be locked in sheathed mode for a long time by the toggle cooldown*/
							if (toggleSheatheCooldownCounter > 2f && isSheathed) {
								toggleSheatheCooldownCounter = 2f;
							}
						}
					}

					//turn around if necessary
					Vector3 newScale = transform.localScale;
					if (xDist < 0)
						newScale.x = -Mathf.Abs (newScale.x);
					else {
						newScale.x = Mathf.Abs (newScale.x);
					}
					transform.localScale = newScale;
				}
				//else, set velocity to move towards trackingTarget's position
				else {
					if (Mathf.Abs (xDist) > 0.25f) {
						//Keep moving towards target until 0.25 dist away
						myRigidBody.velocity = new Vector2 (dir * moveSpeed, myRigidBody.velocity.y);
						isMoving = true;
					}

					//turn around if necessary
					Vector3 newScale = transform.localScale;
					if (xDist < 0)
						newScale.x = -Mathf.Abs (newScale.x);
					else {
						newScale.x = Mathf.Abs (newScale.x);
					}
					transform.localScale = newScale;
				}
			}
		}

		Animation ();
	}

	//Handles animations
	void Animation()
	{
		anim.SetFloat("Move_X", xDist);
		anim.SetBool("Is_Moving", isMoving);
		anim.SetBool ("Is_Sheathed", isSheathed);
	}
	//Helper function to turn off all boolean parameters in animator
	void turnOffAllAnimations(){
		foreach(AnimatorControllerParameter parameter in anim.parameters) {            
			anim.SetBool(parameter.name, false);            
		}
	}
	//Teleports us to the target during the dash attack
	void dashTeleport(){
		targetPosition = this.enemyInfo.trackingTarget.transform.position;
		ourPosition = this.transform.position;
		float ourNormalizedLocalx = this.transform.localScale.x / Mathf.Abs (this.transform.localScale.x);
		float dir = 0f;
		if (ourPosition.x <= targetPosition.x) {
			//we are to left of target
			if (ourNormalizedLocalx == -1f) {
				//if we are facing left, we will dash left, so make sure we appear to the right of target
				dir = 1f;
			} 
			else {
				dir = -1f;
			}
		} 
		else {
			//we are to right of target
			if (ourNormalizedLocalx == -1f) {
				dir = 1f;
			} 
			else {
				//if we are facing right, we will dash right, so make sure we appear to the left of target
				dir = -1f;
			}
		}
		this.transform.position = new Vector3 (targetPosition.x + (dir * 2f), ourPosition.y, this.transform.position.z);
	}
	/*Move forward a little.
	dist = distance to step forward*/
	void stepForward(float dist){
		myRigidBody.velocity = new Vector2 (dist * this.transform.localScale.x, myRigidBody.velocity.y);
	}
	void stopVelocity(){
		myRigidBody.velocity = Vector2.zero;
	}
	/*Sets invulnerability status*/
	void activateInvulnerability(){
		enemyInfo.setIsInvulnerable (true);
	}
	void deactivateInvulnerability(){
		enemyInfo.setIsInvulnerable (false);
	}

	//Handles audio
	void playDashSound(){
		audiosource.pitch = 1f;
		audiosource.PlayOneShot (dashSound, 0.5f);
	}
	void playDashAttackSound(){
		audiosource.pitch = 1f;
		audiosource.PlayOneShot (dashAttackSound, 0.5f);
	}
	void playUnsheathedAttackSound(){
		audiosource.pitch = 1f;
		audiosource.PlayOneShot (swordSwingSound, 0.5f);
	}
	void playSheatheSound(){
		audiosource.pitch = 1f;
		audiosource.PlayOneShot (sheatheSound);
	}
	void playUnsheatheSound(){
		audiosource.pitch = 1.15f;
		audiosource.PlayOneShot (unsheatheSound, 0.5f);
	}
}
