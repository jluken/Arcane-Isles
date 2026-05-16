using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public List<SpawnPoint> spawns;

    public string LevelName;

    public int[] worldCoords = new int[2];
    public int[] levelDims = new int[2];
    public Sprite levelMap;

    public List<string> levelScenes;

    public List<GameObject> sceneTriggers;

    public List<VisRegion> visRegions;

    void Start()
    {
        SceneLoader.Instance.SetLevel(this);

        LevelSaveData levelData = SceneLoader.Instance.GetLevelData(LevelName);
        if (levelData == null) { return; } // Nothing loaded; keep default values

        foreach (VisRegion region in visRegions)
        {
            var regionSaveData = levelData.Regions.FirstOrDefault(reg => reg.regionName == region.name);
            if (regionSaveData.regionName != null)
            {
                region.LoadFromSaveData(regionSaveData);
            }
        }
    }

    

    public List<Vector3> GetSpawnPoints(int idx)
    {
        return spawns[idx].spawnPoints.Select(obj => obj.transform.position).ToList();
    }

    public bool InsideBlockedRegion(Vector3 point)
    {
        foreach (var region in visRegions)  // TODO: combin into double Any()
        {
            if (region.regionState == VisRegion.RegionState.Undiscovered && region.ClickBlockers.Any(blocker => blocker.activeSelf && blocker.GetComponent<Collider>().bounds.Contains(point))) return true;
        }
        return false;
    }

    public bool InsideInvisibleRegion(Vector3 point)
    {
        foreach (var region in visRegions)  // TODO: combin into double Any()
        {
            if (region.regionState != VisRegion.RegionState.Visible && region.ClickBlockers.Any(blocker => blocker.activeSelf && blocker.GetComponent<Collider>().bounds.Contains(point))) return true;
        }
        return false;
    }
}
