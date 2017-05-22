using UnityEngine;
using System.Collections;

/*Follows trackingTarget relentlessly, attacking when in range*/
public class EnemyController : MonoBehaviour {

	public GameObject trackingTarget;
	public float moveSpeed;
	private Animator anim;
	private Rigidbody2D myRigidBody;

	//variables for tracking
	private float euclideanDist;
	private Vector2 targetPosition;
	private Vector2 ourPosition;
	public float attackRange;
	public float alertRange;
	public bool isAlerted;
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

	//variables for knockback animation
	private bool isKnockedBack;//lets the object that hit us set this
	public float knockbackTime;
	private float knockbackTimeCounter;
	private float flinchX;//-1 if we need to flinch left, 1 if right
	public void setFlinchX(float val){
		flinchX = val;
	}
	public void	setKnockBack(bool val){
		//lets other classes set isKnockedback
		isKnockedBack = val;
	}

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
		isKnockedBack = false;
		isAlerted = false;

		knockbackTimeCounter = -1;
	}
	
	// Update is called once per frame
	void Update () {
		isMoving = false;

		//Check if we're in contact with ground.
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

		if (isKnockedBack) {
			if (knockbackTimeCounter == -1) {
				//				Debug.Log ("Entered flinch block");
				anim.SetBool ("Is_Knocked_Back", true);
				knockbackTimeCounter = knockbackTime;
				anim.SetFloat ("Flinch_X", flinchX);
			}

			//			Debug.Log ("knockBackCounter: " + knockbackTimeCounter.ToString ());
			if (knockbackTimeCounter > 0f) {
				knockbackTimeCounter -= Time.deltaTime;
			} else {
				isKnockedBack = false;
				myRigidBody.velocity = Vector2.zero;
				knockbackTimeCounter = -1f;

				//set anim variable to false
				anim.SetBool ("Is_Knocked_Back", false);
			}
			return;

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
			//calculate dist from trackingTarget
			targetPosition = trackingTarget.transform.position;
			ourPosition = transform.position;

			xDist = targetPosition.x - ourPosition.x;//(-) if we need to go left, (+) if we need to go right
			//yDist = targetPosition.y - ourPosition.y;

			float dir = xDist / Mathf.Abs (xDist);//-1 go left, +1 go right

			float rand = Random.Range (0f, 100f);
			bool canAttack = rand < probOfAttack;
			isAlerted = Mathf.Abs (xDist) <= alertRange;

			//if we are close enough to attack...
			if (Mathf.Abs(xDist) <= attackRange) {
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
						newScale.x = -Mathf.Abs(newScale.x);
					else {
						newScale.x = Mathf.Abs(newScale.x);
					}
					transform.localScale = newScale;
				}
			}
			//else, set velocity to move towards trackingTarget's position
			else {
				if (isAlerted && isGrounded) {
					//scale xDist, yDist by moveSpeed/euclideanDist to get a net force of moveSpeed in the right direction
					myRigidBody.velocity = new Vector2 (dir * moveSpeed, 0f);

					//turn around if necessary
					Vector3 newScale = transform.localScale;
					if (xDist < 0)
						newScale.x = -Mathf.Abs(newScale.x);
					else {
						newScale.x = Mathf.Abs(newScale.x);
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
}
