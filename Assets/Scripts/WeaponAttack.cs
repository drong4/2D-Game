using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EZCameraShake;//for camera shake when Player hits something

public class WeaponAttack : MonoBehaviour {
	public float knockbackAmount;
	public int damageApplied;
	public string targetTag; //What do we want to damage? "Player" or "Enemy"

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == targetTag && other.name == "Hitbox") {
			GameObject targetObject = other.gameObject;

			//Store the vector 2 of the location where the initial hit happened 
			//(note: Vector3 automatically converts to Vector2 by removing z);
			Vector2 initialHitPoint = targetObject.transform.position;
			Vector2 centre = transform.position;


			//Determine x direction to push
			float xForce = 0;
			if (initialHitPoint.x > centre.x)
				xForce = 1;
			else if (initialHitPoint.x < centre.x)
				xForce = -1;
			else
				xForce = 0;

			//Determine y direction to push
			float yForce = 0;
			if (initialHitPoint.y > centre.y)
				yForce = 1;
			else if (initialHitPoint.y < centre.y)
				yForce = -1;
			else
				yForce = 0;

			//Try to apply damage (target could be invulnerable)
			//Vector3 pointOfContact = new Vector3(initialHitPoint.x, initialHitPoint.y, transform.position.z);
			targetObject.GetComponentInParent<HealthManager> ().ReceiveDamage (damageApplied);

			if (targetObject.tag == "Player") {
				if (targetObject.GetComponentInParent<WASDPlayerController>().isInvulnerable ||
					targetObject.GetComponentInParent<WASDPlayerController> ().getValidCounterStatus ()) {
					return; //Don't apply knockback or flinch
				}
			}

			/*Check to see if this attack has enough knockback to flinch them.
			 Compare knockback to the mass of the targetObject to determine if they should flinch*/
			if (knockbackAmount >= targetObject.GetComponentInParent<Rigidbody2D> ().mass) {
				if (targetObject.tag == "Player") {
					targetObject.GetComponentInParent<WASDPlayerController> ().setKnockBack (true);
				}
				if (targetObject.tag == "Enemy") {
					targetObject.GetComponentInParent<EnemyInformation> ().setIsKnockedBack (true);
				}
			}

			//Grab our collided with objects rigibody and apply velocity
			Rigidbody2D rigidForForce = other.gameObject.GetComponentInParent<Rigidbody2D> ();
			//calculate the velocity to apply
			Vector2 newVelocity = new Vector2 (xForce * knockbackAmount / rigidForForce.mass,
				                     yForce * knockbackAmount / rigidForForce.mass); 
			rigidForForce.velocity = newVelocity;
		}
	}
}
