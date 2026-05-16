using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static VisRegion;

public class VisMask : MonoBehaviour
{
    public RegionState state;

    public Material black;
    public Material shadow;
    public Material clear;

    private HashSet<VisRegion> connectedRegions;

    void Awake()
    {
        state = RegionState.Undiscovered;
        GetComponent<Renderer>().material = black;
        connectedRegions = new HashSet<VisRegion>();
    }

    public void ConnectRegion(VisRegion region)
    {
        connectedRegions.Add(region);
    }

    public void SetSeen()
    {
        state = RegionState.Visible;
        GetComponent<Renderer>().material = clear;
    }

    public void Discover()
    {
        state = RegionState.Unseen;
        GetComponent<Renderer>().material = shadow;
    }

    public void SetHidden()
    {
        if (state == RegionState.Undiscovered || connectedRegions.Any(reg => reg.regionState == RegionState.Visible)) return;
        state = RegionState.Unseen;
        GetComponent<Renderer>().material = shadow;
    }
}
