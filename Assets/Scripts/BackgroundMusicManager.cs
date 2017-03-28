using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour {

	public AudioClip soundtrack;//soundtrack to play
	private AudioSource audiosource;
	private float origVol;
	public float fadeTime;
	private bool isTriggered;
	public void setIsTriggered(bool newBool){
		isTriggered = newBool;
	}
	private bool isPlaying;

	// Use this for initialization
	void Start () {
		audiosource = GetComponent<AudioSource> ();
		origVol = audiosource.volume;
		isTriggered = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (isTriggered && !isPlaying) {
			isPlaying = true;
			MusicFadeIn (soundtrack, fadeTime);
		}
		if (!isTriggered && isPlaying) {
			isPlaying = false;
			MusicFadeOut (fadeTime);
		}

	}

	//INCOMPLETE------------------------------------
	//Helper Function to fade into "soundtrack"
	void MusicFadeIn(AudioClip soundtrack, float fadeTime){
		audiosource.volume = 0f;//start off at 0 volume
		audiosource.clip = soundtrack;
		audiosource.Play ();

		while (audiosource.volume < origVol) {
			audiosource.volume +=  Time.deltaTime/fadeTime;
		}

	}
	//Helper Function to fade out of audio clip
	void MusicFadeOut(float fadeTime){
		audiosource.Stop ();
	}
}
