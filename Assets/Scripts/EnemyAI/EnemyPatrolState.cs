using UnityEngine;

public class EnemyPatrolState: IdleState
{
    //private MoveToClick mover;
    private int pathIndex = 0;
    private Enemy enemy;
    public EnemyPatrolState(Enemy npc, NPCStateMachine enemyStateMachine) : base(npc, enemyStateMachine)
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
            //Debug.Log("Not moving");
            //Debug.Log(enemy.gameObject.name);
            pathIndex = (pathIndex + 1) % enemy.PathMarkers.Count;
            //Debug.Log(pathIndex);
            enemy.mover.SetDestination(enemy.PathMarkers[pathIndex].transform.position);
        }
        //Debug.Log("Moving");


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
