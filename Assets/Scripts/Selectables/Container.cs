using System;
using System.Collections.Generic;
using UnityEngine;

public class Container : Selectable
{
    public EntityInventory inventory { get; private set; }

    public SelectionData open;

    public override void Start()
    {
        open = new SelectionData(this)
        {
            actionName = "Open",
            setSelect = true,
            interaction = new Open()
        };

        inventory = gameObject.GetComponent<EntityInventory>();
        base.Start();
    }

    public override List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>();
        acts.Add(open);
        acts.Add(inspectSelection);
        acts.Add(goHere);
        return acts;
    }
}

public class Open : Interaction
{
    public override void Interact(PartyMember player, Selectable interactable)
    {
        if (interactable.GetComponent<Container>() == null) { Debug.LogError("Can only open containers"); }
        var container = interactable.GetComponent<Container>();
        UIController.Instance.ActivateContainerScreen(container.inventory);
    }
}
