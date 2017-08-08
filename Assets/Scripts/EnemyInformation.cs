using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Generalized script to hold the values of variables for all enemies so other scripts can read/write them,
	  without having to specify which script the enemy is using (EnemyController, WhipEnemyController, etc.)*/
public class EnemyInformation : MonoBehaviour {
	
	public bool isAlerted;
	private bool isKnockedBack;
	public GameObject trackingTarget;//track 
	public float timeToPassive;//amount of time before growing passive again, (a value of -1 means never goes passive)
	private float counter;
	private bool isInvulnerable;

	private bool isCountering;
	private bool willCounterAttack;

	// Use this for initialization
	void Start () {
		isAlerted = false;
		isKnockedBack = false;
		isInvulnerable = false;
		isCountering = false;
		willCounterAttack = false;

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

	public void setTrackingTarget(GameObject other){
		trackingTarget = other;
	}

	//Setter function for isKnockedBack
	public void setIsKnockedBack(bool val){
		isKnockedBack = val;
	}
	//Getter function for isKnockedBack
	public bool getIsKnockedback(){
		return isKnockedBack;
	}

	//Setter function for isInvulnerable
	public void setIsInvulnerable(bool val){
		isInvulnerable = val;
	}
	//Getter function for isInvulnerable
	public bool getIsInvulnerable(){
		return isInvulnerable;
	}

	//Setter function for isCountering
	public void setIsCounteringFalse(){
		isCountering = false;
	}
	public void setIsCounteringTrue(){
		isCountering = true;
	}
	//Getter function for isInvulnerable
	public bool getIsCountering(){
		return isCountering;
	}

	//Setter function for willCounterAttack
	public void setWillCounterAttack(bool val){
		willCounterAttack = val;
	}
	//Getter function for isInvulnerable
	public bool getWillCounterAttack(){
		return willCounterAttack;
	}
}
