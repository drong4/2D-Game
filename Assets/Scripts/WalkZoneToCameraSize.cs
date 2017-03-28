using UnityEngine;
using System.Collections;

//Changes walkzone size to camera size
public class WalkZoneToCameraSize : MonoBehaviour {

    public Camera mainCamera;
    private float height;
    private float width;

	// Use this for initialization
	void Start () {
        //Change position to position of camera
        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, transform.position.z);
        //Change dimensions of collider
        height = 2f * mainCamera.orthographicSize;
        width = height * mainCamera.aspect;
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(width, height);
    }
	
	// Update is called once per frame
	void Update () {
        //Change position to position of camera
        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, transform.position.z);
        //Change dimensions of collider
        height = 2f * mainCamera.orthographicSize;
        width = height * mainCamera.aspect;
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(width, height);
    }
}
