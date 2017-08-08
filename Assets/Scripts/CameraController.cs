using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float moveSpeed;//How fast camera gets to desired position (determines camera lag)

    public GameObject followTarget1;

    private float tar1X;
    private float tar1Y;
	private float tar1Z;

	//Set camera boundaries
	[SerializeField] private float xMin;
	[SerializeField] private float xMax;
	[SerializeField] private float yMin;
	[SerializeField] private float yMax;

    private Vector3 desiredPos;//Position that camera will go to


    private static bool cameraExists;

	// Use this for initialization
	void Start () {
//        //Deals with any duplicates of the camera when any scene loading happens
//        if (!cameraExists)
//        {
//            cameraExists = true;
//            DontDestroyOnLoad(transform.gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }

        //The following code determines the camera's start position
		tar1X = Mathf.Clamp(followTarget1.transform.position.x, xMin, xMax);
		tar1Y = Mathf.Clamp (followTarget1.transform.position.y, yMin, yMax);
		tar1Z = followTarget1.transform.position.z;
		transform.position = new Vector3(tar1X, tar1Y+1, tar1Z - 10f);
    }

    // Update is called once per frame
	void Update () {
		//Move cam
		smoothTrackTarget ();
    }

	//Move cam to target smoothly overtime
	public void smoothTrackTarget(){
		tar1X = Mathf.Clamp(followTarget1.transform.position.x, xMin, xMax);
		tar1Y = Mathf.Clamp (followTarget1.transform.position.y, yMin, yMax);
		tar1Z = followTarget1.transform.position.z;

		desiredPos = new Vector3(tar1X, tar1Y + 1, tar1Z - 10f);
		
		transform.position = Vector3.Lerp (transform.position, desiredPos, moveSpeed * Time.deltaTime);
	}

	//Move cam to target instantly
	public void instantTrackTarget(){
		tar1X = Mathf.Clamp(followTarget1.transform.position.x, xMin, xMax);
		tar1Y = Mathf.Clamp (followTarget1.transform.position.y, yMin, yMax);
		tar1Z = followTarget1.transform.position.z;

		transform.position = new Vector3(tar1X, tar1Y + 1, tar1Z - 10f);
	}

	//Let other scenes set the new cam bounds
	public void setCamBounds(float newXmin, float newXmax, float newYmin, float newYmax){
		xMin = newXmin;
		xMax = newXmax;
		yMin = newYmin;
		yMax = newYmax;
	}

	//Helper function to tell if cam is within bounds
	private bool camIsWithinBounds(){
		Vector3 camPos = transform.position;
		if (camPos.x < xMin || camPos.x > xMax || camPos.y < yMin || camPos.y > yMax) {
			return false;
		}

		return true;
	}
}
