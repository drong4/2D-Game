using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleGreatSwordEnemyController : MonoBehaviour {

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
	private float xDist;
	private float yDist;

	[Space]

	[Header("Attacking")]
	//variables for attacking animation
	public float probOfAttack;//probability of attacking when in range
	private bool isAttacking;

	[Space]

	[Header("Countering")]
	private bool canCounter;
	public float counterCoolDownTime;
	private float counterCoolDownCounter;

	[Space]

	[Header("Animation")]
	//variables for animation
	private bool isMoving;
	public Vector2 lastMove;

	[Space]

	[Header("Audio")]
	//variables for audio
	public AudioClip attack_1_1_Sound;
	public AudioClip attack_1_2_Sound;
	public AudioClip slamAttackSound;
	public AudioClip counterStanceSound;
	public AudioClip counterAttackSound;

	AudioSource audiosource;

	[Space]

	[Header("Knockback")]
	//variables for knockback
//	public bool canFlinch;
//	public float flinchTimeWindow;//after this amount of time, we won't flinch for a bit
//	private float flinchTimeCounter;
//	private bool wasFlinched;
//	public float superArmorWindow;//amount of time we can superarmor through flinches

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
		canCounter = true;
//		canFlinch = true;//initially can flinch
//		wasFlinched = false;
	}
	
	// Update is called once per frame
	void Update () {
		isMoving = false;

		if (!canCounter) {
			if (counterCoolDownCounter <= 0f) {
				canCounter = true;//cooldown over
			} 
			else {
				counterCoolDownCounter -= Time.deltaTime;
			}
		}

		if (anim.GetBool("Counter_Stance")) {
			float normTime = anim.GetCurrentAnimatorStateInfo (0).normalizedTime;
			if (normTime > 1f && anim.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.Counter_Stance")) {
				//If we finished the counter stance animation...
				enemyInfo.setIsCounteringFalse();
				anim.SetBool ("Counter_Stance", false);
			}
			else{
				//If we are hit at any point during the counter, switch to counter attack
				if (enemyInfo.getWillCounterAttack ()) {
					//Change direction if we need to
					targetPosition = this.enemyInfo.trackingTarget.transform.position;
					ourPosition = transform.position;
					xDist = targetPosition.x - ourPosition.x;//(-) if we need to go left, (+) if we need to go right
					Vector3 newScale = transform.localScale;
					if (xDist < 0)
						newScale.x = -Mathf.Abs (newScale.x);
					else {
						newScale.x = Mathf.Abs (newScale.x);
					}
					transform.localScale = newScale;

					//Perform counter attack
					isAttacking = true;
					anim.SetBool ("Counter_Attack", true);
					anim.SetBool ("Counter_Stance", false);
					enemyInfo.setIsCounteringFalse();//we're not countering anymore
					enemyInfo.setWillCounterAttack(false);
				}
			}

		}
		else if (isAttacking) {
			float normTime = anim.GetCurrentAnimatorStateInfo (0).normalizedTime;
			if(normTime > 1f && (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack_1") || 
				anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Slam_Attack") || 
				anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Counter_Attack"))){
				//We finished the animation
				isAttacking = false;
				anim.SetBool ("Attack_1", false);
				anim.SetBool ("Slam_Attack", false);
				anim.SetBool ("Counter_Attack", false);
			}
		} 
		else {
			if (this.enemyInfo.isAlerted &&
			    this.enemyInfo.trackingTarget != null) {
				//calculate dist from trackingTarget
				targetPosition = this.enemyInfo.trackingTarget.transform.position;
				ourPosition = transform.position;

				xDist = targetPosition.x - ourPosition.x;//(-) if we need to go left, (+) if we need to go right
				//yDist = targetPosition.y - ourPosition.y;

				float dir = xDist / Mathf.Abs (xDist);//-1 go left, +1 go right

				bool canAttack = Random.Range (0f, 100f) < probOfAttack;


				//if we are close enough to attack...
				if (Mathf.Abs (xDist) <= attackRange) {
					//check if we should attack this frame
					if (canAttack) {
						isAttacking = true;

						//decide which attack to do
						int attackNum = Random.Range (1, 4);//return 1-3
						if (attackNum == 1) {
							//Do attack 1
							anim.SetBool ("Attack_1", true);
						} else if (attackNum == 2) {
							//Do attack 2
							anim.SetBool ("Slam_Attack", true);
						} else {
							isAttacking = false;//we're not attacking
							//Check if we can counter
							if(canCounter){
								//Do counter
								anim.SetBool ("Counter_Stance", true);
	//							enemyInfo.setIsCountering (true);
								canCounter = false;
								counterCoolDownCounter = counterCoolDownTime;
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
				} 
				//else, set velocity to move towards trackingTarget's position
				else {
					//scale xDist, yDist by moveSpeed/euclideanDist to get a net force of moveSpeed in the right direction
					myRigidBody.velocity = new Vector2 (dir * moveSpeed, myRigidBody.velocity.y);

					//turn around if necessary
					Vector3 newScale = transform.localScale;
					if (xDist < 0)
						newScale.x = -Mathf.Abs (newScale.x);
					else {
						newScale.x = Mathf.Abs (newScale.x);
					}
					transform.localScale = newScale;

					isMoving = true;
				}
			}
		}
		anim.SetBool ("Walk", isMoving);
	}

	void playAttack_1_1Sound()
	{
		audiosource.pitch = 1.25f;
		audiosource.PlayOneShot (attack_1_1_Sound, 0.5f);
	}
	void playAttack_1_2Sound()
	{
		audiosource.pitch = 1.4f;
		audiosource.PlayOneShot (attack_1_2_Sound, 0.5f);
	}
	void playSlamAttackSound()
	{
		audiosource.pitch = 1.5f;//reset pitch to original
		audiosource.PlayOneShot (slamAttackSound, 0.25f);
	}
	void playCounterAttackSound()
	{
		audiosource.pitch = 1f;//reset pitch to original
		audiosource.PlayOneShot (counterAttackSound, 1f);
	}
	void playerCounterStanceSound()
	{
		audiosource.pitch = 1f;//reset pitch to original
		audiosource.PlayOneShot (counterStanceSound, 0.5f);
	}

	/*Move forward a little.
	dist = distance to step forward*/
	void stepForward(float dist){
		myRigidBody.velocity = new Vector2 (dist * this.transform.localScale.x, myRigidBody.velocity.y);
	}
}
