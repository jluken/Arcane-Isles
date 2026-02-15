using System.Collections.Generic;
using UnityEngine;

public class ActiveCombatState: NPCState
{
    public ActiveCombatState(NPC npc, NPCStateMachine npcStateMachine, List<SelectionData> actions) : base(npc, npcStateMachine, actions)
    {
    }

    public override bool isActive => true;

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        npc.mover.agent.avoidancePriority = 80; // TODO: store these priorities somewhere
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

    public override void MoveTo(Vector3 destination)
    {
        Selectable target;
        if (SelectionController.Instance.selectedItem != null) target = SelectionController.Instance.selectedItem;
        else target = CombatManager.Instance.activeCombatant.GetComponent<MoveToClick>().SetTempMarker(destination); // TODO: make into an agnostic event with no move destination hack
        CombatManager.Instance.UseCombatAbility(target, CombatManager.CombatActionType.Run);
    }
}
