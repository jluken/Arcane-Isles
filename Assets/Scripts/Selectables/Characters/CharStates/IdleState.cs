using System.Collections.Generic;
using UnityEngine;

public class IdleState: CharState
{
    public IdleState(Character npc, CharStateMachine npcStateMachine, List<SelectionData> actions) : base(npc, npcStateMachine, actions)
    {
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void SetActiveChar() { 
        charStateMachine.ChangeState(character.ActiveState);
    }
}
