using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VectorGraphics;
using UnityEngine;
using static LevelSaveData;

public class VisRegion : MonoBehaviour
{

    public enum RegionState
    {
        Undiscovered,
        Unseen,
        Visible
    }

    public RegionState regionState { get; private set; }

    public List<VisMask> VisMasks;

    public List<GameObject> ClickBlockers;

    public void Awake()
    {
        regionState = RegionState.Undiscovered;
    }

    public void Start()
    {
        foreach (VisMask mask in VisMasks)
        {
            mask.ConnectRegion(this);
            mask.SetHidden();
        }
    }

    public void LoadFromSaveData(RegionData regionData)
    {
        regionState = regionData.discovered ? RegionState.Unseen : RegionState.Undiscovered;
        if (regionState != RegionState.Undiscovered) { foreach (VisMask mask in VisMasks) mask.Discover(); }
    }

    public void SetSeen()
    {
        regionState = RegionState.Visible;
        foreach (VisMask mask in VisMasks) mask.SetSeen();
        // TODO: register masks/blockers with level to save and also keep around "partially" (separate mask that doesn't block clicks, or just disable that?)
    }

    public void SetHidden()
    {
        if (regionState == RegionState.Undiscovered) return;
        regionState = RegionState.Unseen;
        foreach (VisMask mask in VisMasks) mask.SetHidden();
        // TODO: register masks/blockers with level to save and also keep around "partially" (separate mask that doesn't block clicks, or just disable that?)
    }
}
