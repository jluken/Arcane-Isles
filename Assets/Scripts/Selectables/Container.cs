using System;
using System.Collections.Generic;
using UnityEngine;

public class Container : Selectable
{
    public EntityInventory inventory => gameObject.GetComponent<EntityInventory>();

    public override void Start()
    {
        base.Start();
    }

    public override List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>();
        acts.Add(openInventory);
        acts.Add(inspectSelection);
        acts.Add(goHere);
        return acts;
    }
}


