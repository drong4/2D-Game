﻿using UnityEngine;
using System.Collections;

/*Follows trackingTarget relentlessly, attacking when in range*/
public class EnemyController : MonoBehaviour {

	public float moveSpeed;
	private Animator anim;
	private Rigidbody2D myRigidBody;

	//variables for tracking
	private float euclideanDist;
	private Vector2 targetPosition;
	private Vector2 ourPosition;
	public float attackRange;
	private float xDist;
	private float yDist;
	//variables for attacking animation
	public float probOfAttack;//probability of attacking when in range
	private bool isAttacking;
	public float minAttackDelay;
	public float maxAttackDelay;
	private float attackTimeCounter;

	//variables for checking if on ground
	private bool isGrounded;
	float groundCheckRadius = 0.2f;
	public LayerMask groundLayer;
	public Transform groundCheck;

	[Space]

	private EnemyInformation enemyInfo;

	//variables for animation
	private bool isMoving;
	public Vector2 lastMove;

	//variables for audio
	public AudioClip attack1Sound;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		myRigidBody = GetComponent<Rigidbody2D>();

		isGrounded = false;
		isAttacking = false;
		enemyInfo = GetComponent<EnemyInformation> ();
	}
	
	// Update is called once per frame
	void Update () {
		isMoving = false;

		//Check if we're in contact with ground.
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

		if (enemyInfo.getIsKnockedback()) {
			if (!anim.GetBool ("Is_Flinched")) {
				anim.SetBool ("Is_Flinched", true);
			} 
			else {
				float normTime = anim.GetCurrentAnimatorStateInfo (0).normalizedTime;//1.49 = looped animation once, 49% second time
				if (normTime > 1f && anim.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.Flinch")) {
					//Finished the flinching animation
					//set anim variable to false
					anim.SetBool ("Is_Flinched", false);
					enemyInfo.setIsKnockedBack(false);
				} 
			}
		}
		else if (isAttacking) {
			//attack lag
			if (attackTimeCounter > 0f)
			{
				attackTimeCounter -= Time.deltaTime;
				anim.SetBool("Attack_1", false);
			}
			else
			{
				isAttacking = false;
			}
		}
		else{
			//If something with a "Player" tag is within our AlertRange...
			if (this.enemyInfo.isAlerted && 
				this.enemyInfo.trackingTarget != null) {
				//calculate dist from trackingTarget
				targetPosition = this.enemyInfo.trackingTarget.transform.position;
				ourPosition = transform.position;

				xDist = targetPosition.x - ourPosition.x;//(-) if we need to go left, (+) if we need to go right
				//yDist = targetPosition.y - ourPosition.y;

				float dir = xDist / Mathf.Abs (xDist);//-1 go left, +1 go right

				float rand = Random.Range (0f, 100f);
				bool canAttack = rand < probOfAttack;

				//if we are close enough to attack...
				if (Mathf.Abs (xDist) <= attackRange) {
					//check if we should attack this frame
					if (canAttack) {
						isAttacking = true;
						//stop moving
						myRigidBody.velocity = Vector2.zero;
						anim.SetBool ("Attack_1", true);
						attackTimeCounter = Random.Range (minAttackDelay, maxAttackDelay);

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
					if (isGrounded) {
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
		anim.SetFloat("Move_Y", yDist);
		anim.SetBool("Is_Moving", isMoving);
		anim.SetFloat("Last_Move_X", xDist);
		anim.SetFloat("Last_Move_Y", yDist);
	}

	//Handles audio
	void playAttack1Sound(){
		AudioSource audiosource = GetComponent<AudioSource> ();
		audiosource.PlayOneShot (attack1Sound, 0.25f);
	}

	//Function to walk towards a target that's within our AlertRange collider
	void trackTarget(){
	
	}

}
