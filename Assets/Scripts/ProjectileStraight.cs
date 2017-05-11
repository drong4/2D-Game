using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStraight : MonoBehaviour {

	public float speed; 
	public int damage;
	public float aliveTime;//maximum time arrow is alive
	private float aliveTimeCounter;//
	public string targetTag; //determines what this projectile can damage
	public AudioClip collisionSound; 
	private Rigidbody2D myRigidBody;

	// Use this for initialization
	void Start () {
		myRigidBody = GetComponent<Rigidbody2D>();
		myRigidBody.velocity = new Vector2 (speed * transform.localScale.x, myRigidBody.velocity.y);
		aliveTimeCounter = aliveTime;
	}
	
	// Update is called once per frame
	void Update () {
		aliveTimeCounter -= Time.deltaTime;
		if (aliveTimeCounter < 0) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag != "Player") {
			if (other.tag == targetTag) {
				other.gameObject.GetComponentInParent<HealthManager> ().ReceiveDamage (damage);
			}
			Destroy (gameObject);
		}
	}

	void playCollisionSound(){
	
	}
}
