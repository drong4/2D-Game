using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Quit : MonoBehaviour {

	public void quitGame(){
		Debug.Log ("Game quit.");
		Application.Quit ();
	}
}
