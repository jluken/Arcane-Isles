using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class doorway : Selectable
{

    public bool open = false;
    public GameObject door;  // TODO: with actual assets, door will be one "object" and can have separate invisible barrier tied to it

    private NavMeshSurface surface;

    public override void Start()
    {
        surface = GameObject.Find("Floor").GetComponent<NavMeshSurface>();
        door.SetActive(!open);
        base.Start();
    }

    public override List<(string, Action)> Actions()
    {
        var acts = new List<(string, Action)>();
        if (open) acts.Add(("Close", DoorTarget));
        else acts.Add(("Open", DoorTarget));
        acts.Add(("Inspect", base.Inspect));

        return acts;
    }

    public void DoorTarget()
    {
        Debug.Log("Door Target");
        base.SetTarget();
        base.SetInteractAction(() => { OpenClose();});
    }

    public void OpenClose()
    {
        Debug.Log("activate door");
        open = !open;
        door.SetActive(!open);
    }
}
