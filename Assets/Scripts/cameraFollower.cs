using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollower : MonoBehaviour
{   //transform position of the camera anchor; this may be changed
    public Transform anchorTransform;
    //cam cannot leave here
    public BoxCollider2D worldBounds;

    //boundary data
    float xMin, yMin, xMax, yMax;
    //camera position data
    float camX, camY;

    //forgot what these do lol
    float camRatio;
    float camSize;

    //ref to the camera
    Camera mainCam;

    //lerp vector
    Vector3 smoothPos;
    //lerp coefficient
    public float smoothRate;

 
    // Start is called before the first frame update
    void Start()
    {
        xMin = worldBounds.bounds.min.x;
        xMax = worldBounds.bounds.max.x;
        yMin = worldBounds.bounds.min.y;
        yMax = worldBounds.bounds.max.y;

        mainCam = gameObject.GetComponent<Camera>();

        //height of camera
        camSize = mainCam.orthographicSize;
        //width of camera i think
        camRatio = (xMax + camSize) / 8.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //finding our desired positions within clamps
        camY = Mathf.Clamp(anchorTransform.position.y, yMin + camSize, yMax - camSize);
        camX = Mathf.Clamp(anchorTransform.position.x, xMin + camRatio, xMax - camRatio);
        //finding the lerp between our current and desired position
        smoothPos = Vector3.Lerp(gameObject.transform.position, new Vector3(camX, camY, gameObject.transform.position.z), smoothRate);
        //applying the lerp
        gameObject.transform.position = smoothPos;
    }
}
