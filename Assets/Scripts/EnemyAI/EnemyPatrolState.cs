using UnityEngine;

public class EnemyPatrolState: EnemyState
{
    private MoveToClick mover;
    private int pathIndex = 0;
    public EnemyPatrolState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        mover = enemy.GetComponent<MoveToClick>();
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
        if (!mover.IsMoving() && enemy.PathMarkers.Count > 0) {
            //Debug.Log("Not moving");
            //Debug.Log(enemy.gameObject.name);
            pathIndex = (pathIndex + 1) % enemy.PathMarkers.Count;
            //Debug.Log(pathIndex);
            mover.SetDestination(enemy.PathMarkers[pathIndex].transform.position);
        }
        //Debug.Log("Moving");


        if (enemy.isAggroed)
        {
            CombatManager.Instance.InitiateCombat();
            //enemy.SetToCombat();
            //enemy.StateMachine.ChangeState(enemy.TurnChaseState); // TODO: possible initial combat state of deciding what to do (vs chase/attack?)
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
