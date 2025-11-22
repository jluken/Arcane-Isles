using UnityEngine;

public class ActiveCombatState: PlayerState
{
    public ActiveCombatState(PartyMember player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

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

    //public override void TravelToPoint(Vector3 point)
    //{
    //    var travelPath = player.charObject.GetComponent<MoveToClick>().PathToPoint(point);
    //    // TODO: create temporary target item to pass to the MoveTo ability action (final corner)
    //}

    //public override void TravelToItem(Selectable item)
    //{
    //    var initPath = player.charObject.GetComponent<MoveToClick>().PathToPoint(item.transform.position);
    //    if (initPath != null)
    //    {
    //        RaycastHit hit;
    //        Vector3 penultCorner = initPath.corners[^2];
    //        var rayDirection = item.gameObject.transform.position - penultCorner;
    //        if (Physics.Raycast(penultCorner, rayDirection, out hit))
    //        {
    //            TravelToPoint(hit.transform.position);
    //        }
    //    }
    //}
}
