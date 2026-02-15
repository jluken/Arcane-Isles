using System.Collections.Generic;
using UnityEngine;

public class IdleState: NPCState
{
    public IdleState(NPC npc, NPCStateMachine npcStateMachine, List<SelectionData> actions) : base(npc, npcStateMachine, actions)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        npc.mover.agent.avoidancePriority = 40;
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

    public override void SetActiveNPC() { 
        npcStateMachine.ChangeState(npc.ActiveState);
    }
}
