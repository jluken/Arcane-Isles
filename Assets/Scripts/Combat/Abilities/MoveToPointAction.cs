using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class MoveToPoint : PointAction
{

    public MoveToPoint(string name, Sprite icon, Character actor = null, Vector3 target = new Vector3()) : base(name: name, icon: icon, actor: actor, point: target) { }

    private Vector3 FurthestPoint(Character actor, Vector3 target)
    {
        var path = actor.GetComponent<MoveToClick>().PathToPoint(target);
        if (path != null)
        {
            var maxdist = Math.Min(PathDistToPoint(actor, target), CombatManager.Instance.ActionPoints / actor.charStats.runModifier);
            return MoveToClick.PointAlongPath(path, maxdist);
        }
        return actor.transform.position;
    }

    private float PathDistToPoint(Character actor, Vector3 targetPt)
    {
        var path = actor.mover.PathToPoint(targetPt);
        return MoveToClick.PathDist(path);
    }

    public override bool CheckValidAction()
    {
        return !actor.mover.pathLocked;  // will get as close as possible (apply to MoveToObject too?)
    }

    public override IEnumerator UseAbility()
    {
        var targetPoint = FurthestPoint(actor, target);
        actor.GetComponent<MoveToClick>().SetDestination(targetPoint);
        while (actor.mover.IsMoving())
        {
            yield return null; // Wait for the next frame
        }

        yield break;
    }
}
