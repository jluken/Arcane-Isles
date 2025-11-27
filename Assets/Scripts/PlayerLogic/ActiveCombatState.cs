using UnityEngine;

public class ActiveCombatState: NPCState
{
    public ActiveCombatState(NPC npc, NPCStateMachine npcStateMachine) : base(npc, npcStateMachine)
    {
    }

    public override bool isActive => true;

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        npc.mover.agent.avoidancePriority = 100;
        base.EnterState();
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

    public override void SetIdle()
    {
        npcStateMachine.ChangeState(npc.IdleCombatState);
    }
}
