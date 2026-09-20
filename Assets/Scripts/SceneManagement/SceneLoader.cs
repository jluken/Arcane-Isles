using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class SceneLoader : MonoBehaviour
{

    public static SceneLoader Instance { get; private set; }

    private Dictionary<string, LevelSaveData> LevelData;
    private LevelManager levelManager;

    private Dictionary<string, SceneSaveData> SceneData;
    public Dictionary<string, SceneObjectManager> SceneObjectManagers { get; private set; }
    private List<string> loadingScenes = new List<string>();

    public SlideshowImages initCutscene;

    private void Awake()
    {
        Instance = this;
    }

    public void NewGame()
    {
        ResetData();
        UIController.Instance.ActivateMainMenu();
    }

    public void InitSpawn()  // TODO: this might go somewhere else for handling custom script behavior
    {
        SetToLevelSpawn("BeachManager", spawnLoc: 0);
        PartyController.Instance.playerChar.mover.GetDown();
        UIController.Instance.StartSlides(initCutscene);
    }


    void Start()
    {
        NewGame();
    }

    public void SetLevel(LevelManager newManager)  // Called from level activation
    {
        Debug.Log("Level set to " + newManager.LevelName);
        levelManager = newManager;
    }

    public LevelSaveData GetLevelData(string levelName)
    {
        if (LevelData.ContainsKey(levelName))
        {
            return LevelData[levelName];
        }
        else return null;
    }

    public string GetLevelName()
    {
        return levelManager.LevelName;
    }

    public LevelManager GetLevel()
    {
        return levelManager;
    }

    public Dictionary<string, LevelSaveData> GetAllLevelData()
    {
        var totalLevelData = LevelData.ToDictionary(entry => entry.Key, entry => entry.Value);
        var currentLevel = GetLevel();
        totalLevelData[currentLevel.LevelName] = new LevelSaveData(currentLevel.visRegions);
        return totalLevelData;
    }

    public SceneObjectManager GetCurrentSceneManager(GameObject obj)
    {
        if (!SceneObjectManagers.ContainsKey(obj.scene.name)) Debug.LogError(obj.scene.name + " not present in SceneObjectManagers");
        return SceneObjectManagers[obj.scene.name];
    }

    public void ToMainMenu()
    {
        ResetData();
        UIController.Instance.ActivateMainMenu();
    }

    public void SetToLevelSpawn(string levelName, int spawnLoc) // TODO: decide how best to handle spawn points. Possibly use key names instead of idx ints
    {
        var oldLevel = levelManager;
        UIController.Instance.ActivateLoadingScreen();
        StartCoroutine(ActivateLevel(levelName, spawnLoc));
        if (oldLevel != null && levelName != oldLevel.LevelName) StartCoroutine(DeactivateLevelCoroutine(oldLevel));
    }

    public IEnumerator InitializeLevel(string levelName)
    {
        if (levelManager == null || levelManager.LevelName != levelName)
        {
            UIController.Instance.ActivateLoadingScreen();
            var levelLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            while (!levelLoad.isDone) yield return null;
        }
    }

    public IEnumerator ActivateLevel(string levelName, int spawnLoc = -1)
    {
        yield return InitializeLevel(levelName);

        if (spawnLoc >= 0) PartyController.Instance.MoveParty(levelManager.GetSpawnPoints(spawnLoc), true);
        PartyController.Instance.ActivateParty();
        yield return SafeSceneHandler();
        Debug.Log("Safe scene handler done");

        yield return new WaitForSecondsRealtime(0.5f);
        Debug.Log("Load wait done");
        UIController.Instance.ActivateDefaultScreen();
    }

    public IEnumerator DeactivateLevelCoroutine(LevelManager level)
    {
        DeactivateLevel(level);
        yield return null;
    }

    public void DeactivateLevel(LevelManager level)
    {
        SaveSystem.AutoSave();
        LevelData[level.LevelName] = new LevelSaveData(level.visRegions);
        foreach (string sceneName in level.levelScenes) { StartCoroutine(DeactivateSubscene(sceneName)); }
        SceneManager.UnloadSceneAsync(level.LevelName); ;
    }

    public IEnumerator ActivateSubscene(string sceneName)
    {
        Debug.Log("Activating subscene " + sceneName);
        if(loadingScenes.Contains(sceneName) || SceneObjectManagers.ContainsKey(sceneName)) yield break;
        loadingScenes.Add(sceneName);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return null;
    }

    public IEnumerator SafeSceneHandler()
    {
        var spawnPoints = PartyController.Instance.GetPartyColliders();
        var activateScenes = levelManager.sceneTriggers.Where(trigger => spawnPoints.Any(spawnPoint => trigger.GetComponent<Collider>().bounds.Intersects(spawnPoint.bounds))).ToList();
        var activateSceneNames = activateScenes.Select(trigger => trigger.GetComponent<SceneTrigger>().sceneName).ToList();
        Debug.Log("Activate scenes: " + string.Join(", ", activateSceneNames));
        foreach (var sceneName in activateSceneNames) { StartCoroutine(ActivateSubscene(sceneName)); }// TODO: possibly unnecessary assurance scenes get triggered (even if party starts in trigger)
        //if (activateSceneNames.Any(activateScene => !SceneObjectManagers.ContainsKey(activateScene))) UIController.Instance.ActivateLoadingScreen();

        while (activateSceneNames.Any(sceneName => !SceneObjectManagers.ContainsKey(sceneName)))
        {
            yield return null;
        }
        Debug.Log("scenes activated");

        var deadScenes = SceneObjectManagers.Keys.Where(scene => !activateSceneNames.Contains(scene)).ToList();
        Debug.Log("Dead scenes: " + string.Join(", ", deadScenes));
        int deadCount = deadScenes.Count;
        foreach (var scene in deadScenes) StartCoroutine(DeactivateSubscene(scene));
    }

    public List<string> ScenesByLoc(Vector3 loc)
    {
        var scenes = levelManager.sceneTriggers.Where(trigger => trigger.GetComponent<Collider>().bounds.Contains(loc)).ToList();
        return scenes.Select(trigger => trigger.GetComponent<SceneTrigger>().sceneName).ToList();
    }

    private IEnumerator DeactivateSubscene(string sceneName)
    {
        if (SceneObjectManagers.ContainsKey(sceneName)) {
            var manager = SceneObjectManagers[sceneName];
            Debug.Log("Adding scene Data " + manager.sceneName + " with droppable " + (manager.GroundObjects.Count > 0 ? manager.GroundObjects[0].name : "null"));
            SceneData[sceneName] = new SceneSaveData(manager.npcs, manager.containers, manager.GroundObjects);
            SceneData[sceneName].loaded = false;
            SceneObjectManagers.Remove(sceneName);
            SceneManager.UnloadSceneAsync(sceneName);
        }
        yield return null;

    }

    public SceneSaveData GetSceneData(string sceneName)
    {
        Debug.Log("Getting scene data for " + sceneName);
        if (SceneData.ContainsKey(sceneName))
        {
            Debug.Log("Found scene data for " + sceneName);
            return SceneData[sceneName];
        }
        else return null;
    }

    public void AddSceneManager(SceneObjectManager sceneManager)
    {
        Debug.Log("Adding scene Manager " + sceneManager.sceneName + " with droppable " + (sceneManager.GroundObjects.Count > 0 ? sceneManager.GroundObjects[0].name : "null"));
        loadingScenes.Remove(sceneManager.sceneName);
        SceneObjectManagers[sceneManager.sceneName] = sceneManager;
    }

    public bool SceneLoaded(string sceneName)
    {
        return SceneObjectManagers.ContainsKey(sceneName) || loadingScenes.Contains(sceneName);
    }

    public Dictionary<string, SceneSaveData> GetAllSceneData()
    {
        var totalSceneData = SceneData.ToDictionary(entry => entry.Key, entry => entry.Value);
        foreach (var activeScene in SceneObjectManagers.Keys)
        {
            var manager = SceneObjectManagers[activeScene];
            totalSceneData[activeScene] = new SceneSaveData(manager.npcs, manager.containers, manager.GroundObjects);
            totalSceneData[activeScene].loaded = true;
        }
        return totalSceneData;
    }

    private void ResetData()
    {
        Debug.Log("Reset data");
        //var mainChar = PartyController.Instance.playerChar;
        //mainChar.charStats.setInitStats(true);
        //mainChar.inventory.SetInitInventory();
        //StartCoroutine(mainChar.mover.DefaultAvoidanceAsync());
        //mainChar.SetStates();
        foreach (var character in PartyController.Instance.party) {
            character.charStats.setInitStats(true); 
            character.inventory.SetInitInventory();
            Debug.Log("Reset default");
            StartCoroutine(character.mover.DefaultAvoidanceAsync()); 
            character.SetStates(); }  // initialize chars before manipulating
        PartyController.Instance.DeactivateParty();
        if (levelManager != null) DeactivateLevel(levelManager);
        Debug.Log("level manager reset");
        levelManager = null;
        SceneObjectManagers = new Dictionary<string, SceneObjectManager>();
        LevelData = new Dictionary<string, LevelSaveData>();
        SceneData = new Dictionary<string, SceneSaveData>();
    }

    public IEnumerator LoadFromData(GameSaveData saveData)
    {
        ResetData();
        LevelData = saveData.LevelData;
        SceneData = saveData.SceneData;
        Debug.Log("Load dialog saves: " + saveData.dialogData);
        PersistentDataManager.ApplySaveData(saveData.dialogData);
        GameData.Instance.gameTime = saveData.gameTime;

        yield return InitializeLevel(saveData.levelName);
        PartyController.Instance.InstantiateFromData(saveData.partyData);
        if(saveData.MapData != null) UIController.Instance.InstantiateMapsFromData(saveData.MapData); // TODO: safety exception
        yield return ActivateLevel(saveData.levelName);
    }

    public void SetActiveSceneNPCs(string sceneName, List<string> npcActiveNames, List<string> npcInactiveNames)
    {
        // Call from action/dialog trigger that affects scene states
        var sceneData = GetSceneData(sceneName);

        if (SceneObjectManagers.ContainsKey(sceneName))  // currently active
        {
            foreach (var npc in npcActiveNames)
            {
                SceneObjectManagers[sceneName].EnableDisableNPC(npc, true);
            }
            foreach (var npc in npcInactiveNames)
            {
                SceneObjectManagers[sceneName].EnableDisableNPC(npc, false);
            }
        }
        else if (sceneData != null)
        {  // Currently in loaded data
            foreach (var npc in npcActiveNames)
            {
                sceneData.NPCs.Where(npcdata => npcdata.id == npc).ToList().ForEach(npcdata => npcdata.active = true);
            }
            foreach (var npc in npcInactiveNames)
            {
                sceneData.NPCs.Where(npcdata => npcdata.id == npc).ToList().ForEach(npcdata => npcdata.active = false);
            }
        }
        else // never been loaded yet
        {
            SceneData[sceneName] = new SceneSaveData(npcActiveNames, npcInactiveNames);  // TODO: maybe protect this better
        }
    }
}
