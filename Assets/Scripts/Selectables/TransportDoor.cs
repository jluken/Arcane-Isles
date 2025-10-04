using System;
using System.Collections.Generic;
using UnityEngine;

public class TransportDoor : Selectable
{
    public string toLevel;
    public int spawnPoint;

    public override void Start()
    {
        base.Start();
    }

    public override List<(string, Action)> Actions()
    {
        var acts = new List<(string, Action)>();
        acts.Add(("Go To", GoToNewLevel));
        return acts;
    }

    public void GoToNewLevel()
    {
        base.SetTarget();
        base.SetInteractAction(() => { SceneLoader.Instance.SetToLevelSpawn(toLevel, spawnPoint); });
    }
}
