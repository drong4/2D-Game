using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyManager : MonoBehaviour {

	//This script will start in scene 0.
	//Sets up the game and loads/unloads scenes as player progresses

	public static AnyManager anyManager;//static so any script can access this

	bool gameStart = false; 

	public GameObject menu;

	void Start(){
		menu = GameObject.Find ("Menu");
		menu.SetActive (false);
	}
	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			menu.SetActive (!menu.activeSelf);
		}
	}

	void Awake(){
		if (!gameStart) {
			anyManager = this;//this game object is the AnyManager (only one throughout game session)

			SceneManager.LoadSceneAsync (1, LoadSceneMode.Additive);//load the actual "first" scene

			gameStart = true;
		}
	}



	public void UnloadScene(string sceneToUnload){
		
	}

	IEnumerator Unload(string sceneToUnload){
		yield return null; //yield until the end of the current frame (fixes freezing...)

		SceneManager.UnloadScene (sceneToUnload);
	}
}
