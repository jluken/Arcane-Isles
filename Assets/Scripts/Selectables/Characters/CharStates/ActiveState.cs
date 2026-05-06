using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActiveState : CharState
{
    public ActiveState(Character npc, CharStateMachine playerStateMachine, List<SelectionData> actions) : base(npc, playerStateMachine, actions)
    {
    }

    public override bool isActive => true;

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

    public override void SetIdle()
    {
        charStateMachine.ChangeState(character.IdleState);
    }

    public override void MoveTo(Vector3 destination)
    {
        //npc.mover.SetDestination(destination);
        PartyController.Instance.SetPartyDestination(destination);
    }
}
