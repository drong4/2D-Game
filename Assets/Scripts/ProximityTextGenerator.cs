using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProximityTextGenerator : MonoBehaviour {

	public Text textToGenerate;
	public GameObject textBackground;
	private bool isGenerated;

	public bool getIsGenerated(){
		return isGenerated; 
	}

	// Use this for initialization
	void Start () {
		isGenerated = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player" && other.name == "Hitbox") {
			//Player just entered range
			textBackground.SetActive (true);
			textToGenerate.GetComponent<Text> ().enabled = true;
			isGenerated = true;
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.tag == "Player" && other.name == "Hitbox") {
			//Player just left range
			textBackground.SetActive (false);
			textToGenerate.GetComponent<Text> ().enabled = false;
			isGenerated = false;
		}
	}
}
