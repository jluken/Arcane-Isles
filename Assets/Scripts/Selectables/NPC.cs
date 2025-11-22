using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : Selectable  // TODO: rename to "Character" or something since it includes player characters
{
    public CharStats charStats;
    public EntityInventory inventory;

    public CapsuleCollider InteractRad;  // Todo: move this to NPC
    public float reach = 1;

    public SelectionData talk;
    public SelectionData trade;
    public SelectionData recruit;
    public SelectionData startAttack;
    public SelectionData attack;

    public List<string> dialogue;
    public override void Start()
    {
        LoadNPCData();
        InteractRad.radius = reach;

        talk = new SelectionData(this)
        {
            actionName = "Talk",
            setSelect = true,
            interaction = new Talk()
        };
        trade = new SelectionData(this)
        {
            actionName = "Trade",
            setSelect = true,
            interaction = new Trade()
        };
        recruit = new SelectionData(this)
        {
            actionName = "Recruit",
            setSelect = true,
            interaction = new Recruit()
        };
        startAttack = new SelectionData(this)
        {
            actionName = "Attack",
            immediateAction = CombatManager.Instance.InitiateCombat
        };
        attack = new SelectionData(this)
        {
            actionName = "Attack",
            immediateAction = TargetAttack
        };

        base.Start();
    }

    public virtual void LoadNPCData()
    {
        charStats = gameObject.GetComponent<CharStats>();
        inventory = gameObject.GetComponent<EntityInventory>();
    }

    public override List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>() { talk, inspectSelection, trade, recruit, startAttack};
        return acts;
    }

    public override List<SelectionData> CombatActions()
    {
        var acts = new List<SelectionData>() { attack, inspectSelection }; //TODO: add combatMovement as an option when made generic
        return acts;
    }

    public void TargetAttack()  // TODO: find better place to put attack methods (NPC?)
    {
        CombatManager.Instance.UseCombatAbility(this, CombatManager.CombatActionType.Attack);
    }

    public virtual void SetToCombat()
    {
        //TODO: move states to here?
    }

    public virtual void EndCombat()
    {
        //TODO: move states to here?
    }
}

public class Talk : Interaction
{
    public override void Interact(PartyMember player, Selectable interactable)
    {
        if (interactable.GetComponent<NPC>() == null) { Debug.LogError("Can only talk to NPCs"); }
        var npc = interactable.GetComponent<NPC>();
        UIController.Instance.ActivateDialog(npc.dialogue, npc.charStats.charImage);
    }
}

public class Trade : Interaction
{
    public override void Interact(PartyMember player, Selectable interactable)
    {
        if (interactable.GetComponent<NPC>() == null) { Debug.LogError("Can only trade with NPCs"); }
        var npc = interactable.GetComponent<NPC>();
        UIController.Instance.ActivateTradeScreen(npc.inventory);
    }
}

public class Recruit : Interaction
{
    public override void Interact(PartyMember player, Selectable interactable)
    {
        if (interactable.GetComponent<NPC>() == null) { Debug.LogError("Can only recruit NPCs"); }
        // TODO: Add restrictions to NPC recruiting when more specific follower details are created
        var npc = interactable.GetComponent<NPC>();
        PartyController.Instance.CreateCompanion(npc, npc.gameObject);
    }
}

