using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class groundScript : MonoBehaviour
{
    private Controller controller;

    private PlayerController playerController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Barrier");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, ~layerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject(-1) && hit.transform.gameObject == gameObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("ground point");
                Debug.Log(hit.point);
                Debug.Log(hit.transform.position);
                SetTarget(hit.point);
            }
        }
    }

    public void SetTarget(Vector3 dest)
    {
        controller.NewSelection();
        var currentLeader = playerController.CurrentSelectedLeader().GetComponent<MoveToClick>();
        currentLeader.SetDestination(dest); // TODO: Will need to handle more complex navigation; possibly update MoveToClick to handle parties
    }
}
