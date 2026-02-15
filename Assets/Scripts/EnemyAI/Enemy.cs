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

        combatantType = CombatManager.CombatantType.Enemy;
        base.Start();
    }

    //public override List<SelectionData> Actions()
    //{
    //    var acts = new List<SelectionData>() { inspectSelection, goHere, startAttack };
    //    return acts;
    //}

    //public override List<SelectionData> CombatActions()
    //{
    //    var acts = new List<SelectionData>() { attack, inspectSelection, combatMovement };
    //    return acts;
    //}

    public override void SetStates()
    {
        base.SetStates();

        var acts = new List<SelectionData>() { inspectSelection, goHere, startAttack };

        ActiveState = new ActiveState(this, StateMachine, acts);
        IdleState = new EnemyPatrolState(this, StateMachine, acts);

        Debug.Log("Enemy init to idle");
        StateMachine.Initialize(IdleState);
    }

    public override void Die()
    {
        Debug.Log("Enemy Die");
        QuestLog.SetQuestEntryState("Kill the bug", 1, "success");  //TODO: temporary testing
        base.Die();
    }

    public override void EndCombat()
    {
        SetIdle();
    }

    public void TakeAction()
    {
        StartCoroutine(CombatBehavior.CombatTurn(this));
    }
}
