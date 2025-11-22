using NUnit.Framework.Internal;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveTo", menuName = "Scriptable Objects/MoveTo")]
public class MoveTo : AbilityAction
{

    private float PathDist(NPC actor, Selectable target)
    {
        var path = actor.GetComponent<MoveToClick>().PathToPoint(target.transform.position); // TODO: make sure that, when priority is adjusted, still going to center

        if (SelectionController.Instance.selectedItem == target && path != null) {  // Will stop prior to actually reaching the target object
            RaycastHit hit;
            Vector3 penultCorner = path.corners[^2];
            var rayDirection = path.corners[^1] - penultCorner;
            if (Physics.Raycast(penultCorner, rayDirection, out hit))
            {
                path = actor.GetComponent<MoveToClick>().PathToPoint(hit.transform.position);
            }
            return MoveToClick.PathDist(path) - actor.reach;
        }

        return MoveToClick.PathDist(path);
    }

    public override bool CheckValidTarget(NPC actor, Selectable target)
    {
        var inRange = PathDist(actor, target) <= CombatManager.Instance.ActionPoints;  // TODO: meter conversion
        return inRange;  // TODO: put in a check for special marker item
    }

    public override int UseAbility(NPC actor, Selectable target)
    {
        //target.GetComponent<groundScript>().SetTarget();

        actor.GetComponent<MoveToClick>().SetDestination(target.transform.position);  // TODO: Always use this ability to move, but party vs individual will be based on NPC state

        return Mathf.CeilToInt(PathDist(actor, target));
    }
}
