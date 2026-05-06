using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : Character
{
    public BaseCombatBehavior CombatBehavior;

    public List<GameObject> PathMarkers;

    public AggroRad AggroRad;
    public AwareRad AwareRad;

    public List<GameObject> AwarePlayers = new List<GameObject>();

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

    public override void SetToCombat()
    {
        if(AwarePlayers.Count > 0) base.SetToCombat();
    }

    public override void SetStates()
    {
        base.SetStates();

        var acts = new List<SelectionData>() { inspectSelection, goHere, startAttack };

        ActiveState = new ActiveState(this, StateMachine, acts);
        IdleState = new EnemyPatrolState(this, StateMachine, acts);

        StateMachine.Initialize(IdleState);
    }

    public override void Die()
    {
        Debug.Log("Enemy Die");
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
