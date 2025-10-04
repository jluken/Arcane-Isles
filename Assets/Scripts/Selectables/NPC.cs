using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : Selectable
{
    private CharStats charStats;
    private EntityInventory inventory;

    public enum ActionName  // Used for optional actions
    {
        Talk,
        Trade,
        Recruit
    };
    private Dictionary<ActionName, Action> actionMapping; 
    public List<ActionName> actionNames;

    public List<string> dialogue;
    public override void Start()
    {
        charStats = gameObject.GetComponent<CharStats>();
        inventory = gameObject.GetComponent<EntityInventory>();

        actionMapping = new Dictionary<ActionName, Action> {
            { ActionName.Talk, Talk },
            { ActionName.Trade, Trade },
            { ActionName.Recruit, Recruit },
        };
        base.Start();
    }

    public override List<(string, Action)> Actions()
    {
        var acts = new List<(string, Action)>();
        foreach (var action in actionNames) { 
            acts.Add((action.ToString(), actionMapping[action]));
        }
        return acts;
    }

    public void Talk()
    {
        // TODO: will need to overhaul when introduce dialogue
        base.SetTarget();
        //Action act = () => { dialogueBox.ActivateChat(dialogue, charStats.charImage); };
        base.SetInteractAction(() => { UIController.Instance.ActivateDialog(dialogue, charStats.charImage); });
    }

    public void Trade()
    {
        //inventoryManager.ActivateInventory(inventory);
        base.SetTarget();
        base.SetInteractAction(() => { UIController.Instance.ActivateTradeScreen(inventory); });
    }

    public void Recruit()
    {
        //inventoryManager.ActivateInventory(inventory);
        base.SetTarget();
        //base.SetInteractAction(() => { PartyController.Instance.AddCompanion(this.GetComponent<PartyMember>()); });
        
        // TODO: eventually will have smarter logic of what a companion's stats are upon recruitment
        base.SetInteractAction(() => { PartyController.Instance.CreateCompanion(charStats, inventory, gameObject); });  
    }
}