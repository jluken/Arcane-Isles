using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSaveData
{
    [System.Serializable]
    public struct RegionData
    {
        public string regionName;
        public bool discovered;
    }

    public List<RegionData> Regions;

    public LevelSaveData(List<VisRegion> regions)
    {
        Regions = new List<RegionData>();
        foreach (var region in regions)
        {
            var regionData = new RegionData();
            regionData.regionName = region.name;
            regionData.discovered = region.regionState != VisRegion.RegionState.Undiscovered;
            Regions.Add(regionData);
        }
    }
}
