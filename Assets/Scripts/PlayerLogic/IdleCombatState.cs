using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleCombatState: NPCState
{
    public IdleCombatState(NPC npc, NPCStateMachine playerStateMachine, List<SelectionData> actions) : base(npc, playerStateMachine, actions)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        npc.mover.StopMoving();
        npc.mover.agent.avoidancePriority = 50;
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

    public override void SetActiveNPC()
    {
        npcStateMachine.ChangeState(npc.ActiveCombatState);
    }

}
