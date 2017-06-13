using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipEnemyController : MonoBehaviour {

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
	public float attack1Delay;
	public float attack2Delay;
	public float attack12Delay;
	private float attackTimeCounter;

	[Space]

	[Header("Animation")]
	//variables for animation
	private bool isMoving;
	public Vector2 lastMove;

	[Space]

	[Header("Audio")]
	//variables for audio
	public AudioClip attack1Sound;
	public AudioClip attack2Sound;
	AudioSource audiosource;



	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		myRigidBody = GetComponent<Rigidbody2D> ();
		audiosource = GetComponent<AudioSource> ();

		isAttacking = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (isAttacking) {
			//attack lag
			if (attackTimeCounter > 0f)
			{
				attackTimeCounter -= Time.deltaTime;
			}
			else
			{
				isAttacking = false;
				anim.SetBool ("Attack_1", false);
				anim.SetBool ("Attack_2", false);
				anim.SetBool ("Attack_12", false);
			}
		}
		else{
			if (this.GetComponent<EnemyInformation> ().isAlerted) {
				//calculate dist from trackingTarget
				targetPosition = this.GetComponent<EnemyInformation> ().trackingTarget.transform.position;
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

						//decide whether to do attack 1, 2, or (1,2)
						int attackNum = Random.Range (1, 4);//return 1-3
						if (attackNum == 1) {
							//Do attack 1
							anim.SetBool ("Attack_1", true);
							attackTimeCounter = attack1Delay;
						} else if (attackNum == 2) {
							//Do attack 2
							anim.SetBool ("Attack_2", true);
							attackTimeCounter = attack2Delay;
						} else {
							//Do attack12 combo
							anim.SetBool ("Attack_12", true);
							attackTimeCounter = attack12Delay;
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
		audiosource.PlayOneShot (attack1Sound, 0.25f);
	}
	void playAttack2Sound(){
		audiosource.PlayOneShot (attack2Sound, 0.25f);
	}

}
