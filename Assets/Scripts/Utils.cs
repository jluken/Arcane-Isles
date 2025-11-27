using Unity.Mathematics;
using UnityEngine;

public class Utils
{
    public static bool LineOfSight(GameObject origin, GameObject target)
    {
        RaycastHit hit;
        var rayDirection = target.transform.position - origin.transform.position;
        if (Physics.Raycast(origin.transform.position, rayDirection, out hit) && (hit.transform == target.transform))
        {
                return true;
        }
        return false;
    }
}
