using System.Net;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Utils
{
    public static bool LineOfSight(GameObject origin, GameObject target)
    {
        return LineOfSight(origin, target.transform.position);
    }

    public static bool LineOfSight(GameObject origin, Vector3 target)
    {
        int barrier = 1 << LayerMask.NameToLayer("Barrier");
        return !Physics.Linecast(origin.transform.position, target, barrier);
    }

    public static Vector3 GroundPoint(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        Vector3 center = rend.bounds.center;

        return new Vector3(center.x, rend.bounds.min.y, center.z);
    }
}
