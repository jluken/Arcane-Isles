using System.Collections.Generic;
using UnityEngine;

public class NPCState
{
    protected NPC npc;
    protected NPCStateMachine npcStateMachine;
    protected List<SelectionData> actions;

    public NPCState(NPC npc, NPCStateMachine npcStateMachine, List<SelectionData> actions)
    {
        //Debug.Log("Define State: " + npc.charStats.charName + " " + this);
        //Debug.Log("actions: ");
        //foreach (var action in actions) {
        //    Debug.Log(action);
        //}
        this.npc = npc;
        this.npcStateMachine = npcStateMachine;
        this.actions = actions;
    }

    public virtual bool isActive => false;

    public virtual void EnterState() { Debug.Log("Enter State: " + npc.charStats.charName + " " + this); }

    public virtual void ExitState() { }

    public virtual void FrameUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void SetActiveNPC() { }

    public virtual void SetIdle() { }

    public virtual void MoveTo(Vector3 destination) { }

    public virtual void EnterCombat() {
        Debug.Log(npc.charStats.charName + "entering combat");
        npcStateMachine.ChangeState(npc.IdleCombatState);
        CombatManager.Instance.insertIntoInitiative(npc, npc.combatantType);
    }

    public List<SelectionData> GetActions()
    {
        return actions;
    }

    public virtual void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) { }

}
