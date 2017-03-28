using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

	public float knockbackAmount;
	public int damageApplied;
	public string targetTag; //"Player" or "Enemy"

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "Player") {
			//if hurting another player

			GameObject targetObject = other.gameObject;
//			if (!targetObject.GetComponent<WASDPlayerController> ().isInvulnerable) {
//				//only deal damage if target not invulnerable
//				targetObject.GetComponent<HealthManager> ().ReceiveDamage (damageApplied);
//				targetObject.GetComponent<WASDPlayerController> ().setKnockBack (true);
//				targetObject.GetComponent<WASDPlayerController> ().knockbackTime = knockbackAmount;
//				targetObject.GetComponent<WASDPlayerController>().setFlinchX(
//			}

			//Store the vector 2 of the location where the initial hit happened 
			//(note: Vector3 automatically converts to Vector2 by removing z);
			Vector2 initialHitPoint = other.gameObject.transform.position;

			//calculate the minimum and maximum 'extents' that we will use for picking direction
			Vector2 extentSize = transform.localScale / 3.0f;
			Vector2 centre = transform.position;
			Vector2 minExtent = centre-extentSize;
			Vector2 maxExtent = centre+extentSize;

			//Determine x direction to push
			float xForce = 0;
			if (initialHitPoint.x > maxExtent.x)
				xForce = 1;
			else if (initialHitPoint.x < minExtent.x)
				xForce = -1;
			else
				xForce = 0;

			//Determine y direction to push
			float yForce = 0;
			if (initialHitPoint.y > maxExtent.y)
				yForce = 1;
			else if (initialHitPoint.y < minExtent.y)
				yForce = -1;
			else
				yForce = 0;

			//calculate the velocity to apply
			Vector2 newVelocity = new Vector2(xForce * knockbackAmount, yForce * knockbackAmount); 

			//print out lots of stuff
//			Debug.Log("Hit point: " + initialHitPoint.ToString());
//			Debug.Log("Min extent: " + minExtent.ToString());
//			Debug.Log("Max extent: " + maxExtent.ToString());
//			Debug.Log("New velocity: " + newVelocity.ToString());

			//Grab our collided with objects rigibody and apply velocity
			Rigidbody2D rigidForForce = other.gameObject.GetComponent<Rigidbody2D>();
			rigidForForce.velocity = newVelocity;

			if (!targetObject.GetComponent<WASDPlayerController> ().isInvulnerable) {
				//only deal damage if target not invulnerable
				targetObject.GetComponent<HealthManager> ().ReceiveDamage (damageApplied);
				targetObject.GetComponent<WASDPlayerController> ().setKnockBack (true);
				targetObject.GetComponent<WASDPlayerController> ().knockbackTime = knockbackAmount;
				targetObject.GetComponent<WASDPlayerController> ().setFlinchX (xForce);
			}
		}

		if (other.gameObject.tag == "Enemy") {
			//if hurting an enemy

			GameObject targetObject = other.gameObject;
//			if (!targetObject.GetComponent<EnemyController> ().isInvulnerable) {
				//only deal damage if target not invulnerable
//				targetObject.GetComponent<HealthManager> ().ReceiveDamage (damageApplied);
//				targetObject.GetComponent<EnemyController> ().setKnockBack (true);
//				targetObject.GetComponent<EnemyController> ().knockbackTime = knockbackAmount;
//			}
			//set variable for the targetObject to do knockback animation


			//Store the vector 2 of the location where the initial hit happened 
			//(note: Vector3 automatically converts to Vector2 by removing z);
			Vector2 initialHitPoint = other.gameObject.transform.position;

			//calculate the minimum and maximum 'extents' that we will use for picking direction
			Vector2 extentSize = transform.localScale / 3.0f;
			Vector2 centre = transform.position;
			Vector2 minExtent = centre-extentSize;
			Vector2 maxExtent = centre+extentSize;

			//Determine x direction to push
			float xForce = 0;
			if (initialHitPoint.x > maxExtent.x)
				xForce = 1;
			else if (initialHitPoint.x < minExtent.x)
				xForce = -1;
			else
				xForce = 0;

			//Determine y direction to push
			float yForce = 0;
			if (initialHitPoint.y > maxExtent.y)
				yForce = 1;
			else if (initialHitPoint.y < minExtent.y)
				yForce = -1;
			else
				yForce = 0;

			//calculate the velocity to apply
			Vector2 newVelocity = new Vector2(xForce * knockbackAmount, yForce * knockbackAmount); 

			//print out lots of stuff
//			Debug.Log("Hit point: " + initialHitPoint.ToString());
//			Debug.Log("Min extent: " + minExtent.ToString());
//			Debug.Log("Max extent: " + maxExtent.ToString());
			//			Debug.Log("New velocity: " + newVelocity.ToString());

			//Grab our collided with objects rigibody and apply velocity
			Rigidbody2D rigidForForce = other.gameObject.GetComponent<Rigidbody2D>();
			rigidForForce.velocity = newVelocity;

			targetObject.GetComponent<HealthManager> ().ReceiveDamage (damageApplied);
			targetObject.GetComponent<EnemyController> ().setKnockBack (true);
			targetObject.GetComponent<EnemyController> ().knockbackTime = knockbackAmount;
			targetObject.GetComponent<EnemyController> ().setFlinchX (xForce);
		}
	}
}
