using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthDisplay : MonoBehaviour {
	public GameObject boss;//the character that will be the boss
	private HealthManager bossHealthManager;
	public Slider healthSlider;
	public Text bossNameText;

	//Variables for boss music
	private AudioSource audiosource;
	private float maxVol;

	// Use this for initialization
	void Start () {
		audiosource = GetComponent<AudioSource> ();
		maxVol = audiosource.volume;
	}
	
	// Update is called once per frame
	void Update () {
		if (boss != null) {
			if (bossHealthManager == null) {
				bossHealthManager = boss.GetComponent<HealthManager> ();
			}

			if (bossHealthManager.currentHealth > 0f && boss.GetComponent<EnemyInformation>().isAlerted) {
				//If boss is alerted and is not dead yet
				enableBossHP ();
				GetComponent<BackgroundMusicManager> ().setIsTriggered (true);//start background music

				bossNameText.text = boss.name;
				healthSlider.maxValue = bossHealthManager.maxHealth;
				healthSlider.value = bossHealthManager.maxHealth;
			} 
			else {
				if (bossHealthManager.currentHealth <= 0f){
					//If boss is no longer active and the health display is enabled, disable it
					disableBossHp ();
					GetComponent<BackgroundMusicManager> ().setIsTriggered (false);//stop background music
				}
			}

			healthSlider.value = bossHealthManager.currentHealth;
		}
	}

	//Helper Function to enable the boss hp display
	void enableBossHP(){
		healthSlider.gameObject.SetActive (true);
		bossNameText.gameObject.SetActive (true);
	}

	//Helper Function to disable the boss hp display
	void disableBossHp(){
		healthSlider.gameObject.SetActive (false);
		bossNameText.gameObject.SetActive (false);
	}

	//Public Function to allow other gameObjects to set themselves as the "boss"
	public void setAsBoss(GameObject newBoss){
		boss = newBoss;
	}


}
