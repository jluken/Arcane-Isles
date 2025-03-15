using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static Selectable;

public class Selectable : MonoBehaviour
{
    private bool selected;
    private bool isActive;
    //private bool reached;
    public Vector3 standPoint;  // TODO: possibly reevaluate whether this is best practice. Old 2d games like BG1 use it, but not modern games like BG3
    private Controller controller;

    private Action interactAction;

    private Color natColor;

    private PlayerController playerController;
    private SelectMenu selectMenu;

    private void Awake()
    {
        isActive = false;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        natColor = GetComponent<Renderer>().material.color;
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        controller.selectEvent += Deselect;
        playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
        selectMenu = GameObject.Find("SelectMenu").GetComponent<SelectMenu>();
        selected = false;
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
                Actions()[0].Item2.Invoke();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("select menu: " + selectMenu);
                selectMenu.GetComponent<SelectMenu>().ActivateMenu(Input.mousePosition, Actions());
            }

            HoverDisplay();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.CurrentSelectedLeader() && !IsActive() && selected)
        {
            RaycastHit hit;
            var rayDirection = other.gameObject.transform.position - transform.position;
            if (Physics.Raycast(transform.position, rayDirection, out hit))
            {
                if (hit.transform == other.gameObject.transform)
                {  // Line of sight between player and target
                    Interact();
                    playerController.CurrentSelectedLeader().GetComponent<MoveToClick>().StopMoving();
                }

            }
        }
    }

    public virtual void Interact()
    {
        interactAction?.Invoke();
        Deselect();
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    public void HoverDisplay()
    {
        //Debug.Log("Hover over object " + gameObject.name);
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void SetTarget()
    {
        Debug.Log("Set Target");
        controller.NewSelection();
        selected = true;
        var currentLeader = playerController.CurrentSelectedLeader().GetComponent<MoveToClick>(); 
        currentLeader.SetDestination(gameObject.transform.TransformPoint(standPoint)); // TODO: Will need to handle more complex navigation; possibly update MoveToClick to handle parties
    }

    public void SetInteractAction(Action activeAct)
    {
        Debug.Log("Set interact " + activeAct);
        interactAction = activeAct;
    }

    public void Deselect()
    {
        //GetComponent<Renderer>().material.color = natColor;
        Debug.Log("Deselect");
        interactAction = null;
        selected = false;
        Deactivate();
    }

    public virtual List<(string, Action)> Actions()
    {
        // Return the actions that the menu will need
        var acts = new List<(string, Action)>();
        acts.Add(("Go Here", SetTarget));
        
        return acts;
    }

}
