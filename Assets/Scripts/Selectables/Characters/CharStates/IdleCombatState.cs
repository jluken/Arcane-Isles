using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleCombatState: CharState
{
    public IdleCombatState(Character npc, CharStateMachine playerStateMachine, List<SelectionData> actions) : base(npc, playerStateMachine, actions)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        character.mover.StopMoving();
        //character.mover.PlantFeet();
        base.EnterState();
    }

    public override void ExitState()
    {
        //character.mover.DefaultAvoidance();
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

    public override void SetActiveChar()
    {
        charStateMachine.ChangeState(character.ActiveCombatState);
    }

    public override void EndCombat()
    {
        Debug.Log("Ending combat for " + character.name + " into state " + character.IdleState);
        charStateMachine.ChangeState(character.IdleState);
    }

    public override void EnterCombat()
    {
        // Do nothing
        CombatManager.Instance.insertIntoInitiative(character, character.combatantType);
    }

}
