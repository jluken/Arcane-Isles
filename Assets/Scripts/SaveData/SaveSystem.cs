using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveGame(PartyController partyController, SceneLoader sceneLoader)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/gameData.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        PartyData partyData = new PartyData(partyController);
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.partyData = partyData;
        gameSaveData.levelName = sceneLoader.GetLevel();
        gameSaveData.SceneData = sceneLoader.GetAllSceneData();
        gameSaveData.dialogData = PersistentDataManager.GetSaveData();

        formatter.Serialize(stream, gameSaveData);
        stream.Close();
    }

    public static GameSaveData LoadGame()
    {
        string path = Application.persistentDataPath + "/gameData.sav";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameSaveData data = formatter.Deserialize(stream) as GameSaveData;
            stream.Close();
              
            return data;
        }
        else
        {
            //Debug.LogError("Save File not found");
            return null;
        }
    }

    public static void SaveParty(PartyController partyController)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/partyData.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        PartyData data = new PartyData(partyController);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PartyData LoadParty()
    {
        string path = Application.persistentDataPath + "/partyData.sav";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PartyData data = formatter.Deserialize(stream) as PartyData;

            return data;
        }
        else
        {
            //Debug.LogError("Save File not found");
            return null;
        }
    }
}
