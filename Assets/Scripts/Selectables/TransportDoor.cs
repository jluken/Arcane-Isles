using System;
using System.Collections.Generic;
using UnityEngine;

public class TransportDoor : doorway
{
    public string toLevel;
    public int spawnPoint;

    public override void Start()
    {
        base.Start();
    }

    public override List<SelectionData> Actions()
    {
        var goToLevel = new SelectionData(this)
        {
            actionName = "Go To",
            setSelect = true,
            interaction = new NewLevel()
        };
        var acts = new List<SelectionData>() { goToLevel };  // TODO: will eventually merge more with real door when out of test mode
        return acts;
    }
}

public class NewLevel : Interaction
{
    public override void Interact(PartyMember player, Selectable interactable)
    {
        if (interactable.GetComponent<TransportDoor>() == null) { Debug.LogError("Need transport door to travel to new level"); }
        var door = interactable.GetComponent<TransportDoor>();
        SceneLoader.Instance.SetToLevelSpawn(door.toLevel, door.spawnPoint);
    }
}
