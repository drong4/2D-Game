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
			//Player either entered, or left
			//if on, turn off. if off, turn on
			if (textToGenerate.GetComponent<Text> () == null) {
				Debug.Log("yo");
			}
			textToGenerate.GetComponent<Text> ().enabled = !textToGenerate.GetComponent<Text>().enabled;
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.tag == "Player" && other.name == "Hitbox") {
			//Player either entered, or left
			//if on, turn off. if off, turn on
			textToGenerate.GetComponent<Text> ().enabled = !textToGenerate.GetComponent<Text>().enabled;
		}
	}
}
