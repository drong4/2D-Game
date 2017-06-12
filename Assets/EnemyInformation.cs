using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Generalized script to hold the values of variables for all enemies so other scripts can read/write them,
	  without having to specify which script the enemy is using (EnemyController, WhipEnemyController, etc.)*/
public class EnemyInformation : MonoBehaviour {
	
	public bool isAlerted;
	public Collider2D trackingTarget;//track them by their collider
	public float timeToPassive;//amount of time before growing passive again, (a value of -1 means never goes passive)
	public float counter;

	// Use this for initialization
	void Start () {
		isAlerted = false;
		trackingTarget = null;

		counter = timeToPassive;
	}
	
	// Update is called once per frame
	void Update () {
		if (isAlerted) {
			if (timeToPassive == -1) {
				return;//don't bother trying to set isAlerted back to false
			}
			counter -= Time.deltaTime;
			if (counter <= 0f) {
				isAlerted = false;//go passive again
			}
		}
	}

	//Function to let the AlertRangeController script "reset" the counter
	public void setCounter(){
		counter = timeToPassive;
	}

	public void setIsAlerted(bool val){
		isAlerted = val;
	}

	public void setTrackingTarget(Collider2D other){
		trackingTarget = other;
	}
}
