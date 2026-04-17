using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class MoveToPoint : PointAction
{

    public MoveToPoint(string name, Sprite icon, NPC actor = null, Vector3 target = new Vector3()) : base(name: name, icon: icon, actor: actor, point: target) { }

    private Vector3 FurthestPoint(NPC actor, Vector3 target)
    {
        var path = actor.GetComponent<MoveToClick>().PathToPoint(target);
        if (path != null)
        {
            var maxdist = Math.Min(PathDistToPoint(actor, target), CombatManager.Instance.ActionPoints);
            return MoveToClick.PointAlongPath(path, maxdist);
        }
        return actor.transform.position;
    }

    private float PathDistToPoint(NPC actor, Vector3 targetPt)
    {
        var path = actor.mover.PathToPoint(targetPt);
        return MoveToClick.PathDist(path);
    }

    public override bool CheckValidAction()
    {
        Debug.Log("name " + actionName);
        Debug.Log("actor " + actor);
        Debug.Log("mover " + actor.mover);
        return !actor.mover.pathLocked;  // will get as close as possible (apply to MoveToObject too?)
    }

    public override IEnumerator UseAbility()
    {
        var targetPoint = FurthestPoint(actor, target);
        Debug.Log("Towards: " + targetPoint);
        Debug.Log("Move towards raw cost " + PathDistToPoint(actor, targetPoint));
        var cost = Mathf.CeilToInt(PathDistToPoint(actor, targetPoint) - 0.1f); //TODO: meter to AP conversion
        actor.GetComponent<MoveToClick>().SetDestination(targetPoint, true);
        while (actor.mover.IsMoving())
        {
            yield return null; // Wait for the next frame
        }

        //CombatManager.Instance.SpendActionPoints(cost); // account for floating point and wiggle room
        //CombatManager.Instance.FinishAction();
        yield break;
    }
}
