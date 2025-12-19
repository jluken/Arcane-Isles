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

    public MainMenu mainMenu;
    public LoadingScreen loadingScreen;

    private LevelManager levelManager;


    //public List<string> ActiveLevelScenes { get; private set; }  // TODO: is this redundant with the keys of SceneObjectManagers (will just need to remove them when unloaded)? 
    private Dictionary<string, SceneSaveData> SceneData;
    public Dictionary<string, SceneObjectManager> SceneObjectManagers;

    private void Awake()
    {
        Instance = this;
        //ActiveLevelScenes = new List<string>();
        SceneData = new Dictionary<string, SceneSaveData>();
        levelManager = null;
        SceneObjectManagers = new Dictionary<string, SceneObjectManager>();
        mainMenu.ActivateMenu();
    }


    void Start()
    {
    }

    public void SetLevel(LevelManager newManager)
    {
        levelManager = newManager;
    }

    public string GetLevel()
    {
        return levelManager.LevelName;
    }

    public void ToMainMenu()
    {
        mainMenu.ActivateMenu();
        UIController.Instance.DeactivateAllMenus();
        UIController.Instance.AllowMenus(false);
        PartyController.Instance.DeactivateParty();
        StartCoroutine(DeactivateLevel(levelManager));
    }

    public void SetToLevelSpawn(string levelName, int spawnLoc)
    {
        var oldLevel = levelManager;
        SetLoadingScreen();
        //PartyController.Instance.MoveParty(levelManager.GetSpawnPoints(spawnLoc), false);
        StartCoroutine(ActivateLevel(levelName, spawnLoc));
        if (oldLevel != null && levelName != oldLevel.LevelName) DeactivateLevel(oldLevel);
    }

    public IEnumerator ActivateLevel(string levelName, int spawnLoc = -1)
    {
        if (levelManager == null || levelManager.LevelName != levelName)
        {
            SetLoadingScreen();  // TODO: eventually refactor into UI manager
            var levelLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            while (!levelLoad.isDone) yield return null;
        }
        if (spawnLoc >= 0) PartyController.Instance.MoveParty(levelManager.GetSpawnPoints(spawnLoc), false);
        var spawnPoints = PartyController.Instance.GetPartyLoc();

        foreach (var point in spawnPoints) Debug.Log("Spawn point: " + point.ToString());

        var activateScenes = levelManager.sceneTriggers.Where(trigger => spawnPoints.Any(spawnPoint => trigger.GetComponent<Collider>().bounds.Contains(spawnPoint))).ToList();
        var activateSceneNames = activateScenes.Select(trigger => trigger.GetComponent<SceneTrigger>().sceneName).ToList();
        foreach (var name in activateSceneNames) Debug.Log("activate scene: " + name);
        if (activateSceneNames.Any(activateScene => !SceneObjectManagers.ContainsKey(activateScene))) SetLoadingScreen();  // Will need to load new scenes before starting


        List<AsyncOperation> asyncOps = new List<AsyncOperation>();
        ActivateLevelScenes(activateSceneNames).ForEach(op => asyncOps.Add(op));
        while (asyncOps.Any(op => !op.isDone)) yield return null;

        PartyController.Instance.ActivateParty();
        Time.timeScale = 1;  // TODO: find better way of handling stopping/starting time than relying on menus (currently getting stuck because the UI stopped for menu then never started)
        yield return new WaitForSeconds(0.5f);
        Debug.Log("wait over");
        UnsetLoadingScreen();
        UIController.Instance.ActivateDefaultScreen();
    }

    private void SetLoadingScreen()
    {
        UIController.Instance.DeactivateAllMenus();
        UIController.Instance.AllowMenus(false);
        loadingScreen.ActivateMenu();
    }

    private void UnsetLoadingScreen()
    {
        loadingScreen.DeactivateMenu();
        UIController.Instance.AllowMenus(true);
    }

    public IEnumerator DeactivateLevel(LevelManager level)
    {
        //PartyController.Instance.DeactivateParty();
        foreach (string sceneName in level.levelScenes) { StartCoroutine(DeactivateSubscene(sceneName)); }
        SceneManager.UnloadSceneAsync(level.LevelName); ;
        yield return null;
    }

    public IEnumerator ActivateSubscene(string sceneName)
    {
        //ActiveLevelScenes.Add(sceneName);
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
        SceneObjectManagers[sceneManager.sceneName] = sceneManager;
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

    public Dictionary<string, SceneObjectManager> GetCurrentSceneManagers()
    {
        //return ActiveLevelScenes.ToDictionary(scene => scene, scene => SceneObjectManagers[scene]);
        return SceneObjectManagers;
    }

    public void LoadFromData(GameSaveData saveData)  //TODO: maybe goes better in SaveSystem
    {
        Debug.Log("Load from Data " + saveData.levelName);
        SceneData = saveData.SceneData;
        PartyController.Instance.InstantiateFromData(saveData.partyData);
        Debug.Log("Party Instantiated");
        PersistentDataManager.ApplySaveData(saveData.dialogData);
        StartCoroutine(ActivateLevel(saveData.levelName));
    }
}
