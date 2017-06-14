using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProximityTextGenerator : MonoBehaviour {

	public Text textToGenerate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player" && other.name == "Hitbox") {
			//Player just entered range
			textToGenerate.GetComponent<Text> ().enabled = true;
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.tag == "Player" && other.name == "Hitbox") {
			//Player just left range
			textToGenerate.GetComponent<Text> ().enabled = false;
		}
	}
}
