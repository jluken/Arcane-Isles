using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{

    public static string saveFolder = "saves";
    private static string saveFormat = ".sav";

    private static string newSaveSlot = "saveNum";
    private static string autoSaveSlot = "autoNum";
    private static int autoSaves = 3;

    public static void AutoSave()
    {
        int prevAutoSaveNum = PlayerPrefs.GetInt(autoSaveSlot, 0);
        int newAutoNum = (prevAutoSaveNum + 1) % autoSaves;
        string autosaveName = "Autosave " + (newAutoNum + 1);
        SaveGame(autosaveName);
        PlayerPrefs.SetInt(autoSaveSlot, newAutoNum);
    }

    public static string GetNextSave()
    {
        int newSaveNum = PlayerPrefs.GetInt(newSaveSlot, 1);
        return "New Save " + newSaveNum;
    }

    public static void SaveGame(string saveName)
    {
        Directory.CreateDirectory(Application.persistentDataPath + "/" + saveFolder);
        var partyController = PartyController.Instance;
        var sceneLoader = SceneLoader.Instance;
        BinaryFormatter formatter = new BinaryFormatter();

        if(saveName == null || saveName == "")
        {
            int newSaveNum = PlayerPrefs.GetInt(newSaveSlot, 1);
            saveName = "New Save " + newSaveNum;
            PlayerPrefs.SetInt(newSaveSlot, newSaveNum + 1);
        }

        string path = Application.persistentDataPath + "/" + saveFolder + "/" + saveName + saveFormat;
        FileStream stream = new FileStream(path, FileMode.Create);

        PartyData partyData = new PartyData(partyController);
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.partyData = partyData;
        gameSaveData.levelName = sceneLoader.GetLevelName();
        gameSaveData.SceneData = sceneLoader.GetAllSceneData();
        gameSaveData.dialogData = PersistentDataManager.GetSaveData();
        gameSaveData.gameTime = GameData.Instance.gameTime;
        gameSaveData.saveName = saveName;
        gameSaveData.saveTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

        formatter.Serialize(stream, gameSaveData);
        stream.Close();
    }

    public static GameSaveData LoadGame(string saveName)
    {
        Directory.CreateDirectory(Application.persistentDataPath + "/" + saveFolder);
        string path = Application.persistentDataPath + "/" + saveFolder + "/" + saveName + saveFormat;
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

    public static void DeleteSave(string saveName)
    {
        if (saveName != null && GetSaves().Contains(saveName)) File.Delete(Application.persistentDataPath + "/" + saveFolder + "/" + saveName + saveFormat);
    }

    public static List<string> GetSaves()
    {
        Directory.CreateDirectory(Application.persistentDataPath + "/" + saveFolder);
        var files = Directory.GetFiles(Application.persistentDataPath + "/" + saveFolder).Select(Path.GetFileName).ToList();
        return files.Select(s => s.EndsWith(saveFormat) ? s.Substring(0, s.Length - saveFormat.Length) : s).ToList();
    }

    //public static void SaveParty(PartyController partyController)
    //{
    //    BinaryFormatter formatter = new BinaryFormatter();

    //    string path = Application.persistentDataPath + "/partyData.sav";
    //    FileStream stream = new FileStream(path, FileMode.Create);

    //    PartyData data = new PartyData(partyController);

    //    formatter.Serialize(stream, data);
    //    stream.Close();
    //}

    //public static PartyData LoadParty()
    //{
    //    string path = Application.persistentDataPath + "/partyData.sav";
    //    if (File.Exists(path))
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        FileStream stream = new FileStream(path, FileMode.Open);

    //        PartyData data = formatter.Deserialize(stream) as PartyData;

    //        return data;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}
}
