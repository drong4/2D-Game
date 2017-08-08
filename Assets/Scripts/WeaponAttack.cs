using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EZCameraShake;//for camera shake when Player hits something

public class WeaponAttack : MonoBehaviour {
	public float knockbackAmount;
	public int damageApplied;
	public string targetTag; //What do we want to damage? "Player" or "Enemy"

	public float camShakeAmount;

	public GameObject hitImpact;

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
			int xForce = 0;
			if (initialHitPoint.x >= centre.x)
				xForce = 1;
			else
				xForce = -1;

			//Determine y direction to push
			float yForce = 0;
			if (initialHitPoint.y >= centre.y)
				yForce = 1;
			else
				yForce = -1;

			//bool that represents if the target got hit
			bool hitTarget = targetObject.GetComponentInParent<HealthManager> ().ReceiveDamage (damageApplied);

			if (targetObject.tag == "Player") {
				if (targetObject.GetComponentInParent<WASDPlayerController>().isInvulnerable ||
					targetObject.GetComponentInParent<WASDPlayerController> ().getValidCounterStatus ()) {
					return; //Don't apply knockback or flinch
				}
			}

			if (hitTarget) {
				/*Check to see if this attack has enough knockback to flinch them.
			 Compare knockback to the mass of the targetObject to determine if they should flinch*/
//			if (knockbackAmount >= targetObject.GetComponentInParent<Rigidbody2D> ().mass) {
				if (targetObject.tag == "Player") {
					targetObject.GetComponentInParent<WASDPlayerController> ().setKnockBack (true);
				}
				if (targetObject.tag == "Enemy") {
					targetObject.GetComponentInParent<EnemyInformation> ().setIsKnockedBack (true);
				}
//			}
				DoCameraShake(camShakeAmount * 0.01f, 0.1f);

				//Grab our collided with objects rigibody and apply velocity
				Rigidbody2D rigidForForce = other.gameObject.GetComponentInParent<Rigidbody2D> ();
				//calculate the velocity to apply
				Vector2 newVelocity = new Vector2 (xForce * knockbackAmount / rigidForForce.mass,
					                     yForce * knockbackAmount / rigidForForce.mass); 
				rigidForForce.velocity = newVelocity;

				//Play the hit impact animation for this weapon at target's location
				if (hitImpact != null) {
					hitImpactAnimation (xForce, targetObject.transform);
				}

			}
		}
	}

	public void DoCameraShake(float shakeAmt, float duration){
		GameObject camHolder = GameObject.Find ("CameraHolder");
		if (camHolder == null) {
			Debug.Log ("Couldn't find CameraHolder...");
		}
		CameraShake camShake = camHolder.GetComponentInChildren<CameraShake> ();
		camShake.Shake (shakeAmt, duration);
	}

	/*Helper function to spawn this weapons hit impact animation.
	 * Transform loc = position at which to spawn it
	 * int dir = holds value -1 or 1. Will spawn with local scale of "dir".
	*/
	private void hitImpactAnimation(int dir, Transform targetTrans){
		//Spawn this weapons hit impact animation at "position"
		Vector3 location = new Vector3 (targetTrans.position.x + (dir * 0.25f), targetTrans.position.y + 0.1f, targetTrans.position.z);

		var impact = Instantiate(hitImpact, location, targetTrans.rotation);

		impact.transform.localScale = new Vector3(dir * impact.transform.localScale.x, impact.transform.localScale.y, 0f);
	}
}
