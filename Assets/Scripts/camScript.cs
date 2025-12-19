using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camScript : MonoBehaviour
{
    public static camScript Instance { get; private set; }

    private GameObject trackedObj;

    public float scrollSpeed = 0.002f; // Set camera movement speed.
    public float zoomSpeed = 0.1f; // Set camera movement speed.
    private Camera cam; // Reference to camera.
   // private Camera iconCam; // Used to view icons
    private Transform ct;

    private float xTilt = 37.5f;
    private float yRot = 135f;

    private bool talking;

    private void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>(); // Access player's Rigidbody.
        //iconCam = GameObject.Find("IconCamera").GetComponent<Camera>();
        ct = cam.transform;
        Camera.main.orthographic = true;
        ct.rotation = Quaternion.identity * Quaternion.Euler(xTilt, yRot, 0);

        DialogueManager.instance.conversationStarted += (sender) => { talking = true; };  // TODO: wrap this up in the controller for general UI
        DialogueManager.instance.conversationEnded += (sender) => { talking = false; };
    }

    void Update()
    {
        if (trackedObj != null) CenterCamera(trackedObj.transform.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!talking && UIController.Instance.DefaultUIOpen()) // TODO: with UI update: better handling of when can move camera (allow in combat)
        {
            // Move player based on vertical input.
            float moveVertical = Input.GetAxis("Vertical");  // TODO: legacy code
            float moveHorizontal = Input.GetAxis("Horizontal");

            bool inBounds = Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height && Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width;
            bool unmoving = moveHorizontal == 0 && moveVertical == 0;

            if (inBounds && unmoving)
            {
                float upThreshold = Screen.height * 0.90f;
                float downThreshold = Screen.height * 0.10f;
                float rightThreshold = Screen.width * 0.90f;
                float leftThreshold = Screen.width * 0.10f;
                if (Input.mousePosition.y >= upThreshold) moveVertical = (Input.mousePosition.y - upThreshold) / (Screen.height - upThreshold);
                else if (Input.mousePosition.y <= downThreshold) moveVertical = (Input.mousePosition.y - downThreshold) / (downThreshold);
                if (Input.mousePosition.x >= rightThreshold) moveHorizontal = (Input.mousePosition.x - rightThreshold) / (Screen.width - rightThreshold);
                else if (Input.mousePosition.x <= leftThreshold) moveHorizontal = (Input.mousePosition.x - leftThreshold) / (leftThreshold);

                Vector3 screenUp = new Vector3(1.0f, 0.0f, -1.0f);
                Vector3 screenRight = new Vector3(-1.0f, 0.0f, -1.0f);
                Vector3 movement = ((screenUp * moveVertical) + (screenRight * moveHorizontal));

                camScript.Instance.MoveCamera(movement, Input.mouseScrollDelta.y);
            }
        }
    }

    public void MoveCamera(Vector3 direction, float zoom)
    {
        //TODO: limit distance from player for scrolling (to avoid seeing unloaded content), or possibly load based on camera
        StopTracking();
        Vector3 movement = direction * scrollSpeed * cam.orthographicSize;

        ct.position += movement;

        cam.orthographicSize -= zoom * zoomSpeed;
        cam.orthographicSize = Math.Max(cam.orthographicSize, 2.0f);
        cam.orthographicSize = Math.Min(cam.orthographicSize, 5.0f);
        //iconCam.orthographicSize = cam.orthographicSize;
    }

    public void CenterCamera(Vector3 position)
    {
        var tiltDownRad = Math.PI * xTilt / 180.0;
        var rotRad = Math.PI * yRot / 180.0;

        float heightAbove = ct.position.y - position.y;

        float xzHypotenuse = heightAbove / (float)Math.Tan(tiltDownRad);
        float xpos = position.x - (xzHypotenuse * (float)Math.Sin(rotRad));
        float zpos = position.z - (xzHypotenuse * (float)Math.Cos(rotRad));

        ct.position = new Vector3(xpos, ct.position.y, zpos);
    }

    public void TrackObj(GameObject obj)
    {
        trackedObj = obj;
    }

    public void StopTracking()
    {
        trackedObj = null;
    }
    
}
