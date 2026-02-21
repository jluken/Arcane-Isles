using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    }

    void Update()
    {
        if (trackedObj != null) CenterCamera(trackedObj.transform.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!UIController.Instance.PauseTime())
        {
            Vector2 mousePosition = SelectionController.Instance.MousePosition();
            Vector2 mouseScroll = SelectionController.Instance.MouseScroll();
            Vector2 moveDirection = InputSystem.actions.FindActionMap("UI").FindAction("Navigate").ReadValue<Vector2>();

            bool inBounds = mousePosition.y >= 0 && mousePosition.y <= Screen.height && mousePosition.x >= 0 && mousePosition.x <= Screen.width;

            if (inBounds && moveDirection == Vector2.zero)
            {
                float upThreshold = Screen.height * 0.90f;
                float downThreshold = Screen.height * 0.10f;
                float rightThreshold = Screen.width * 0.90f;
                float leftThreshold = Screen.width * 0.10f;
                if (mousePosition.y >= upThreshold) moveDirection.y = (mousePosition.y - upThreshold) / (Screen.height - upThreshold);
                else if (mousePosition.y <= downThreshold) moveDirection.y = (mousePosition.y - downThreshold) / (downThreshold);
                if (mousePosition.x >= rightThreshold) moveDirection.x = (mousePosition.x - rightThreshold) / (Screen.width - rightThreshold);
                else if (mousePosition.x <= leftThreshold) moveDirection.x = (mousePosition.x - leftThreshold) / (leftThreshold);
            }

            Vector3 screenUp = new Vector3(1.0f, 0.0f, -1.0f);
            Vector3 screenRight = new Vector3(-1.0f, 0.0f, -1.0f);
            Vector3 movement = ((screenUp * moveDirection.y) + (screenRight * moveDirection.x));

            MoveCamera(movement, mouseScroll.y);
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

    public void CenterCamera(Vector3 position)  // TODO: make private? Only call Track Obj?
    {
        //Debug.Log("Center Camera: " + position);
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
