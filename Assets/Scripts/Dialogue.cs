using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Dialogue : MonoBehaviour {

	private Text textComp;
	public string[] dialogueStrings;
	private int currStringIndex;
	public float secondsBetweenChars = 0.15f;
	public float charSpeedMultiplier = 0.01f;//how much player can speed up the dialogue
	public bool willDisablePlayerMovement;

	private KeyCode dialogueInteractInput = KeyCode.E;

	//bool to tell if currently displaying text
	private bool isDisplayingText;

	public GameObject dialogueBox;
	public GameObject textAlert;
	public GameObject dialogueContinue;//indicator to go to next line
	public AudioClip textSound;
	private AudioSource audioSource;

	// Use this for initialization
	void Start () {
		textComp = this.GetComponent<Text> ();
		textComp.text = "";//initially show nothing
		isDisplayingText = false;
		currStringIndex = 0;

		audioSource = this.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		//If we're close enough and E is pressed...
		if (textAlert.GetComponent<ProximityTextGenerator>().getIsGenerated() && 
			!isDisplayingText && Input.GetKeyDown (dialogueInteractInput)) {
			//Enable the "dialogue box"
			dialogueBox.SetActive(true);

			isDisplayingText = true;
			currStringIndex = 0;

			if (willDisablePlayerMovement) {
				//Disable player movement
				GameObject player = GameObject.Find("Player");
				player.GetComponent<WASDPlayerController> ().setCanMove (false);
			}

			//Start coroutine
			StartCoroutine (DisplayString(dialogueStrings));
		}
	}

	//Co-routine
	private IEnumerator DisplayString(string[] stringsToDisplay){
		foreach(string currString in stringsToDisplay){
			int stringLen = currString.Length;
			int currCharIndex = 0;

			//Clear the text
			textComp.text = "";

			while (currCharIndex < stringLen) {
				textComp.text += currString [currCharIndex];
				if (textSound != null && currCharIndex % 2 == 0) {
					//Play sound for every other character
//					audioSource.pitch = 0.75f;
					audioSource.PlayOneShot (textSound, 0.025f);
				}
				currCharIndex++;

				if (currCharIndex <= stringLen) {
					if (Input.anyKey) {
						yield return new WaitForSeconds (secondsBetweenChars * charSpeedMultiplier);
					} 
					else {
						yield return new WaitForSeconds (secondsBetweenChars);
					}
				} 
				else {
					break;
				}
			}

			//Reached the end of this line. Wait until player wants to move on
			if (currStringIndex < (stringsToDisplay.Length - 1)) {
				//As long as not at the last string, have the indicator for next line
				dialogueContinue.SetActive (true);
			}
			while (true) {
				if (Input.anyKeyDown) {
					dialogueContinue.SetActive (false);
					break;
				}

				yield return 0;
			}
			currStringIndex++;
		}

		//We're done displaying this string
		textComp.text = "";
		dialogueBox.SetActive (false);
		isDisplayingText = false;
		if (willDisablePlayerMovement) {
			//Enable player movement
			GameObject player = GameObject.Find("Player");
			player.GetComponent<WASDPlayerController> ().setCanMove (true);
		}
	}
}
