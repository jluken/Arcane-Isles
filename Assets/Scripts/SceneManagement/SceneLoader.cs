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

    private Dictionary<string, SceneSaveData> SceneData;
    private Dictionary<string, SceneObjectManager> SceneObjectManagers;
    private List<string> loadingScenes = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void NewGame()
    {
        ResetData();
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

        yield return new WaitForSeconds(0.5f);
        UIController.Instance.ActivateDefaultScreen();
    }

    public IEnumerator DeactivateLevelCoroutine(LevelManager level)
    {
        DeactivateLevel(level);
        yield return null;
    }

    public void DeactivateLevel(LevelManager level)
    {
        foreach (string sceneName in level.levelScenes) { StartCoroutine(DeactivateSubscene(sceneName)); }
        SceneManager.UnloadSceneAsync(level.LevelName); ;
    }

    public IEnumerator ActivateSubscene(string sceneName)
    {
        if(loadingScenes.Contains(sceneName) || SceneObjectManagers.ContainsKey(sceneName)) yield break;
        loadingScenes.Add(sceneName);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return null;
    }

    public IEnumerator SafeSceneHandler()
    {
        var spawnPoints = PartyController.Instance.GetPartyLoc();
        var activateScenes = levelManager.sceneTriggers.Where(trigger => spawnPoints.Any(spawnPoint => trigger.GetComponent<Collider>().bounds.Contains(spawnPoint))).ToList();
        var activateSceneNames = activateScenes.Select(trigger => trigger.GetComponent<SceneTrigger>().sceneName).ToList();
        if (activateSceneNames.Any(activateScene => !SceneObjectManagers.ContainsKey(activateScene))) UIController.Instance.ActivateLoadingScreen();

        while (activateSceneNames.Any(sceneName => !SceneObjectManagers.ContainsKey(sceneName))) yield return null;

        var deadScenes = SceneObjectManagers.Keys.Where(scene => !activateSceneNames.Contains(scene)).ToList();
        int deadCount = deadScenes.Count;
        foreach (var scene in deadScenes) StartCoroutine(DeactivateSubscene(scene));
    }

    private IEnumerator DeactivateSubscene(string sceneName)
    {
        if (SceneObjectManagers.ContainsKey(sceneName)) {
            var manager = SceneObjectManagers[sceneName];
            SceneData[sceneName] = new SceneSaveData(manager.npcs, manager.containers, manager.GroundObjects);
            SceneData[sceneName].loaded = false;
            SceneObjectManagers.Remove(sceneName);
            SceneManager.UnloadSceneAsync(sceneName);
        }
        yield return null;

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

    private void ResetData()
    {
        foreach (var character in PartyController.Instance.party) { character.charStats.setInitStats(true); character.inventory.SetInitInventory(); character.SetStates(); }  // initialize chars before manipulating
        PartyController.Instance.DeactivateParty();
        if (levelManager != null) DeactivateLevel(levelManager);
        levelManager = null;
        SceneObjectManagers = new Dictionary<string, SceneObjectManager>();
        SceneData = new Dictionary<string, SceneSaveData>();
    }

    public IEnumerator LoadFromData(GameSaveData saveData)
    {
        ResetData();
        SceneData = saveData.SceneData;
        PersistentDataManager.ApplySaveData(saveData.dialogData);
        GameData.Instance.gameTime = saveData.gameTime;

        yield return InitializeLevel(saveData.levelName);
        PartyController.Instance.InstantiateFromData(saveData.partyData);
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
