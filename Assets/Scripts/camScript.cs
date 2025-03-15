using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camScript : MonoBehaviour
{

    public float scrollSpeed = 0.002f; // Set camera movement speed.
    public float zoomSpeed = 0.1f; // Set camera movement speed.
    private Camera cam; // Reference to camera.
   // private Camera iconCam; // Used to view icons
    private Transform ct;

    private bool haltCam;


    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>(); // Access player's Rigidbody.
        //iconCam = GameObject.Find("IconCamera").GetComponent<Camera>();
        ct = cam.transform;
        Camera.main.orthographic = true;
        haltCam = false;
    }

    void Update()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Move player based on vertical input.
        float moveVertical = Input.GetAxis("Vertical");  // TODO: legacy code
        float moveHorizontal = Input.GetAxis("Horizontal");

        if (moveVertical==0 && Input.mousePosition.y >= Screen.height * 0.99) moveVertical = 1.0f;
        else if (moveVertical == 0 && Input.mousePosition.y <= Screen.height * 0.01) moveVertical = -1.0f;
        if (moveHorizontal == 0 && Input.mousePosition.x >= Screen.width * 0.99) moveHorizontal = 1.0f;
        else if (moveHorizontal == 0 && Input.mousePosition.x <= Screen.width * 0.01) moveHorizontal = -1.0f;
        //Debug.Log("Horz: " + moveHorizontal);
        Vector3 screenUp = new Vector3(1.0f, 0.0f, -1.0f);
        Vector3 screenRight = new Vector3(-1.0f, 0.0f, -1.0f);
        Vector3 movement = ((screenUp * moveVertical) + (screenRight * moveHorizontal)) * scrollSpeed * cam.orthographicSize;

        // TODO: put in a check with the menu controller once that exists so this doesn't move when menu open

        if (!haltCam)
        {
            ct.position += movement;

            cam.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed; // TODO: check if not over a panel (zooming while scrolling through text log)
            cam.orthographicSize = Math.Max(cam.orthographicSize, 2.0f);
            cam.orthographicSize = Math.Min(cam.orthographicSize, 5.0f);
            //iconCam.orthographicSize = cam.orthographicSize;
        }
    }

    public void CamHalt(bool halt) // TODO: what to do if multiple agents are "holding" at once? keep a stack?
    {
        haltCam = halt;
    }

    //private void FixedUpdate()
    //{
        
    //    //ct.position += Input.mouseScrollDelta.y * transform.forward * 1.0f;
    //    //cam.orthographicSize += Input.mouseScrollDelta.y;
    //}
}
