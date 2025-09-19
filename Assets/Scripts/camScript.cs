using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camScript : MonoBehaviour
{
    public static camScript Instance { get; private set; }

    public float scrollSpeed = 0.002f; // Set camera movement speed.
    public float zoomSpeed = 0.1f; // Set camera movement speed.
    private Camera cam; // Reference to camera.
   // private Camera iconCam; // Used to view icons
    private Transform ct;

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

        if (moveVertical==0 && Screen.height * 0.95 <= Input.mousePosition.y && Input.mousePosition.y <= Screen.height * 1.05) moveVertical = 1.0f;
        else if (moveVertical == 0 && Input.mousePosition.y <= Screen.height * 0.05 && Input.mousePosition.y >= Screen.height * -0.05) moveVertical = -1.0f;
        if (moveHorizontal == 0 && Input.mousePosition.x >= Screen.width * 0.95 && Input.mousePosition.x <= Screen.width * 1.05) moveHorizontal = 1.0f;
        else if (moveHorizontal == 0 && Input.mousePosition.x <= Screen.width * 0.05 && Input.mousePosition.x >= Screen.width * -0.05) moveHorizontal = -1.0f;
        //Debug.Log("Horz: " + moveHorizontal);
        Vector3 screenUp = new Vector3(1.0f, 0.0f, -1.0f);
        Vector3 screenRight = new Vector3(-1.0f, 0.0f, -1.0f);
        Vector3 movement = ((screenUp * moveVertical) + (screenRight * moveHorizontal)) * scrollSpeed * cam.orthographicSize;

        if (UIController.Instance.DefaultUIOpen())
        {
            ct.position += movement;

            cam.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
            cam.orthographicSize = Math.Max(cam.orthographicSize, 2.0f);
            cam.orthographicSize = Math.Min(cam.orthographicSize, 5.0f);
            //iconCam.orthographicSize = cam.orthographicSize;
        }
    }

    //private void FixedUpdate()
    //{
        
    //    //ct.position += Input.mouseScrollDelta.y * transform.forward * 1.0f;
    //    //cam.orthographicSize += Input.mouseScrollDelta.y;
    //}
}
