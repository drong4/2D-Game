using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantEnemyController : MonoBehaviour {

	[Header("Movement")]
	public float moveSpeed;
	private Animator anim;
	private Rigidbody2D myRigidBody;

	//variables for wakeup mechanic
	private bool isWakingUp;
	private bool isAwake;

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

	[Header("Animation")]
	//variables for animation
	private bool isMoving;
	public Vector2 lastMove;

	[Space]

	[Header("Audio")]
	//variables for audio
	public AudioClip attack1Sound;
	public AudioClip attackStompSound;
	AudioSource audiosource;

	[Space]

	private EnemyInformation enemyInfo;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		myRigidBody = GetComponent<Rigidbody2D> ();
		audiosource = GetComponent<AudioSource> ();
		enemyInfo = GetComponent<EnemyInformation> ();

		isWakingUp = false;
		isAwake = false;
		isAttacking = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (isWakingUp) {
			if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 1f) {
				//if we finished one loop of this animation...
				anim.SetBool("Is_Waking_Up", false);
				isWakingUp = false;
				isAwake = true;
			}
		}
		else if (isAttacking) {
			if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 1f) {
				//if we finished one loop of this animation..
				anim.SetBool("Attack_1", false);
				anim.SetBool ("Attack_Stomp", false);
				anim.SetBool ("Attack_Multi_Stomp", false);
				anim.SetBool ("Attack_Multi_1", false);

				isAttacking = false;
			}
		}
		else{
			if (this.enemyInfo.isAlerted && 
				this.enemyInfo.trackingTarget != null) {
				if (!isAwake) {
					//We received alert, but we're still asleep... Wake up!
					isWakingUp = true;
					anim.SetBool ("Is_Waking_Up", true);
				} 
				else {
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
							//stop moving
							myRigidBody.velocity = Vector2.zero;

							float percentHP = (float)(this.GetComponent<HealthManager> ().currentHealth) /
								(float)(this.GetComponent<HealthManager> ().maxHealth);
							
							//Choose attack
							if (Mathf.Abs (xDist) <= 0.5f) {
								//if target is super close, do the stomp
								if (percentHP >= 0.5) { 
									anim.SetBool ("Attack_Stomp", true);
								} 
								else {
									//Do multiple stomps if less than 50% hp
									anim.SetBool ("Attack_Multi_Stomp", true);
								}
							} 
							else{
								if (percentHP >= 0.5) { 
									anim.SetBool ("Attack_1", true);
								} 
								else {
									//Do multiple hits if less than 50% hp
									anim.SetBool ("Attack_Multi_1", true);
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
		}

		Animation ();
	}

	//Handles animations
	void Animation()
	{
		anim.SetFloat("Move_X", xDist);
		anim.SetBool("Is_Moving", isMoving);
	}

	//Handles audio
	void playAttack1Sound(){
		audiosource.PlayOneShot (attack1Sound, 0.5f);
	}
	void playAttackStompSound(){
		audiosource.PlayOneShot (attackStompSound, 0.5f);
	}
}
