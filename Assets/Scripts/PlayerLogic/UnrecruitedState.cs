using System;
using System.Collections.Generic;
using UnityEngine;

public class UnrecruitedState : IdleState
{
    public UnrecruitedState(NPC npc, NPCStateMachine npcStateMachine, List<SelectionData> actions) : base(npc, npcStateMachine, actions)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        npc.mover.agent.avoidancePriority = 20;
        base.EnterState();
    }

    public override void SetIdle() {
        npcStateMachine.ChangeState(npc.IdleState);
    }

    public override void EnterCombat()
    {
        //npcStateMachine.ChangeState(npc.IdleCombatState);
        CombatManager.Instance.insertIntoInitiative(npc, CombatManager.CombatantType.Bystander); // TODO: determine behavior if combat starts near unrecruited companion
    }

    public override void SetActiveNPC()
    {
        throw new Exception("Cannot set unrecruited as active"); // TODO: determine behavior if combat starts near unrecruited companion
    }
}
