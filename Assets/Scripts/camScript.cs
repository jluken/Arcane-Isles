using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class camScript : MonoBehaviour
{
    public static camScript Instance { get; private set; }

    private GameObject trackedObj;

    public float scrollSpeed = 0.0015f; // Set camera movement speed.
    public float zoomSpeed = 0.1f; // Set camera movement speed.
    public float maxCamDist = 40f;
    public Camera cam { get;  private set; } // Reference to camera.
    private Transform ct;

    private float xTilt = 37.5f;
    private float yRot = 135f;

    public event Action camMove;

    private void Awake()
    {
        Instance = this;

        cam = GetComponent<Camera>(); // Access player's Rigidbody.
        ct = cam.transform;
        Camera.main.orthographic = true;
        ct.rotation = Quaternion.identity * Quaternion.Euler(xTilt, yRot, 0);
    }


    // Start is called before the first frame update
    void Start()
    {
        //cam = GetComponent<Camera>(); // Access player's Rigidbody.
        //ct = cam.transform;
        //Camera.main.orthographic = true;
        //ct.rotation = Quaternion.identity * Quaternion.Euler(xTilt, yRot, 0);
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
            Vector3 screenUp = new Vector3(1.0f, 0.0f, -1.0f);
            Vector3 screenRight = new Vector3(-1.0f, 0.0f, -1.0f);

            Vector2 mousePosition = SelectionController.MousePosition();
            Vector2 mouseScroll = SelectionController.MouseScroll();
            Vector2 moveDirection = InputSystem.actions.FindActionMap("UI").FindAction("Navigate").ReadValue<Vector2>();

            bool inBounds = mousePosition.y >= 0 && mousePosition.y <= Screen.height && mousePosition.x >= 0 && mousePosition.x <= Screen.width;

            if (inBounds && moveDirection == Vector2.zero)
            {
                float upThreshold = Screen.height * 0.95f;
                float downThreshold = Screen.height * 0.05f;
                float rightThreshold = Screen.width * 0.95f;
                float leftThreshold = Screen.width * 0.05f;
                if (mousePosition.y >= upThreshold) moveDirection.y = (mousePosition.y - upThreshold) / (Screen.height - upThreshold);
                else if (mousePosition.y <= downThreshold) moveDirection.y = (mousePosition.y - downThreshold) / (downThreshold);
                if (mousePosition.x >= rightThreshold) moveDirection.x = (mousePosition.x - rightThreshold) / (Screen.width - rightThreshold);
                else if (mousePosition.x <= leftThreshold) moveDirection.x = (mousePosition.x - leftThreshold) / (leftThreshold);
            }

            // Disallow moving too far away  // TODO: maybe just restrict to bounds of level map
            var pointAbovePlayer = CameraAbovePoint(PartyController.Instance.selectedPartyMember.transform.position);
            var distRight = Vector3.Scale(ct.position - pointAbovePlayer, screenRight);
            var distUp = Vector3.Scale(ct.position - pointAbovePlayer, screenUp);
            var cappedMovement = new Vector2(CapMovementVector(moveDirection.x, distRight.magnitude), CapMovementVector(moveDirection.y, distUp.magnitude));

            
            Vector3 movement = ((screenUp * cappedMovement.y) + (screenRight * cappedMovement.x));
            var scroll = !EventSystem.current.IsPointerOverGameObject() ? mouseScroll.y : 0.0f;


            //var distFromPlayer = Vector3.Distance(ct.position, PartyController.Instance.selectedPartyMember.transform.position);  
            if (inBounds && (movement != Vector3.zero || scroll != 0)) MoveCamera(movement, scroll);
        }
    }

    private float CapMovementVector(float movement, float dist)
    {
        return Math.Abs(movement + dist) > maxCamDist ? 0f : movement;
    }

    public void MoveCamera(Vector3 direction, float zoom)
    {
        StopTracking();
        Vector3 movement = direction * scrollSpeed * cam.orthographicSize;

        ct.position += movement;

        cam.orthographicSize -= zoom * zoomSpeed;
        cam.orthographicSize = Math.Max(cam.orthographicSize, 2.0f);
        cam.orthographicSize = Math.Min(cam.orthographicSize, 5.0f);
        camMove.Invoke();
    }

    public void CenterCamera(Vector3 position)
    {
        ct.position = CameraAbovePoint(position);
    }

    private Vector3 CameraAbovePoint(Vector3 position)
    {
        var tiltDownRad = Math.PI * xTilt / 180.0;
        var rotRad = Math.PI * yRot / 180.0;

        float heightAbove = ct.position.y - position.y;

        float xzHypotenuse = heightAbove / (float)Math.Tan(tiltDownRad);
        float xpos = position.x - (xzHypotenuse * (float)Math.Sin(rotRad));
        float zpos = position.z - (xzHypotenuse * (float)Math.Cos(rotRad));

        return new Vector3(xpos, ct.position.y, zpos);
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
