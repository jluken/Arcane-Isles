using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NPC
{
    public BaseCombatBehavior CombatBehavior;

    public List<GameObject> PathMarkers;

    public AggroRad AggroRad;
    public AwareRad AwareRad;

    public List<GameObject> AwarePlayers = new List<GameObject>();
    
    public EnemyStateMachine StateMachine;
    public EnemyIdleState IdleState;
    public EnemyPatrolState PatrolState;
    public EnemyTurnChaseState TurnChaseState;

    public bool isAggroed = false;
    
    
    public enum AnimationTriggerType
    {
        EnemyDamaged,
        PlayFootstepSound
    }

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        PatrolState = new EnemyPatrolState(this, StateMachine);
        TurnChaseState = new EnemyTurnChaseState(this, StateMachine);

        StateMachine.Initialize(PatrolState);

        //TODO: add event listener to start combat and stop combat
        //TODO: will need to be able to report whether close enough to keep combat going

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();      
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    public override void SetToCombat()
    {
        StateMachine.ChangeState(TurnChaseState);
    }

    public override void EndCombat()
    {
        StateMachine.ChangeState(PatrolState);
    }

    public void TakeAction()
    {
        StartCoroutine(CombatBehavior.CombatTurn(this));
    }
}
