using NUnit.Framework.Internal;
using System.Collections;
using System.IO;
using UnityEngine;

public class MoveToObject : InteractionAction
{
    public MoveToObject(string name, Sprite icon, float range, Character actor = null) : base(name, icon, range, actor)
    {
    }

    protected float PathDist(Character actor, Selectable target)
    {
        var path = actor.mover.PathToPoint(target.transform.position);

        if (SelectionController.Instance.selectedItem == target && path != null) {  // Will stop prior to actually reaching the target object
            RaycastHit hit;
            Vector3 penultCorner = path.corners[^2];
            var rayDirection = path.corners[^1] - penultCorner;
            if (Physics.Raycast(penultCorner, rayDirection, out hit))
            {
                path = actor.mover.PathToPoint(hit.transform.position);
            }
            return MoveToClick.PathDist(path) - actor.reach;
        }

        return MoveToClick.PathDist(path);
    }

    public override bool CheckValidAction()
    {
        return CheckValidTarget(target);
    }

    public override bool CheckValidTarget(Selectable target)
    {
        var inRange = Mathf.CeilToInt(PathDist(actor, target) / actor.charStats.runModifier) <= CombatManager.Instance.ActionPoints;
        return !actor.mover.pathLocked && inRange;
    }

    public override IEnumerator UseAbility()
    {
        var cost = PathDist(actor, target);
        actor.mover.SetDestination(target);

        while (actor.mover.IsMoving())
        {
            yield return null; // Wait for the next frame
        }
    }
}
