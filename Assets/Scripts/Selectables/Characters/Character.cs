using System;
using System.Collections.Generic;
using UnityEngine;
using static PartyData;

public class Character : Selectable
{
    public CharStats charStats => gameObject.GetComponent<CharStats>();
    public CharSigils sigils => gameObject.GetComponent<CharSigils>();
    public EntityInventory inventory => gameObject.GetComponent<EntityInventory>();
    public MoveToClick mover => gameObject.GetComponent<MoveToClick>();

    public GameObject renderBody;
    public Animator animator;

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
    public CapsuleCollider wanderZone;

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

    public override void Update()
    {
        StateMachine.CurrentPlayerState.FrameUpdate();
        base.Update();
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
        if(sigils != null) sigils.LoadFromSaveData(charSaveData.charSigils);
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
        var totalAbilities = new List<AbilityAction>();
        var mainWeapon = inventory.GetEquipment(EntityInventory.EquipmentInvType.holdMainHand);
        if(mainWeapon != null) totalAbilities.AddRange(mainWeapon.ItemActions());
        else if (defaultWeapon != null) totalAbilities.AddRange(defaultWeapon.ItemActions());
        var offWeapon = inventory.GetEquipment(EntityInventory.EquipmentInvType.holdOffHand);
        if (offWeapon != null) totalAbilities.AddRange(offWeapon.ItemActions());
        foreach ( var action in totalAbilities) { action.SetActor(this); }
        return totalAbilities;
    }

    public List<AbilityAction> GetSigilAbilities()
    {
        if (sigils == null) return new List<AbilityAction>();
        var abilities = sigils.PreparedSigilAbilities();
        foreach (var action in abilities) { if(action != null) action.SetActor(this); }
        return abilities;
    }

    public AbilityAction GetDefaultAttack()
    {
        var weapon = inventory.GetEquipment(EntityInventory.EquipmentInvType.holdMainHand);
        if (weapon == null || weapon.DefaultAttack() == null) return defaultWeapon.DefaultAttack();
        var attack = weapon.DefaultAttack();
        attack.SetActor(this);
        return attack;
    }

    public void takeDamage(int rawDamage, bool bypassArmor = false)
    {
        // TODO: alert
        var totalArmor = bypassArmor ? 0 : inventory.GetEquipmentArmor();
        var damage = Math.Max(rawDamage - totalArmor, 0);
        charStats.updateHealth(-1 * damage);
    }

    public void TargetAttack()
    {
        CombatManager.Instance.TargetObj(this);
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
        CombatManager.Instance.SetActions(this, GetWeaponAbilities(), GetSigilAbilities());
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
        StartCoroutine(mover.PlantFeetAsync());
        EventHandler.Instance.TriggerDeathEvent(this);
    }

    public void BringIntoCombat()
    {
        CombatManager.Instance.InitiateCombat(new List<Character> { this });
    }

    //public List<AbilityAction> GetActions()
    //{
    //    var abilities = GetWeaponAbilities();
    //    abilities.Insert(0, CombatManager.Instance.defaultRun);
    //    foreach (var action in abilities) { action.SetActor(this); }
    //    return abilities;
    //}

    public virtual bool IsActive => StateMachine.CurrentPlayerState.isActive;
}

public class Talk : Interaction
{
    public override void Interact(Character npc, Selectable interactable)
    {
        if (interactable.GetComponent<Character>() == null) { Debug.LogError("Can only talk to NPCs"); }
        DialogueInterface.Instance.StartPlayerCharConversation("TestConvo1", npc, interactable.GetComponent<Character>());
    }
}

public class Trade : Interaction
{
    public override void Interact(Character npc, Selectable interactable)
    {
        if (interactable.GetComponent<Merchant>() == null) { Debug.LogError("Can only trade with MerchantNPCs"); }
        UIController.Instance.ActivateTradeScreen(interactable.GetComponent<Merchant>(), interactable.GetComponent<Merchant>().isBuying);
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

