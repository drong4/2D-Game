using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGame : MonoBehaviour {

	public float restartTime;
	private bool restartNow;
	private float resetTime;

	// Use this for initialization
	void Start () {
		restartNow = false;
		resetTime = Time.time; 
	}
	
	// Update is called once per frame
	void Update () {
		if (restartNow && resetTime <= Time.time) {
			Application.LoadLevel (Application.loadedLevel);//reload the currently loaded level
		}
	}

	public void restartTheGame(){
		restartNow = true;
		resetTime = Time.time + restartTime;
	}

}
