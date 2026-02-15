using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPC : Selectable  // TODO: rename to "Character" or something since it includes player characters
{
    public CharStats charStats => gameObject.GetComponent<CharStats>();
    public EntityInventory inventory => gameObject.GetComponent<EntityInventory>();
    public MoveToClick mover => gameObject.GetComponent<MoveToClick>();

    public NPCInteract interactRad;
    public float reach = 1;

    public SelectionData talk;
    public SelectionData trade;
    public SelectionData recruit;
    public SelectionData startAttack;
    public SelectionData attack;

    public NPCStateMachine StateMachine;
    public ActiveState ActiveState;
    public IdleState IdleState;
    public ActiveCombatState ActiveCombatState;
    public IdleCombatState IdleCombatState;
    public DeadState DeadState;

    public List<string> dialogue;

    public List<AbilityAction> defaultAttacks;

    public CombatManager.CombatantType combatantType = CombatManager.CombatantType.Bystander;

    public override void Awake()
    {
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
            immediateAction = BringIntoCombat
        };
        attack = new SelectionData(this)
        {
            actionName = "Attack",
            immediateAction = TargetAttack
        };
        base.Awake();
        SetStates(); // TODO: possibly move this and action definitions to Awake (but they have to come first)
    }
    public override void Start()
    {
        interactRad.npc = this;
        interactRad.interactCollider.radius = reach;

        CombatManager.Instance.callToArms += SetToCombat;
        base.Start();
    }

    public void OnDestroy()
    {
        CombatManager.Instance.callToArms -= SetToCombat;
    }


    public virtual void SetStates()
    {
        StateMachine = new NPCStateMachine();

        var acts = new List<SelectionData>() { talk, inspectSelection, trade, recruit, startAttack };
        var combatActs = new List<SelectionData>() { attack, inspectSelection, combatMovement };
        var deadActs = new List<SelectionData>() { inspectSelection, openInventory };

        ActiveState = new ActiveState(this, StateMachine, acts);
        IdleState = new IdleState(this, StateMachine, acts);
        ActiveCombatState = new ActiveCombatState(this, StateMachine, combatActs);
        IdleCombatState = new IdleCombatState(this, StateMachine, combatActs);
        DeadState = new DeadState(this, StateMachine, deadActs);

        StateMachine.Initialize(IdleState);
    }

    public override List<SelectionData> Actions()
    {
        //Debug.Log("Get Actions from " + charStats.charName + " at state " + StateMachine.CurrentPlayerState);
        return StateMachine.CurrentPlayerState.GetActions();
    }

    //public override List<SelectionData> CombatActions()
    //{
    //    var acts = new List<SelectionData>() { attack, inspectSelection, combatMovement };
    //    return acts;
    //}

    public List<AbilityAction> GetWeaponAbilities()
    {
        var weapon = inventory.GetEquipment(InventoryData.ItemType.weapon);
        if (weapon == null) return new List<AbilityAction>(defaultAttacks); ;
        return new List<AbilityAction>(weapon.abilities);
    }

    public void TargetAttack()
    {
        CombatManager.Instance.UseCombatAbility(this, CombatManager.CombatActionType.Attack);
    }

    void Update()
    {
        StateMachine.CurrentPlayerState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentPlayerState.PhysicsUpdate();
    }

    public virtual void SetToCombat()
    {
        Debug.Log("Set to Combat: " + charStats.charName);
        StateMachine.CurrentPlayerState.EnterCombat();
    }

    public virtual void EndCombat()
    {
        StateMachine.ChangeState(IdleState);
    }

    public virtual void SetActiveNPC()
    {
        StateMachine.CurrentPlayerState.SetActiveNPC();
    }

    public virtual void SetIdle()
    {
        StateMachine.CurrentPlayerState.SetIdle();
    }

    public virtual void Die()
    {
        //CombatManager.Instance.RemoveCombatant(this);
        //SceneLoader.Instance.SceneObjectManagers[gameObject.scene.name].RemoveNPC(this);
        foreach (var equipSlot in inventory.equipment.Keys) inventory.Dequip(equipSlot);
        StateMachine.ChangeState(DeadState);
        EventHandler.Instance.TriggerDeathEvent(this);
    }

    public void BringIntoCombat()
    {
        CombatManager.Instance.InitiateCombat(new List<NPC> { this });
    }

    public List<AbilityAction> GetActions()
    {
        var attacks = GetWeaponAbilities();
        attacks.Insert(0, CombatManager.Instance.defaultRun);  // TODO: handle special case of movement better
        return attacks;
    }

    public virtual bool IsActive => StateMachine.CurrentPlayerState.isActive;
}

public class Talk : Interaction
{
    public override void Interact(NPC npc, Selectable interactable)
    {
        if (interactable.GetComponent<NPC>() == null) { Debug.LogError("Can only talk to NPCs"); }
        //UIController.Instance.ActivateDialog(npc.dialogue, npc.charStats.charImage);
        //DialogueManager.StartConversation("TestConvo1");
        DialogueInterface.Instance.StartPlayerConversation("TestConvo1", npc, interactable.GetComponent<NPC>());
    }
}

public class Trade : Interaction
{
    public override void Interact(NPC npc, Selectable interactable)
    {
        if (interactable.GetComponent<NPC>() == null) { Debug.LogError("Can only trade with NPCs"); }
        UIController.Instance.ActivateTradeScreen(interactable.GetComponent<NPC>().inventory);
    }
}

public class Recruit : Interaction
{
    public override void Interact(NPC npc, Selectable interactable)
    {
        if (interactable.GetComponent<Companion>() == null) { Debug.LogError("Can only recruit Companions"); }
        // TODO: Add restrictions to NPC recruiting when more specific follower details are created
        Companion companion = interactable.GetComponent<Companion>();
        PartyController.Instance.RecruitCompanion(companion);
    }
}

