using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{

    public List<SpawnPoint> spawns;

    public string LevelName;

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
