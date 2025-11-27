using UnityEngine;

public class NPCState
{
    protected NPC npc;
    protected NPCStateMachine npcStateMachine;

    public NPCState(NPC npc, NPCStateMachine npcStateMachine)
    {
        this.npc = npc;
        this.npcStateMachine = npcStateMachine;
    }

    public virtual bool isActive => false;

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void FrameUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void SetActiveNPC() { }

    public virtual void SetIdle() { }

    public virtual void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) { }

}
