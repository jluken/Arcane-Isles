using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeadState : CharState
{
    public DeadState(Character npc, CharStateMachine playerStateMachine, List<SelectionData> actions) : base(npc, playerStateMachine, actions)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        character.mover.StopMoving();
        character.mover.PlantFeet();
        base.EnterState();
    }

    public override void ExitState()
    {
        character.mover.DefaultAvoidance();
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

    public override void EnterCombat()
    {
    }

    public override void SetActiveChar()
    {
        throw new Exception("Cannot set dead Char as active");
    }

}
