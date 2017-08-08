using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadLevel : MonoBehaviour {

	//Level to load
	[SerializeField] string levelToLoad;

	//New camera boundaries
	[SerializeField] private float xMin;
	[SerializeField] private float xMax;
	[SerializeField] private float yMin;
	[SerializeField] private float yMax;

	//Position to teleport player to
	public float newPlayerLocX;
//	public float locY;

	GameObject player;
	GameObject camHolder;

	//Bool to make sure we don't keep loading the same level
	bool loaded = false;

	void Awake(){
		player = GameObject.Find ("Player");
		camHolder = GameObject.Find ("CameraHolder");
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			//SceneManager.LoadScene (levelToLoad);

			//if the level we're trying to load isn't loaded yet
			if (!loaded && SceneManager.GetSceneByName(levelToLoad).name != levelToLoad) {
				//LoadSceneAsync loads scene in background
				//LoadSceneMode.Additive means loading this scene without destroying the current scene
				SceneManager.LoadSceneAsync (levelToLoad, LoadSceneMode.Additive);
				loaded = true;
			}
				
			//Update camera boundaries
			camHolder.GetComponent<CameraController> ().setCamBounds (xMin, xMax, yMin, yMax);
			camHolder.GetComponent<CameraController> ().instantTrackTarget ();


			//Teleport player to new position
			player.transform.position = new Vector3(newPlayerLocX, player.transform.position.y, player.transform.position.z);
		}
	}
}
