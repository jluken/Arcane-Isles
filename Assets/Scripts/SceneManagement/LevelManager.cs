using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{

    public List<SpawnPoint> spawns;

    public string LevelName;

    public int[] worldCoords = new int[2];
    public int[] levelDims = new int[2];
    public Sprite levelMap;

    public List<string> levelScenes;

    public List<GameObject> sceneTriggers;

    void Awake()
    {
        SceneLoader.Instance.SetLevel(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Vector3> GetSpawnPoints(int idx)
    {
        return spawns[idx].spawnPoints.Select(obj => obj.transform.position).ToList();
    }
}
