using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(PartyMember))]
public class PartyMember : Character
{
    public virtual bool mainChar { get; } = false;

    public SelectionData setPlayer;
    public SelectionData stay;
    public SelectionData follow;

    private bool stayPut;

    public override void Awake()
    {
        setPlayer = new SelectionData(this)
        {
            actionName = "Select",
            immediateAction = SetPlayer
        };
        stay = new SelectionData(this)
        {
            actionName = "Stay",
            immediateAction = Stay
        };
        follow = new SelectionData(this)
        {
            actionName = "Follow",
            immediateAction = Follow
        };
        base.Awake();
    }

    public override void Start()
    {
        stayPut = false;

        

        combatantType = CombatManager.CombatantType.Party;
        base.Start();
    }

    public override void SetStates()
    {
        base.SetStates();

        var activeActs = new List<SelectionData>() { inspectSelection };
        var acts = new List<SelectionData>() { setPlayer, talk, CanFollow() ? stay : follow, inspectSelection };
        var combatActs = new List<SelectionData>() { inspectSelection };

        ActiveState = new ActiveState(this, StateMachine, activeActs);
        IdleState = new IdleState(this, StateMachine, acts);
        ActiveCombatState = new ActiveCombatState(this, StateMachine, combatActs);
        IdleCombatState = new IdleCombatState(this, StateMachine, combatActs);

        StateMachine.Initialize(IdleState);
    }

    public void SetPlayer()
    {
        PartyController.Instance.SelectChar(this);
    }

    public void Stay()
    {
        stayPut = true;
        var acts = new List<SelectionData>() { setPlayer, talk, follow, inspectSelection };
        IdleState = new IdleState(this, StateMachine, acts);
        StateMachine.Initialize(IdleState);
    }

    public void Follow()
    {
        stayPut = false;
        var acts = new List<SelectionData>() { setPlayer, talk, stay, inspectSelection };
        IdleState = new IdleState(this, StateMachine, acts);
        mover.Follow(PartyController.Instance.selectedPartyMember);
        StateMachine.Initialize(IdleState);
    }

    public bool CanFollow()
    {
        return !stayPut;
    }

    public override void SetIdle()
    {
        base.SetIdle();
        if (!CombatManager.Instance.combatActive) Follow();  // TODO: put in custom state?
    }

    public virtual void MoveCommand(Vector3 destination)
    {
        StateMachine.CurrentPlayerState.MoveTo(destination);
    }

    public override void Die()
    {
        CombatManager.Instance.RemoveCombatant(this);
        PartyController.Instance.RemoveCompanion(this);  // TODO: allow for only mostly dead (still in party, but disabled and can be rezzed)
        StateMachine.ChangeState(DeadState);
        EventHandler.Instance.TriggerDeathEvent(this);
    }
}
