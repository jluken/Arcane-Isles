using NUnit.Framework.Internal;
using System.Collections;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveTo", menuName = "Scriptable Objects/MoveTo")]
public class MoveTo : AbilityAction
{

    protected float PathDist(NPC actor, Selectable target)
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

    public override bool CheckValidTarget(NPC actor, Selectable target)
    {
        var inRange = Mathf.CeilToInt(PathDist(actor, target)) <= CombatManager.Instance.ActionPoints;  // TODO: meter conversion
        Debug.Log("Check in range: " + inRange);
        Debug.Log("locked: " + actor.mover.pathLocked);
        return !actor.mover.pathLocked && inRange;
    }

    public override IEnumerator UseAbility(NPC actor, Selectable target)
    {
        var cost = PathDist(actor, target);
        Debug.Log("Move to raw cost " + cost);
        actor.mover.SetDestination(target.transform.position, true);

        while (actor.mover.IsMoving())
        {
            yield return null; // Wait for the next frame
        }
        Debug.Log("Move to done");
        CombatManager.Instance.SpendActionPoints(Mathf.CeilToInt(cost - 0.1f)); // account for floating point and wiggle room
        CombatManager.Instance.FinishAction();
    }
}
