using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Utils
{
    public static bool LineOfSight(GameObject origin, GameObject target)
    {
        return LineOfSight(origin, target.transform.position);
        //RaycastHit hit;
        //int barrier = LayerMask.NameToLayer("Barrier");
        //if (target == null) return false;
        //var rayDirection = target.transform.position - origin.transform.position;
        //if (Physics.Raycast(origin.transform.position, rayDirection, out hit, math.INFINITY, barrier) && (hit.transform == target.transform))
        //{
        //        return true;
        //}
        //return false;
    }

    public static bool LineOfSight(GameObject origin, Vector3 target)
    {
        int barrier = LayerMask.NameToLayer("Barrier");
        return !Physics.Linecast(origin.transform.position, target, barrier);
    }
}
