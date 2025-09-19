using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PartyMember))]
public class Follower : Selectable
{
    private UIController ui;
    private CharStats charStats;
    private EntityInventory inventory;

    public enum ActionName  //TODO: possibly apply this logic to other Selectables as well
    {
        Select,
        Talk,
        Trade,
        Stay,
        Follow,
        Move_To
    };
    private Dictionary<ActionName, Action> actionMapping;
    public List<ActionName> actionNames;

    public List<string> dialogue;

    private bool stayPut;
    public override void Start()
    {
        ui = UIController.Instance;
        charStats = gameObject.GetComponent<CharStats>();
        inventory = gameObject.GetComponent<EntityInventory>();
        stayPut = false;

        actionMapping = new Dictionary<ActionName, Action> { 
            { ActionName.Select, Select },
            { ActionName.Talk, Talk },
            { ActionName.Trade, Trade },
            { ActionName.Stay, Stay },
            { ActionName.Follow, Follow }
        };
        base.Start();
    }

    public override List<(string, Action)> Actions()
    {
        var acts = new List<(string, Action)>();
        foreach (var action in actionNames)
        {
            acts.Add((action.ToString().Replace('_', ' '), actionMapping[action]));
        }
        return acts;
    }

    public void Select()
    {
        PartyController.Instance.SelectChar(gameObject.GetComponent<PartyMember>());
    }

    public void Talk()
    {
        // TODO: will need to overhaul when introduce dialogue
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

    public void Stay()
    {
        stayPut = true;
    }

    public void Follow()
    {
        stayPut = false;
        // TODO: Immediately catch up with the leader
    }

    public bool CanFollow()
    {
        return !stayPut;
    }
}
