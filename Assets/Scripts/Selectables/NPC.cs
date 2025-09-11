using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Selectable
{
    private UIController ui;
    private CharStats charStats;
    private EntityInventory inventory;

    public List<string> dialogue;
    public override void Start()
    {
        ui = UIController.Instance;
        charStats = gameObject.GetComponent<CharStats>();
        inventory = gameObject.GetComponent<EntityInventory>();
        base.Start();
    }

    public override List<(string, Action)> Actions()
    {
        var acts = new List<(string, Action)>();
        acts.Add(("Talk", Talk));
        acts.Add(("Trade", Trade));
        return acts;
    }

    public void Talk()
    {
        base.SetTarget();
        //Action act = () => { dialogueBox.ActivateChat(dialogue, charStats.charImage); };
        base.SetInteractAction(() => { ui.ActivateDialog(dialogue, charStats.charImage); });
    }

    public void Trade()
    {
        //inventoryManager.ActivateInventory(inventory);
        base.SetTarget();
        base.SetInteractAction(() => { ui.ActivateTradeScreen(inventory); });
    }
}
