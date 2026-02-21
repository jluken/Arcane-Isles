using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public static SceneLoader Instance { get; private set; }

    private LevelManager levelManager;


    //public List<string> ActiveLevelScenes { get; private set; }  // TODO: is this redundant with the keys of SceneObjectManagers (will just need to remove them when unloaded)? 
    private Dictionary<string, SceneSaveData> SceneData;
    public Dictionary<string, SceneObjectManager> SceneObjectManagers; // TODO: make private
    private List<string> loadingScenes = new List<string>();

    private void Awake()
    {
        Instance = this;
        //ActiveLevelScenes = new List<string>();
        
    }

    public void NewGame()
    {
        // TODO: Unload existing scenes, make sure players in right position
        Debug.Log("New Game");
        SceneData = new Dictionary<string, SceneSaveData>();
        foreach (var character in PartyController.Instance.party) { character.charStats.setInitStats(true); character.SetStates(); }  // TODO: handle more "default" save data
        PartyController.Instance.DeactivateParty();
        if (levelManager != null) DeactivateLevel(levelManager);
        levelManager = null;
        SceneObjectManagers = new Dictionary<string, SceneObjectManager>();

        UIController.Instance.ActivateMainMenu();
    }


    void Start()
    {
        NewGame();
    }

    public void SetLevel(LevelManager newManager)
    {
        levelManager = newManager;
    }

    public string GetLevelName()
    {
        return levelManager.LevelName;
    }

    public LevelManager GetLevel()
    {
        return levelManager;
    }

    public void ToMainMenu()
    {
        UIController.Instance.ActivateMainMenu();
        PartyController.Instance.DeactivateParty();
        DeactivateLevel(levelManager);
    }

    public void SetToLevelSpawn(string levelName, int spawnLoc)
    {
        var oldLevel = levelManager;
        UIController.Instance.ActivateLoadingScreen();
        //PartyController.Instance.MoveParty(levelManager.GetSpawnPoints(spawnLoc), false);
        StartCoroutine(ActivateLevel(levelName, spawnLoc));
        if (oldLevel != null && levelName != oldLevel.LevelName) StartCoroutine(DeactivateLevelCoroutine(oldLevel));
    }

    public IEnumerator ActivateLevel(string levelName, int spawnLoc = -1)
    {
        if (levelManager == null || levelManager.LevelName != levelName)
        {
            UIController.Instance.ActivateLoadingScreen();
            var levelLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            while (!levelLoad.isDone) yield return null;
        }

        Debug.Log("About to move party");
        if (spawnLoc >= 0) PartyController.Instance.MoveParty(levelManager.GetSpawnPoints(spawnLoc), true);
        PartyController.Instance.ActivateParty();
        var spawnPoints = PartyController.Instance.GetPartyLoc();

        foreach (var point in spawnPoints) Debug.Log("Spawn point: " + point.ToString());

        var activateScenes = levelManager.sceneTriggers.Where(trigger => spawnPoints.Any(spawnPoint => trigger.GetComponent<Collider>().bounds.Contains(spawnPoint))).ToList();
        var activateSceneNames = activateScenes.Select(trigger => trigger.GetComponent<SceneTrigger>().sceneName).ToList();
        foreach (var name in activateSceneNames) Debug.Log("activate scene: " + name);
        if (activateSceneNames.Any(activateScene => !SceneObjectManagers.ContainsKey(activateScene))) UIController.Instance.ActivateLoadingScreen();  // Will need to load new scenes before starting


        //List<AsyncOperation> asyncOps = new List<AsyncOperation>();
        //ActivateLevelScenes(activateSceneNames).ForEach(op => asyncOps.Add(op));
        Debug.Log("Ready to activate nearby scenes");
        foreach(var sceneName in activateSceneNames) Debug.Log(sceneName);
        while (activateSceneNames.Any(sceneName => !SceneObjectManagers.ContainsKey(sceneName))) yield return null;
        Debug.Log("nearby scenes activated");

        yield return new WaitForSeconds(0.5f);
        Debug.Log("wait over");
        UIController.Instance.ActivateDefaultScreen();
    }

    public IEnumerator DeactivateLevelCoroutine(LevelManager level)
    {
        DeactivateLevel(level);
        yield return null;
    }

    public void DeactivateLevel(LevelManager level)
    {
        //PartyController.Instance.DeactivateParty();
        Debug.Log("Deactivate Level: " + level.LevelName);
        foreach (string sceneName in level.levelScenes) { StartCoroutine(DeactivateSubscene(sceneName)); }
        SceneManager.UnloadSceneAsync(level.LevelName); ;
    }

    public IEnumerator ActivateSubscene(string sceneName)
    {
        if (!loadingScenes.Contains(sceneName)) loadingScenes.Add(sceneName);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return null;
    }

    public IEnumerator DeactivateSubscene(string sceneName)
    {
        Debug.Log("Deactivate Subscene " + sceneName);
        if (SceneObjectManagers.ContainsKey(sceneName)) {
            Debug.Log("Contains");
            var manager = SceneObjectManagers[sceneName];
            Debug.Log("Contains2");
            SceneData[sceneName] = new SceneSaveData(manager.npcs, manager.containers, manager.GroundObjects);
            Debug.Log("Contains3");
            SceneData[sceneName].loaded = false;
            Debug.Log("Unloaded");
            //ActiveLevelScenes.Remove(sceneName);
            SceneObjectManagers.Remove(sceneName);
            Debug.Log("Removed");
            SceneManager.UnloadSceneAsync(sceneName);
            Debug.Log("Async unloaded");

        }
        yield return null;

    }

    private List<AsyncOperation> ActivateLevelScenes(List<string> levelScenes)
    {
        List<AsyncOperation> asyncOps = new List<AsyncOperation>();
        foreach (string sceneName in levelScenes)
        {
            if (!SceneObjectManagers.ContainsKey(sceneName))
            {
                asyncOps.Add(SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive));
                //ActiveLevelScenes.Add(sceneName);
            }
        }
        return asyncOps;
    }

    public SceneSaveData GetSceneData(string sceneName)
    {
        if (SceneData.ContainsKey(sceneName))
        {
            return SceneData[sceneName];
        }
        else return null;
    }

    public void AddSceneManager(SceneObjectManager sceneManager)
    {
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

    //public Dictionary<string, SceneObjectManager> GetCurrentSceneManagers()
    //{
    //    //return ActiveLevelScenes.ToDictionary(scene => scene, scene => SceneObjectManagers[scene]);
    //    return SceneObjectManagers;
    //}

    public void LoadFromData(GameSaveData saveData)  //TODO: maybe goes better in SaveSystem
    {
        Debug.Log("Load from Data " + saveData.levelName);
        SceneData = saveData.SceneData;
        PartyController.Instance.InstantiateFromData(saveData.partyData);
        //Debug.Log("Party Instantiated");
        PersistentDataManager.ApplySaveData(saveData.dialogData);
        StartCoroutine(ActivateLevel(saveData.levelName));
    }

    public void SetActiveSceneNPCs(string sceneName, List<string> npcActiveNames, List<string> npcInactiveNames)
    {
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
        else if (sceneData != null) {  // Currently in loaded data
            foreach (var npc in npcActiveNames) {
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
