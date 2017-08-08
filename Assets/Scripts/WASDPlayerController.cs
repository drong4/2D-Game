using UnityEngine;
using System.Collections;

/* WASD 
 * J-attack
 * K-roll
 * L-shoot
 * Space-jump
 * S+K - counter*/
public class WASDPlayerController : MonoBehaviour {
    public float moveSpeed;
	public bool canMove;
	public void setCanMove(bool val){
		canMove = val;
	}
		
	[Header("Jumping")] 
    private bool isGrounded;
	//Variables for jumping
	public float jumpForce;
	private bool isJumping;
	float groundCheckRadius = 0.01f;
	public LayerMask groundLayer;
	public Transform groundCheck;
	//for double jump
	private bool canDoubleJump;

	[Space]

    private Animator anim;
    private Rigidbody2D myRigidBody;

    private bool playerIsMoving;
    private Vector2 lastMove;

    private static bool playerExists;



	[Header("Attack")]
    //Variables for attacking
    private static bool isAttacking;

	[Space]

	[Header("Projectiles")]
	//Variables for firing projectiles
	public Transform firingPoint;
	public GameObject projectile;
	public float firingDelay;//Delay between shots
	private float firingDelayCounter;
	private bool canFire;

	[Space]

	[Header("Rolling Variables")]
	//Variables for rolling
	public float rollSpeed;
	private static  bool isRolling;
	public float rollTime;
	private float rollTimeCounter;
	public float rollCoolDown;
	private float rollCoolDownCounter;
	private bool canRoll;

	private float origGravityScale;//to restore after rolling in air

	[Space]

	[Header("Audio")]
	//variables for audio clips
	public AudioClip attack1Sound;
	public AudioClip attack2Sound;
	public AudioClip attack3_1Sound;
	public AudioClip attack3_2Sound;
	public AudioClip dashSound;
	public AudioClip counterAttackSound;
	public AudioClip counterStanceSound;
	public AudioClip aerialAttackSound;
	public AudioClip firingSound;
	private AudioSource audiosource;

	[Space]

	[Header("Counter")]
	//variables for countering
	public float counterTime;//time stuck in counter animation
	private float counterTimeCounter;
	private float counterTimeCounterMax;
	public bool validCounter = false;
	private static bool isCountering;
	public bool getValidCounterStatus(){
		return validCounter;
	}
	public float counterAttackTime;
	public float counterTimeWindow;//time window where we can do a successful counter



	[Space]


	[Header("Knockback")]
	//variables for knockback animation
	private bool isKnockedBack;//lets the objects that hit us alter this

	public void	setKnockBack(bool val){
		/*make sure all the other states are set to false
		(in case we got hit in the middle of something)*/
		turnOffAllAnimations ();

		isAttacking = false;
		isRolling = false;
		isCountering = false;

		//lets other classes set isKnockedback
		isKnockedBack = val;
		anim.SetBool ("Is_Flinched", true);
	}

	[Space]

	[Header("I - Frames")]
	public bool isInvulnerable;//let the thing attacking us check this value
	public float rollIFrames;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();
		audiosource = GetComponent<AudioSource> ();

//        //Deals with duplicates of the player when any scene loading happens
//         if (!playerExists)
//         {
//             playerExists = true;
//             DontDestroyOnLoad(transform.gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }

        isGrounded = true;
		isJumping = false;
		canDoubleJump = true;
		isAttacking = false;
		isKnockedBack = false;
		isInvulnerable = false;
		canFire = true;
		canRoll = true;
		canMove = true;

		//start off facing right
		lastMove = new Vector2(1f, 0f);

