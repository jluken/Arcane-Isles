using System;
using System.Collections.Generic;
using UnityEngine;

public class UnrecruitedState : IdleState
{
    public UnrecruitedState(Character npc, CharStateMachine npcStateMachine, List<SelectionData> actions) : base(npc, npcStateMachine, actions)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        character.mover.PlantFeet();
        base.EnterState();
    }

    public override void ExitState()
    {
        character.mover.DefaultAvoidance();
        base.ExitState();
    }

    public override void SetIdle() {
        charStateMachine.ChangeState(character.IdleState);
    }

    public override void EnterCombat()
    {
        //npcStateMachine.ChangeState(npc.IdleCombatState);
        CombatManager.Instance.insertIntoInitiative(character, CombatManager.CombatantType.Bystander);
    }

    public override void SetActiveChar()
    {
        throw new Exception("Cannot set unrecruited as active");
    }
}
