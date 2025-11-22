using NUnit.Framework.Internal;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "MoveTo", menuName = "Scriptable Objects/MoveTowards")]
public class MoveTowards : AbilityAction
{

    private Vector3 FurthestPoint(NPC actor, Selectable target)
    {
        var path = actor.GetComponent<MoveToClick>().PathToPoint(target.transform.position); // TODO: make sure that, when priority is adjusted, still going to center

        if (SelectionController.Instance.selectedItem == target && path != null)
        {  // Will stop prior to actually reaching the target object
            RaycastHit hit;
            Vector3 penultCorner = path.corners[^2];
            var rayDirection = path.corners[^1] - penultCorner;
            if (Physics.Raycast(penultCorner, rayDirection, out hit))
            {
                path = actor.GetComponent<MoveToClick>().PathToPoint(hit.transform.position);
            }
            return MoveToClick.PointAlongPath(path, CombatManager.Instance.ActionPoints); //TODO: meter conversion
        }

        return path != null ? path.corners[^1] : actor.transform.position;
    }

    private float PathDist(NPC actor, Vector3 targetPt)
    {
        var path = actor.GetComponent<MoveToClick>().PathToPoint(targetPt);
        return MoveToClick.PathDist(path);
    }

    public override bool CheckValidTarget(NPC actor, Selectable target)
    {
        return true;
    }

    public override int UseAbility(NPC actor, Selectable target)
    {
        //target.GetComponent<groundScript>().SetTarget();

        actor.GetComponent<MoveToClick>().SetDestination(FurthestPoint(actor, target));

        return Mathf.CeilToInt(PathDist(actor, FurthestPoint(actor, target))); //TODO: meter to AP conversion
    }
}
