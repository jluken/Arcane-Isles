using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Merchant : NPC  // Making a type of NPC for now
{

    public override void Start()
    {
        base.Start();
    }


    public override List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>() { inspectSelection, trade };

        return acts;
    }
}
