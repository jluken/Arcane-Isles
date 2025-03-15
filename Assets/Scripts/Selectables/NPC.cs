using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Selectable
{
    public DialogueBox dialogueBox;
    private CharStats charStats;
    private InventoryManager inventoryManager;
    private EntityInventory inventory;

    public List<string> dialogue;
    public override void Start()
    {
        charStats = gameObject.GetComponent<CharStats>();
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
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
        base.SetInteractAction(() => { dialogueBox.ActivateChat(dialogue, charStats.charImage); });
    }

    public void Trade()
    {
        //inventoryManager.ActivateInventory(inventory);
        base.SetTarget();
        base.SetInteractAction(() => { inventoryManager.ActivateInventory(inventory); });
    }
}
