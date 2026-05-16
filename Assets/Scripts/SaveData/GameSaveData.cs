using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public string saveName;
    public string saveTime;

    public PartyData partyData;
    public string levelName;
    public Dictionary<string, LevelSaveData> LevelData;
    public Dictionary<string, SceneSaveData> SceneData;
    public string dialogData;
    public float gameTime;
}
