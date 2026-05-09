using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static PartyData;

public class Character : Selectable
{
    public CharStats charStats => gameObject.GetComponent<CharStats>();
    public EntityInventory inventory => gameObject.GetComponent<EntityInventory>();
    public MoveToClick mover => gameObject.GetComponent<MoveToClick>();

    public CharInteract interactRad;
    public float reach = 1;

    public SelectionData talk;
    public SelectionData trade;
    public SelectionData recruit;
    public SelectionData startAttack;
    public SelectionData attack;

    public CharStateMachine StateMachine;
    public ActiveState ActiveState;
    public IdleState IdleState;
    public ActiveCombatState ActiveCombatState;
    public IdleCombatState IdleCombatState;
    public DeadState DeadState;

    public WeaponItem defaultWeapon;

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
        SetStates();
    }
    public override void Start()
    {
        interactRad.character = this;
        interactRad.interactCollider.radius = reach;

        CombatManager.Instance.callToArms += SetToCombat;
        CombatManager.Instance.setPeace += EndCombat;
        base.Start();
    }

    public void OnDestroy()
    {
        CombatManager.Instance.callToArms -= SetToCombat;
        CombatManager.Instance.setPeace -= EndCombat;
    }

    public void LoadFromSaveData(CharSaveData charSaveData)
    {
        charStats.LoadFromSaveData(charSaveData.charStatData);
        inventory.LoadFromSaveData(charSaveData.inventory);
        mover.agent.Warp(new Vector3(charSaveData.pos[0], charSaveData.pos[1], charSaveData.pos[2]));
        transform.rotation = Quaternion.identity * Quaternion.Euler(charSaveData.rot[0], charSaveData.rot[1], charSaveData.rot[2]);
        Debug.Log("Loading State + " + charSaveData.stateName + " for " + charStats.charName);
        LoadState(charSaveData.stateName);
    }


    public virtual void SetStates()
    {
        StateMachine = new CharStateMachine();

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

    public virtual void LoadState(string stateName)
    {
        switch (stateName)
        {
            case nameof(ActiveState):
                StateMachine.ChangeState(ActiveState); break;
            case nameof(IdleState):
                StateMachine.ChangeState(IdleState); break;
            case nameof(ActiveCombatState):
                StateMachine.ChangeState(ActiveCombatState); break;
            case nameof(IdleCombatState):
                StateMachine.ChangeState(IdleCombatState); break;
            case nameof(DeadState):
                StateMachine.ChangeState(DeadState); break;
        }
    }

    public string StateName() => StateMachine.CurrentPlayerState.stateName;

    public override void StartHover()
    {
        base.StartHover();
        GetComponent<Outline>().OutlineColor = Color.yellow;
    }

    public override List<SelectionData> Actions()
    {
        //Debug.Log("Get Actions from " + charStats.charName + " at state " + StateMachine.CurrentPlayerState);
        return StateMachine.CurrentPlayerState.GetActions();
    }

    public List<AbilityAction> GetWeaponAbilities()
    {
        var weapon = inventory.GetEquipment(InventoryData.ItemType.weapon);
        if (weapon == null) return new List<AbilityAction>(defaultWeapon.ItemActions());
        var abilities =  new List<AbilityAction>(weapon.ItemActions());
        foreach ( var action in abilities) { action.SetActor(this); }
        return abilities;
    }

    public AbilityAction GetDefaultAttack()
    {
        var weapon = inventory.GetEquipment(InventoryData.ItemType.weapon);
        if (weapon == null || weapon.DefaultAttack() == null) return defaultWeapon.DefaultAttack();
        var attack = weapon.DefaultAttack();
        attack.SetActor(this);
        return attack;
    }

    public void takeDamage(int rawDamage)
    {
        var totalArmor = inventory.GetEquipmentArmor();
        charStats.updateHealth(-1 * Math.Max(rawDamage - totalArmor, 0));
    }

    public void TargetAttack()
    {
        CombatManager.Instance.AttackTarget(this);
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
        StateMachine.CurrentPlayerState.EnterCombat();
    }

    public virtual void EndCombat()
    {
        StateMachine.CurrentPlayerState.EndCombat();
    }

    public virtual void SetActiveChar()
    {
        StateMachine.CurrentPlayerState.SetActiveChar();
    }

    public virtual void SetIdle()
    {
        StateMachine.CurrentPlayerState.SetIdle();
    }

    public virtual void Die()
    {
        foreach (var equipSlot in inventory.equipment.Keys) inventory.Dequip(equipSlot);
        StateMachine.ChangeState(DeadState);
        EventHandler.Instance.TriggerDeathEvent(this);
    }

    public void BringIntoCombat()
    {
        CombatManager.Instance.InitiateCombat(new List<Character> { this });
    }

    public List<AbilityAction> GetActions()
    {
        var abilities = GetWeaponAbilities();
        abilities.Insert(0, CombatManager.Instance.defaultRun);
        foreach (var action in abilities) { action.SetActor(this); }
        return abilities;
    }

    public virtual bool IsActive => StateMachine.CurrentPlayerState.isActive;
}

public class Talk : Interaction
{
    public override void Interact(Character npc, Selectable interactable)
    {
        if (interactable.GetComponent<Character>() == null) { Debug.LogError("Can only talk to NPCs"); }
        DialogueInterface.Instance.StartPlayerConversation("TestConvo1", npc, interactable.GetComponent<Character>());
    }
}

public class Trade : Interaction
{
    public override void Interact(Character npc, Selectable interactable)
    {
        if (interactable.GetComponent<Character>() == null) { Debug.LogError("Can only trade with NPCs"); }
        UIController.Instance.ActivateTradeScreen(interactable.GetComponent<Character>().inventory);
    }
}

public class Recruit : Interaction
{
    public override void Interact(Character npc, Selectable interactable)
    {
        if (interactable.GetComponent<Companion>() == null) { Debug.LogError("Can only recruit Companions"); }
        // TODO: Add restrictions to NPC recruiting when more specific follower details are created
        Companion companion = interactable.GetComponent<Companion>();
        PartyController.Instance.RecruitCompanion(companion);
    }
}

