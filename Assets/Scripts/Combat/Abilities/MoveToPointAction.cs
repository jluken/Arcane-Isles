using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class MoveToPoint : AbilityAction  // TODO: possibly rethink movetopoint logic
{
    Vector3 targetPt;
    Selectable targetObj;

    public MoveToPoint(string name, Sprite icon, Character actor = null, Selectable targetObj = null, Vector3 target = new Vector3()) : base(name: name, icon: icon, actor: actor) {
        if (targetObj != null) this.targetObj = targetObj;
        else targetPt = target;
    }

    public override void SetTarget(Vector3 point)
    {
        targetPt = point;
        targetObj = null;
    }

    public override void SetTarget(Selectable target)
    {
        targetObj = target;
        targetPt = Vector3.zero;
    }

    //private Vector3 FurthestPoint(Character actor, Vector3 target)
    //{
    //    var path = actor.GetComponent<MoveToClick>().PathToPoint(target);
    //    if (path != null)
    //    {
    //        Debug.Log("moveto Path is not null");

    //        var maxdist = Math.Min(PathDistToPoint(actor, target), CombatManager.Instance.ActionPoints * actor.charStats.runModifier);
    //        Debug.Log("runMod: " + actor.charStats.runModifier);
    //        Debug.Log("maxdist: " + maxdist);
    //        return MoveToClick.PointAlongPath(path, maxdist);
    //    }
    //    Debug.Log("moveto Path is null");
    //    return actor.transform.position;
    //}

    private float PathDistToTarget(Character actor)
    {
        var path = targetObj != null ? actor.mover.PathToObj(targetObj) : actor.mover.PathToPoint(targetPt);
        return MoveToClick.PathDist(path);
    }

    public override bool CheckValidAction()
    {
        return !actor.mover.pathLocked;
    }

    public override IEnumerator UseAbility()
    {
        //Debug.Log("Using MoveToPoint for point " + target);
        //var targetPoint = FurthestPoint(actor, target);
        //CombatManager.Instance.LockAction(this);
        //actor.mover.SetDestination(targetPoint);
        if (targetObj != null) actor.mover.SetDestination(targetObj);
        else actor.mover.SetDestination(targetPt);
        while (actor.mover.IsMoving())
        {  
            yield return null; // Wait for the next frame
        }
        Debug.Log("Done moving moveToPt");
        CombatManager.Instance.FinishAction();  // handled by StopAction?
        yield break;
    }

    public override int GetActionCost()
    {
        return (int)Math.Ceiling(PathDistToTarget(actor) / actor.charStats.runModifier);
    }

    public override void DisplayTarget()
    {
        if (!CheckValidAction()) return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        if (targetObj != null) actor.mover.DrawTo(targetObj);
        else actor.mover.DrawTo(targetPt);
    }
}
