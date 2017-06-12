using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertRangeController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerStay2D(Collider2D other){
		if (other.tag == "Player") {
			//if a player is within our alert range, notify this object to chase after the player
			this.GetComponentInParent<EnemyInformation> ().setIsAlerted(true);
			this.GetComponentInParent<EnemyInformation> ().setTrackingTarget(other);
			this.GetComponentInParent<EnemyInformation> ().setCounter ();
		}
	}
}
