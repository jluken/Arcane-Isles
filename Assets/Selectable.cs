using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    private bool selected;
    private bool isActive;
    //private bool reached;
    public Vector3 standPoint;  // TODO: possibly reevaluate whether this is best practice. Old 2d games like BG1 use it, but not modern games like BG3
    private Controller controller;

    private Action activateAction;

    private Color natColor;

    public List<SelectAction> selectActions;

    private MoveToClick mover;

    public enum SelectAction
    {
        Select,
        Talk,
        Trade,
        GoTo
    }

    private void Awake()
    {
        isActive = false;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        natColor = GetComponent<Renderer>().material.color;
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        mover = GameObject.Find("Player").GetComponent<MoveToClick>();
    }

    // Update is called once per frame
    void Update()
    {
        //bool select = (controller.target == gameObject);
        //if (select && !selected)
        //{
        //    reached = false;
        //}
        //selected = select;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //Debug.Log(other.tag);
    //    //Debug.Log(selected);
    //    // Check if the object entering the trigger is the player (or another specified object)
    //    if (other.CompareTag("Player") && selected && !reached)
    //    {
    //        ReachTarget();
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Player") && selected && !reached)
    //    {
    //        ReachTarget();
    //    }
    //}

    public virtual void Activate()  // TODO: too broad; during refactor separate out into possible interactions
    {
        Debug.Log("Activate object " + gameObject.name);
        activateAction?.Invoke();
        isActive = true;
    }

    public void Deactivate()
    {
        Debug.Log("Deactivate object " + gameObject.name);
        isActive = false;
    }

    public void Display()
    {
        //Debug.Log("Hover over object " + gameObject.name);
    }

    public bool IsActive()
    {
        return isActive;
    }

    public virtual void Select() // TODO: have this be what is called by controller, and then children can decide whether they want to set destination. PS: objects shouldn't be implementing "raw" selectable
    {
        GetComponent<Renderer>().material.color = Color.green;
        //controller.SetTarget(gameObject);
    }

    public void SetTarget()
    {
        controller.targetSelectableObj = gameObject;
        mover.SetDestination(gameObject.transform.TransformPoint(standPoint));  // TODO: mover will be handled by current priority player
    }

    public void SetActivateAction(Action activeAct)
    {
        activateAction = activeAct;
    }

    public void Deselect()
    {
        GetComponent<Renderer>().material.color = natColor;
        activateAction = null;
        Deactivate();
    }

    public Dictionary<string, Action> Actions() // TODO: possibly refactor whole way the "Controller" is currently handling all click and actions
    {
        // Return the actions that the menu will need
        var dict = new Dictionary<string, Action>();
        dict.Add("select", Select);  // TODO: Make this part of a scriptable object where each selectable object can choose from a list of pre-made options where applicable (with empty methods to be overwritten by inherited classes)
        for (int i = 0; i < selectActions.Count; i++) { 
            if (selectActions[i] == SelectAction.Talk) dict.Add("Talk", Talk);
            else if (selectActions[i] == SelectAction.Trade) dict.Add("Trade", Trade);
            else if (selectActions[i] == SelectAction.Select) dict.Add("Select", Select);
            else if (selectActions[i] == SelectAction.GoTo) dict.Add("Go Here", SetTarget);
        }
        
        return dict;
    }

    // TODO: actions: Chat, inspect, trade, attack, follow (temp), Interact(?), Take, Open

    public virtual void Trade()
    {
        throw new NotImplementedException();
    }

    public virtual void Talk ()
    {
        throw new NotImplementedException();
    }


    //private void ReachTarget()
    //{
    //    Debug.Log("Reached target");
    //    //reached = true;
    //}
}
