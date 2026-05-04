using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState: IdleState
{
    private int pathIndex = 0;
    private Enemy enemy;
    public EnemyPatrolState(Enemy npc, CharStateMachine enemyStateMachine, List<SelectionData> actions) : base(npc, enemyStateMachine, actions)
    {
        //mover = enemy.mover;
        enemy = npc;
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        Debug.Log("Start Patrol");
        enemy.isAggroed = false;
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (!enemy.mover.IsMoving() && enemy.PathMarkers.Count > 0) {
            pathIndex = (pathIndex + 1) % enemy.PathMarkers.Count;
            enemy.mover.SetDestination(enemy.PathMarkers[pathIndex].transform.position);
        }


        if (enemy.isAggroed)
        {
            CombatManager.Instance.InitiateCombat();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
