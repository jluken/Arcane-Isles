using UnityEngine;

public class EnemyTurnChaseState : EnemyState
{
    //TODO: keep track of how many combatants its "aware of", and if none then will fall into Turn-based idle state (and out of turn order). Once no more in Turn-based fight mode, end combat
    public EnemyTurnChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.GetComponent<MoveToClick>().StopMoving();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
