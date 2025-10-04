using System;
using System.Collections.Generic;
using UnityEngine;

public class Container : Selectable
{
    private EntityInventory inventory;

    public override void Start()
    {
        inventory = gameObject.GetComponent<EntityInventory>();
        base.Start();
    }

    public override List<(string, Action)> Actions()
    {
        var acts = new List<(string, Action)>();
        acts.Add(("Open", OpenContainer));
        return acts;
    }

    public void OpenContainer()
    {
        base.SetTarget();
        base.SetInteractAction(() => { UIController.Instance.ActivateContainerScreen(inventory); });
    }
}
