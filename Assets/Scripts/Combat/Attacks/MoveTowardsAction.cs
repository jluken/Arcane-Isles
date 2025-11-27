using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "MoveTowards", menuName = "Scriptable Objects/MoveTowards")]
public class MoveTowards : MoveTo
{

    private Vector3 FurthestPoint(NPC actor, Selectable target)
    {
        var path = actor.GetComponent<MoveToClick>().PathToPoint(target.transform.position);
        if (path != null)
        {
            var maxdist = Math.Min(PathDist(actor, target), CombatManager.Instance.ActionPoints); //TODO: meter to AP conversion
            return MoveToClick.PointAlongPath(path, maxdist);
        }
        return actor.transform.position;
    }

    private float PathDistToPoint(NPC actor, Vector3 targetPt)
    {
        var path = actor.mover.PathToPoint(targetPt);
        return MoveToClick.PathDist(path);
    }

    public override bool CheckValidTarget(NPC actor, Selectable target)
    {
        return !actor.mover.pathLocked;
    }

    public override IEnumerator UseAbility(NPC actor, Selectable target)
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

        CombatManager.Instance.SpendActionPoints(cost); // account for floating point and wiggle room
        CombatManager.Instance.FinishAction();
    }
}
