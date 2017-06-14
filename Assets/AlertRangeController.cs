using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertRangeController : MonoBehaviour {

	public bool isPassiveUntilHit;
	private bool isAggro;

	// Use this for initialization
	void Start () {
		isAggro = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerStay2D(Collider2D other){
		if (other.tag == "Player") {
			//if a player is within our alert range, check if we should chase

			if (!isAggro) {
				//if not aggro already...
				if (isPassiveUntilHit) {
					//are we full hp?
					if (this.GetComponentInParent<HealthManager> ().getIsFullHp ()) {
						//if we're still full hp, dont chase yet
						return;
					} else {
						//not full hp, so something hit us already!
						isAggro = true;
					}
				} else {
					isAggro = true;
				}
			} else {
				//notify this object to chase after the player
				this.GetComponentInParent<EnemyInformation> ().setIsAlerted (true);
				this.GetComponentInParent<EnemyInformation> ().setTrackingTarget (other.gameObject);
				this.GetComponentInParent<EnemyInformation> ().setCounter ();
			}
		} 

		if (other.tag == "PlayerProjectile") {
			//came in contact with a projectile fired by player
			isAggro = true;//EVEN IF isPassiveUntilHit was true, since projectiles disturb
		}
	}
}
