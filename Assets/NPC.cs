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
    public override void Activate()
    {
        //Talk();
        base.Activate();
        base.Deselect(); // TODO: handle "deselection" better
    }

    public override void Talk()
    {
        base.SetTarget();
        //Action act = () => { dialogueBox.ActivateChat(dialogue, charStats.charImage); };
        base.SetActivateAction(() => { dialogueBox.ActivateChat(dialogue, charStats.charImage); });
    }

    public override void Trade()
    {
        //inventoryManager.ActivateInventory(inventory);
        base.SetTarget();
        base.SetActivateAction(() => { inventoryManager.ActivateInventory(inventory); });
    }
}
