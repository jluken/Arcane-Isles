using System.Collections.Generic;
using UnityEngine;

public class ActiveCombatState: CharState
{
    public ActiveCombatState(Character npc, CharStateMachine npcStateMachine, List<SelectionData> actions) : base(npc, npcStateMachine, actions)
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
        charStateMachine.ChangeState(character.IdleCombatState);
    }

    public override void EndCombat()
    {
        charStateMachine.ChangeState(character.ActiveState);
    }

    public override void EnterCombat()
    {
        // Do nothing
    }

    public override void MoveTo(Vector3 destination)
    {
        Debug.Log("Combat move");
        Debug.Log("active combatant: " + CombatManager.Instance.activeCombatant);
        CombatManager.Instance.TargetPoint(destination);
    }
}
