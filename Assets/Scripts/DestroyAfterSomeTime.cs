using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSomeTime : MonoBehaviour {

	public float aliveTime;
	private float aliveTimeCounter;
	private Animator anim;

	// Use this for initialization
	void Start () {
		aliveTimeCounter = aliveTime;
		anim = this.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (aliveTimeCounter <= 0f) {
			Destroy (gameObject);
		} 
		if (anim != null) {
			if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= .8f) {
				Destroy (gameObject);
			}
		}
	
		aliveTimeCounter -= Time.deltaTime;
	}
}
