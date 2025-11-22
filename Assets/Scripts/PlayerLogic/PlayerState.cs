using UnityEngine;

public class PlayerState
{
    protected PartyMember player;
    protected PlayerStateMachine playerStateMachine;

    public PlayerState(PartyMember player, PlayerStateMachine playerStateMachine)
    {
        this.player = player;
        this.playerStateMachine = playerStateMachine;
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void FrameUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) { }

    public virtual void TravelToPoint(Vector3 point) { }

    public virtual void TravelToItem(Selectable item) { }

}
