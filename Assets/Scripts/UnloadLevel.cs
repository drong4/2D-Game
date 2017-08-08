using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnloadLevel : MonoBehaviour {

	[SerializeField] string levelToUnload;

	bool unloaded = false;

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player" && !unloaded) {
			//SceneManager.LoadScene (levelToLoad);

			unloaded = true;
			//We don't use SceneManager.UnloadScene() because it freezes, so we use our custom one
			AnyManager.anyManager.UnloadScene(levelToUnload);
		}
	}
}