		origGravityScale = myRigidBody.gravityScale;
    }
		
	
	// FixedUpdate is called once after certain amount of time, while Update is called once per frame (variable time)
	void Update () {
        playerIsMoving = false;

		//Check if we can fire
		if (!canFire) {
			//if we can't, then count down
			if (firingDelayCounter < 0) {
				canFire = true;
			} 
			else {
				firingDelayCounter -= Time.deltaTime;
			}
		}


		//Check if we're in contact with ground.
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
		if (isJumping && isGrounded) {
			//we just hit the ground again
			canDoubleJump = true;
		}
		isJumping = !isGrounded;

		if (!canRoll) {
			if (rollCoolDownCounter <= 0f) {
				//if roll cooldown is over
				canRoll = true;
			} 
			else {
				//update the cooldown
				rollCoolDownCounter -= Time.deltaTime;
			}
		}


		if (isKnockedBack) {
			float normTime = anim.GetCurrentAnimatorStateInfo (0).normalizedTime;//1.49 = looped animation once, 49% second time
			if (normTime > 1f && anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Flinch")) {
				//Finished the flinching animation
				//set anim variable to false
				anim.SetBool ("Is_Flinched", false);
				isKnockedBack = false;
			} 
		}
		//Countering
		else if (isCountering) {
			float timePassed = counterTimeCounterMax - counterTimeCounter;
			validCounter = timePassed <= counterTimeWindow;
			if (counterTimeCounter > 0f) {
				counterTimeCounter -= Time.deltaTime;


				if (GetComponent<HealthManager> ().getCounterSuccessStatus() && timePassed <= counterTimeWindow) {
					//successful counter!
					anim.SetBool("Successful_Counter", true);//play counter attack animation
					counterTimeCounterMax = counterAttackTime;
					counterTimeCounter = counterAttackTime;//reset counter to counterAttackTime

					//reset HealthManager's counterSuccess to false
					GetComponent<HealthManager> ().setCounterSuccessStatus(false);
				}
			} 
			else {
				//done countering
				isCountering = false;
				validCounter = false;
				anim.SetBool ("Is_Countering", false);
				anim.SetBool ("Successful_Counter", false);
			}
		}
		//Rolling
		else if (isRolling) {
			float timePassed = rollTime - rollTimeCounter;
			isInvulnerable = timePassed <= rollIFrames;

			if (rollTimeCounter > 0f) {
				rollTimeCounter -= Time.deltaTime;

				if (isJumping) 
					transform.Translate (new Vector3 (lastMove.x * (rollSpeed * 1.5f) * Time.deltaTime, 0f, 0f));
				else
					transform.Translate (new Vector3 (lastMove.x * rollSpeed * Time.deltaTime, 0f, 0f));
			}
			else
			{
				isRolling = false;
				isInvulnerable = false; //make sure we don't have i-frames anymore
				anim.SetBool("Is_Rolling", false);

				myRigidBody.gravityScale = origGravityScale;
			}
		}
        //Attack lag
		else if(isAttacking){
			if (anim.GetBool ("Aerial_Attack") && isJumping) {
				Movement ();//allow for movement if aerial attacking, AND off the ground
			}

			float normTime = anim.GetCurrentAnimatorStateInfo (0).normalizedTime;//1.49 = looped animation once, 49% second time
			if (normTime > 1f && (anim.GetCurrentAnimatorStateInfo(0).IsName("Ground Attack.attack_1")
				|| anim.GetCurrentAnimatorStateInfo(0).IsName("Ground Attack.attack_2")
				|| anim.GetCurrentAnimatorStateInfo(0).IsName("Ground Attack.attack_3")
				|| anim.GetCurrentAnimatorStateInfo(0).IsName("Jump.Aerial Attack"))) {
				//if we finished the current animation...
				anim.SetBool ("Attack_1", false);
				anim.SetBool ("Attack_2", false);
				anim.SetBool ("Attack_3", false);
				anim.SetBool ("Aerial_Attack", false);

				isAttacking = false;
			} 
			else {
				//still in middle of animation
				if (anim.GetBool("Attack_1") && !anim.GetBool("Attack_2") && 
					(anim.GetCurrentAnimatorStateInfo(0).IsName("Ground Attack.attack_1") && normTime > 0.4f)) {
					//last 60% of animation is window to continue to Attack_2
					if (Input.GetKeyDown (KeyCode.J)) {
						anim.SetBool ("Attack_2", true);//go do Attack_2
					}
				}
				else if (anim.GetBool("Attack_2") && !anim.GetBool("Attack_3") && 
						(anim.GetCurrentAnimatorStateInfo(0).IsName("Ground Attack.attack_2") && normTime > 0.5f)) {
					//last 60% of animation is window to continue to Attack_3
					if (Input.GetKeyDown (KeyCode.J)) {
						anim.SetBool ("Attack_3", true);//go do Attack_3
					}
				}
			}
         }
		//If we aren't attacking or rolling or getting knocked back, we can move
		else
			Movement();

        Animation();
    }

    //Handles all of the characters movements, user inputs
    void Movement()
    {
		if (!canMove) {
			return;
		}

        //Up
        if (Input.GetKey(KeyCode.W))
        {

        }
        //Left
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0f, 0f));
            playerIsMoving = true;

			//turn around if facing right
			Vector3 newScale = transform.localScale;
			if (newScale.x > 0) {
				newScale.x = -Mathf.Abs(newScale.x);
				transform.localScale = newScale;
			}

            lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
        
        }
        //Down
        if (Input.GetKey(KeyCode.S))
        {
			if (Input.GetKey (KeyCode.K)) {
				//perform counter if Down + Roll
				isCountering = true;
				anim.SetBool ("Is_Countering", true);
				counterTimeCounterMax = counterTime;
				counterTimeCounter = counterTime;
				return;//don't do anything else
			}
        }
        //Right
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0f, 0f));
            playerIsMoving = true;

			//turn around if facing leftP
			Vector3 newScale = transform.localScale;
			if (newScale.x < 0) {
				newScale.x = Mathf.Abs(newScale.x);
				transform.localScale = newScale;
			}

            lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
        }

        //Attack(set as J)
		if (Input.GetKeyDown(KeyCode.J) && !isAttacking)//GetKeyDown() only takes in the first instance the key is pressed
        {
			if (isJumping) {
				//do aerial attack
				anim.SetBool ("Aerial_Attack", true);
			} 
			else {
				//do grounded attack
				anim.SetBool ("Attack_1", true);
			}

			isAttacking = true;

			return;
        }

		//Shoot
		if (Input.GetKeyDown (KeyCode.L) && canFire) {
			var arrow = Instantiate (projectile, firingPoint.position, firingPoint.rotation);
			//orient the projectile towards the direction player is facing
			arrow.transform.localScale = transform.localScale;

			playFiringSound ();

			canFire = false;
			firingDelayCounter = firingDelay;

			return;
		}

		//Roll(set as K)
		if (Input.GetKeyDown (KeyCode.K) && !anim.GetBool ("Aerial_Attack") && canRoll) {
			rollTimeCounter = rollTime;
			isRolling = true;
			anim.SetBool ("Is_Rolling", true);

			//Set it so we are invincible
			isInvulnerable = true;

			//If we're in the air, cancel all velocity
			if (isJumping) {
				myRigidBody.velocity = Vector2.zero;
				myRigidBody.gravityScale = 0;
			}

			playDashSound ();
			//Start cool down timer
			canRoll = false;
			rollCoolDownCounter = rollCoolDown;

			return;
		}

		//Jump(set as Space)
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (isGrounded) {
				//we're on the ground, do initial jump
				isGrounded = false;
				isJumping = true;
				playerIsMoving = true;
				myRigidBody.AddForce (new Vector2 (myRigidBody.velocity.x, jumpForce));
			} else {
				//check if double jump is available
				if (canDoubleJump) {
					//stop movement...
					myRigidBody.velocity = Vector2.zero;
					//...then add the jump force, but half
					myRigidBody.AddForce (new Vector2 (myRigidBody.velocity.x, jumpForce/2));
				}
				canDoubleJump = false;//we used our double jump
			}
			return;
		}
    }

    //Handles animations
    void Animation()
    {
        anim.SetFloat("Move_X", Input.GetAxisRaw("Horizontal"));
		anim.SetBool("Player_Is_Moving", playerIsMoving);
        anim.SetFloat("Last_Move_X", lastMove.x);
		anim.SetBool ("Is_Jumping", isJumping);
		anim.SetFloat ("Vertical_Speed", myRigidBody.velocity.y);
    }
		
	void turnOffAllAnimations(){
		foreach(AnimatorControllerParameter parameter in anim.parameters) {            
			anim.SetBool(parameter.name, false);            
		}
	}

	/*Move forward a little.
	dist = distance to step forward*/
	void stepForward(float dist){
		myRigidBody.velocity = new Vector2 (dist * this.transform.localScale.x, myRigidBody.velocity.y);
	}

	//Audio functions- Called by the Animation window, event
	//				-Can play sound on specific frame in animation
	void playAttack1Sound()
	{
		audiosource.pitch = 1f;//reset pitch to original
		audiosource.PlayOneShot (attack1Sound, 0.5f);
	}
	void playAttack2Sound()
	{
		audiosource.pitch = 1f;//reset pitch to original
		audiosource.PlayOneShot (attack2Sound, 0.5f);
	}
	void playAttack3_1Sound()
	{
		audiosource.pitch = 0.5f;
		audiosource.PlayOneShot (attack3_1Sound, 0.5f);
	}
	void playAttack3_2Sound()
	{
		audiosource.pitch = 0.75f;
		audiosource.PlayOneShot (attack3_2Sound, 0.25f);
	}
	void playDashSound(){
		audiosource.pitch = 1f;//reset pitch to original
		audiosource.PlayOneShot (dashSound, 0.5f);
	}
	void playCounterAttackSound(){
		audiosource.pitch = 1f;
		audiosource.PlayOneShot (counterAttackSound, 8f);

	}
	void playCounterStanceSound(){
		audiosource.pitch = 1f;
		audiosource.PlayOneShot (counterStanceSound, 1f);
	}
	void playAerialAttackSound(){
		audiosource.pitch = 0.75f;
		audiosource.PlayOneShot (aerialAttackSound);
	}
	void playFiringSound(){
		audiosource.pitch = 0.75f;
		audiosource.PlayOneShot (firingSound, 0.3f);
	}
}
