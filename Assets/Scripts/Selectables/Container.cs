using System;
using System.Collections.Generic;
using UnityEngine;

public class Container : Selectable
{
    private EntityInventory inventory;
    private UIController ui;

    public override void Start()
    {
        ui = UIController.Instance;
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
        base.SetInteractAction(() => { ui.ActivateContainerScreen(inventory); });
    }
}
