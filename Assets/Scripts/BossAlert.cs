using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAlert : MonoBehaviour {

	//Attached to a gameObject that is a "BOSS"

	//Let's BossHealthDisplay know when the boss battle starts and ends

	public bool bossIsActive = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (!bossIsActive && GetComponent<EnemyController> ().isAlerted) {
			bossIsActive = true;
		}

		if (GetComponent<HealthManager> ().currentHealth <= 0f) {
			bossIsActive = false;
		}
	}
}
