using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private PlayerController playerController;
    private SelectMenu selectMenu;

    public string description;
    private IEnumerator displayRoutine;
    public GameObject itemPopUp;
    public GameObject textLog; // TODO: should be able to get this from UI Manager
    private TextLog textLogText;

    private void Awake()
    {
        isActive = false;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        controller.selectEvent += Deselect;
        playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
        selectMenu = GameObject.Find("SelectMenu").GetComponent<SelectMenu>();

        if (textLog != null) textLogText = textLog.GetComponent<TextLog>(); // TODO: should be able to get this from UI Manager
        displayRoutine = DisplayText();
        //itemPopUp = GameObject.Find("sharedpopup"); // TODO: use prefab manager to create these instead
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
        controller.NewSelection();
        selected = true;
        var currentLeader = playerController.CurrentSelectedLeader().GetComponent<MoveToClick>(); 
        currentLeader.SetDestination(gameObject.transform.TransformPoint(standPoint)); // TODO: Will need to handle more complex navigation; possibly update MoveToClick to handle parties
    }

    public void SetInteractAction(Action activeAct)
    {
        interactAction = activeAct;
    }

    public void Deselect()
    {
        interactAction = null;
        selected = false;
        Deactivate();
    }

    public virtual List<(string, Action)> Actions()
    {
        var acts = new List<(string, Action)>();
        acts.Add(("Go Here", SetTarget));
        
        return acts;
    }

    public void Inspect()
    {

        textLogText.AddText(description); // TODO: Consolidate Text updates into event controller

        StopCoroutine(displayRoutine);
        displayRoutine = DisplayText();
        StartCoroutine(displayRoutine);
    }

    IEnumerator DisplayText()
    {
        Debug.Log("is itempopup " + (itemPopUp != null));  // TODO: instantiate as child from prefab
        var height = gameObject.GetComponent<MeshRenderer>().bounds.max.y;
        itemPopUp.transform.localPosition = new Vector3(0, (height / 2) + 2, 0);
        itemPopUp.GetComponent<TMP_Text>().text = description;
        itemPopUp.SetActive(true);
        yield return new WaitForSeconds(3);
        itemPopUp.SetActive(false); // TODO: fade out animation?
    }

}
