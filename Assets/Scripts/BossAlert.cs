using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAlert : MonoBehaviour {

	//Attached to a gameObject that is a "BOSS"

	//Let's BossHealthDisplay know when the boss battle starts and ends

	public bool bossIsActive = false;
	private bool isAlerted;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.name == "BossEnemy") {
			isAlerted = GetComponent<EnemyController> ().isAlerted;
		} 
		else if (gameObject.name == "BossWhipEnemy") {
			isAlerted = GetComponent<WhipEnemyController> ().isAlerted;
		} 

		if (!bossIsActive && isAlerted) {
			bossIsActive = true;
		}

		if (GetComponent<HealthManager> ().currentHealth <= 0f) {
			bossIsActive = false;
		}
	}
}
