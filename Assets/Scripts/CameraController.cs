using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float moveSpeed;//How fast camera gets to desired position (determines camera lag)
    //public float maxSize;//Max size of camera
    //public float minSize;//Min size of camera
    //public float changeSizeSpeed;//How fast camera adjusts

    private float currSize;//Curr size of camera
    private float desiredSize;

    public GameObject followTarget1;
    //public GameObject followTarget2;

    private float diffInX;
    private float diffInY;
    private float dist;

    private float tar1X;
    private float tar1Y;
	private float tar1Z;

    private float tar2X;
    private float tar2Y;

    private float averageX;
    private float averageY;
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
        tar1X = followTarget1.transform.position.x;
        tar1Y = followTarget1.transform.position.y;
		tar1Z = followTarget1.transform.position.z;
		transform.position = new Vector3(tar1X, tar1Y, tar1Z - 10f);
    }

    // Update is called once per frame
    void Update () {
        tar1X = followTarget1.transform.position.x;
        tar1Y = followTarget1.transform.position.y;
		tar1Z = followTarget1.transform.position.z;

		desiredPos = new Vector3(tar1X, tar1Y+1, tar1Z - 10f);//Camera's z-position stays the same

        transform.position = Vector3.Lerp(transform.position, desiredPos, moveSpeed * Time.deltaTime);//Move camera
    }
}
