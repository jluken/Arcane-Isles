using NUnit.Framework;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class VisTarget : MonoBehaviour
{

    public List<VisRegion> VisRegions;

    public bool active; // use this to check if already active (ontriggerenter/stay/exit)

    public void TriggerTarget()
    {
        foreach (var region in VisRegions)
        {
            region.SetSeen();
        }
        //TODO: Have character field detect visTarget, check for line of sight, then if true call Trigger Target (find way to reduce the calls to this because expensive)
    }

    public void SetUnseen()
    {
        foreach (var region in VisRegions)
        {
            region.SetHidden();
        }
    }
}
