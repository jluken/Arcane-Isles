using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class doorway : Selectable
{

    public bool open = false;
    public GameObject doorObject;

    //private NavMeshSurface surface;
    public SelectionData openDoor;
    public SelectionData closeDoor;

    public override void Start()
    {
        //surface = GameObject.Find("Floor").GetComponent<NavMeshSurface>();
        doorObject.SetActive(!open);
        openDoor = new SelectionData(this)
        {
            actionName = "Open",
            setSelect = true,
            interaction = new OpenClose()
        };
        closeDoor = new SelectionData(this)
        {
            actionName = "Close",
            setSelect = true,
            interaction = new OpenClose()
        };
        base.Start();
    }

    public override List<SelectionData> Actions()
    {
        if (open) return new List<SelectionData> { closeDoor };
        else return new List<SelectionData> { openDoor, inspectSelection };
    }
}

public class OpenClose : Interaction
{
    public override void Interact(NPC npc, Selectable interactable)
    {
        // TODO: with actual assets, door will be one "object" and can have separate invisible barrier tied to i
        //Debug.Log("Door interact");
        if (interactable.GetComponent<doorway>() == null) { Debug.LogError("Trying to open/close a non-door"); }
        var door = interactable.GetComponent<doorway>();
        door.open = !door.open;
        Debug.Log(door.open);
        door.doorObject.SetActive(!door.open);
    }
}
