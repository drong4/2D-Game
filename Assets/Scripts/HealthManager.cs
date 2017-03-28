using UnityEngine;
using System.Collections;
using UnityEngine.UI;//access to UI

public class HealthManager : MonoBehaviour {

	public int maxHealth;
	public int currentHealth;
	public GameObject damageFX;//particles produced when damaged

//--ONLY FOR PLAYER---------------------------------------------------------
	//HUD variables
	public Slider healthSlider;
	//damage visual
	public Image damageScreen;
	Color damagedColor = new Color(0f, 0f, 0f, 0.5f);//don't alter r,g,b
	float smoothColor = 5f;
	//variables for game over animation
	public GameObject gameOverScreen;
	private Animator gameOverAnim;
	//variables for game restart
	public RestartGame theGameManager;
	//variables for player counter attack
	private bool counterSuccess;
	public bool getCounterSuccessStatus(){
		return counterSuccess;
	}
	public void setCounterSuccessStatus(bool newCounterSuccess){
		this.counterSuccess = newCounterSuccess;
	}
	//variables for healing
	public Text numOfHealsText;
	public int maxNumHeals;
	private int currNumHeals;
	public int healAmount;
//--------------------------------------------------------------------------

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
		currNumHeals = maxNumHeals;

		//HUD initialization
		healthSlider.maxValue = maxHealth;
		healthSlider.value = maxHealth;

		//if this is a player...
		if (transform.tag == "Player") {
			//game over screen initialization
			gameOverAnim = gameOverScreen.GetComponent<Animator> ();
			//counter
			counterSuccess = false;

			numOfHealsText.text = "Heals Left: " + currNumHeals.ToString ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (currentHealth <= 0) {
			//We're dead! 
			gameObject.SetActive (false);

			if (transform.tag == "Player") {
				gameOverAnim.SetBool ("Game_Over", true);
				theGameManager.restartTheGame ();
			}
		}

		//HUD 
		healthSlider.value = currentHealth;

		if (transform.tag == "Player") {
			//Slowly go back to clear screen if damaged 
			damageScreen.color = Color.Lerp (damageScreen.color, Color.clear, smoothColor * Time.deltaTime);

			if (Input.GetKeyDown (KeyCode.LeftShift) && currNumHeals > 0) {
				currentHealth += healAmount;
				if (currentHealth > maxHealth) {
					currentHealth = maxHealth;
				}
				currNumHeals--;
			}

			numOfHealsText.text = "Heals Left: " + currNumHeals.ToString ();
		}
	}

	public void ReceiveDamage(int damageReceived){
		if (transform.tag == "Player") {
			if (this.GetComponent<WASDPlayerController> ().isInvulnerable) {
				//if this is a player and invulnerable, don't take damage
				return;
			} 
			else if (this.GetComponent<WASDPlayerController> ().getValidCounterStatus()) {
				//if this is a player and is countering, don't take damage
				counterSuccess = true;
				return;
			}
			else {
				damageScreen.color = damagedColor;
			}
		}
//		if (transform.tag == "Enemy") {
//			//will give enemy flinch
//			this.GetComponent<EnemyController> ().setKnockBack (true);
//		}

		currentHealth -= damageReceived;
		GameObject generatedFX = Instantiate (damageFX, transform.position, transform.rotation);
		Destroy (generatedFX, generatedFX.GetComponent<ParticleSystem> ().duration);
		//Debug.Log(gameObject.name.ToString() + " got hit!");
	}
}
