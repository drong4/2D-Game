using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public Camera mainCam;
	public float shakeAmount = 0;

	void Awake(){
		if (mainCam == null) {
			mainCam = Camera.main;
		}
	}

	void Update(){
//		if (Input.GetKeyDown (KeyCode.J)) {
//			Shake (0.01f, 0.1f);
//		}
	}

	public void Shake(float amt, float length){
		shakeAmount = amt;
		InvokeRepeating ("DoShake", 0, 0.001f);//calls DoShake after 0 seconds and repeats every 0.001 seconds
		Invoke ("StopShake", length);//calls StopShake after "length" seconds
	}

	public void DoShake(){
		if (shakeAmount > 0) {
			Vector3 camPos = mainCam.transform.position;

			float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
			float offsetY = Random.value * shakeAmount * 2 - shakeAmount;

			camPos.x += offsetX;
			camPos.y += offsetY;

			mainCam.transform.position = camPos;

		}
	}

	public void StopShake(){
		CancelInvoke ("DoShake");//cancels all invoke calls in this monobehavior
		mainCam.transform.localPosition = Vector3.zero;//make local position to parent 0,0,0
	}
}
