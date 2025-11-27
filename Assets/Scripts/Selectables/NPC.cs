using NUnit.Framework;
using System;
using System.Collections.Generic;
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

    public List<string> dialogue;

    public List<AbilityAction> defaultAttacks;

    public virtual void Awake()
    {
        SetStates();
    }
    public override void Start()
    {
        interactRad.npc = this;
        interactRad.interactCollider.radius = reach;

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


    public virtual void SetStates()
    {
        StateMachine = new NPCStateMachine();

        ActiveState = new ActiveState(this, StateMachine);
        IdleState = new IdleState(this, StateMachine);
        ActiveCombatState = new ActiveCombatState(this, StateMachine);
        IdleCombatState = new IdleCombatState(this, StateMachine);

        StateMachine.Initialize(IdleState);
    }

    public override List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>() { talk, inspectSelection, trade, recruit, startAttack};
        return acts;
    }

    public override List<SelectionData> CombatActions()
    {
        var acts = new List<SelectionData>() { attack, inspectSelection, combatMovement };
        return acts;
    }

    public List<AbilityAction> GetWeaponAbilities()
    {
        var weapon = inventory.GetEquipment(InventoryData.ItemType.weapon);
        if (weapon == null) return defaultAttacks;
        return weapon.abilities;
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
        StateMachine.ChangeState(IdleCombatState);
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

    public void Die()
    {
        //TODO: award xp, drop loot, etc
        CombatManager.Instance.RemoveCombatant(this);
        SceneLoader.Instance.SceneObjectManagers[gameObject.scene.name].RemoveNPC(gameObject);
        // TODO: handle if player char and not NPC
    }

    public virtual bool IsActive => StateMachine.CurrentPlayerState.isActive;
}

public class Talk : Interaction
{
    public override void Interact(NPC npc, Selectable interactable)
    {
        if (interactable.GetComponent<NPC>() == null) { Debug.LogError("Can only talk to NPCs"); }
        UIController.Instance.ActivateDialog(npc.dialogue, npc.charStats.charImage);
    }
}

public class Trade : Interaction
{
    public override void Interact(NPC npc, Selectable interactable)
    {
        if (interactable.GetComponent<NPC>() == null) { Debug.LogError("Can only trade with NPCs"); }
        UIController.Instance.ActivateTradeScreen(npc.inventory);
    }
}

public class Recruit : Interaction
{
    public override void Interact(NPC npc, Selectable interactable)
    {
        if (interactable.GetComponent<NPC>() == null) { Debug.LogError("Can only recruit NPCs"); }
        // TODO: Add restrictions to NPC recruiting when more specific follower details are created
        NPC newNPC = interactable.GetComponent<NPC>();
        PartyController.Instance.CreateCompanion(newNPC, newNPC.gameObject);
    }
}

