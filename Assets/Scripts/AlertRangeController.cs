﻿using System.Collections;
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

		if (other.tag == "PlayerProjectile" && !this.GetComponentInParent<HealthManager>().getIsFullHp()) {
			//came in contact with a projectile fired by player and we are not full hp
			isAggro = true;//because we are hit and we know an arrow was fired by a player

			//Move the AlertRange collider SLIGHTLY...
			/*Do this bit to ensure the AlertRange collider will "collide" and register with the Player
			 * This will fix the problem of isPassiveUntilHit enemies not waking up when player is in range
			 * and shooting while standing still*/
			this.transform.position = new Vector2(transform.position.x + 0.001f, transform.position.y);
			this.transform.position = new Vector2(transform.position.x - 0.001f, transform.position.y);
		}
	}
}
