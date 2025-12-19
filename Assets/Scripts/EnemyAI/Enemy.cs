using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NPC
{
    public BaseCombatBehavior CombatBehavior;

    public List<GameObject> PathMarkers;

    public AggroRad AggroRad;
    public AwareRad AwareRad;

    public List<GameObject> AwarePlayers = new List<GameObject>();
    
    //public EnemyStateMachine StateMachine;
    //public EnemyIdleState IdleState;
    //public EnemyPatrolState PatrolState;
    //public EnemyTurnChaseState TurnChaseState;

    public bool isAggroed = false;
    
    
    public enum AnimationTriggerType
    {
        EnemyDamaged,
        PlayFootstepSound
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        AggroRad.enemy = this;
        AwareRad.enemy = this;
        base.Start();
    }

    public override List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>() { inspectSelection, goHere, startAttack };
        return acts;
    }

    public override List<SelectionData> CombatActions()
    {
        var acts = new List<SelectionData>() { attack, inspectSelection, combatMovement };
        return acts;
    }

    public override void SetStates()
    {
        StateMachine = new NPCStateMachine();

        ActiveState = new ActiveState(this, StateMachine);
        IdleState = new EnemyPatrolState(this, StateMachine);
        ActiveCombatState = new ActiveCombatState(this, StateMachine);
        IdleCombatState = new IdleCombatState(this, StateMachine);

        StateMachine.Initialize(IdleState);
    }

    //private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    //{
    //    StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    StateMachine.CurrentEnemyState.FrameUpdate();      
    //}

    //private void FixedUpdate()
    //{
    //    StateMachine.CurrentEnemyState.PhysicsUpdate();
    //}

    //public override void SetToCombat()
    //{
    //    StateMachine.ChangeState(TurnChaseState);
    //}

    //public override void EndCombat()
    //{
    //    StateMachine.ChangeState(PatrolState);
    //}

    public override void Die()
    {
        QuestLog.SetQuestEntryState("Kill the bug", 1, "success");  //TODO: temporary testing
        base.Die();
    }

    public void TakeAction()
    {
        StartCoroutine(CombatBehavior.CombatTurn(this));
    }
}
