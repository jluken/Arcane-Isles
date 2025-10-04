using NUnit.Framework;
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


    public List<string> ActiveLevelScenes { get; private set; }
    private Dictionary<string, SceneSaveData> SceneData;
    public Dictionary<string, SceneObjectManager> SceneObjectManagers;
    private bool partyAndUILoaded;

    private void Awake()
    {
        Instance = this;
        ActiveLevelScenes = new List<string>();
        SceneData = new Dictionary<string, SceneSaveData>();
        partyAndUILoaded = false;
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
        StartCoroutine(ActivateLevel(levelName));
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
        if (activateSceneNames.Any(activateScene => !ActiveLevelScenes.Contains(activateScene))) SetLoadingScreen();  // Will need to load new scenes before starting


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
        ActiveLevelScenes.Add(sceneName);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return null;
    }

    public IEnumerator DeactivateSubscene(string sceneName)
    {
        if (ActiveLevelScenes.Contains(sceneName)) {
            var manager = SceneObjectManagers[sceneName];
            SceneData[sceneName] = new SceneSaveData(manager.npcs, manager.containers, manager.GroundObjects);
            SceneData[sceneName].loaded = false;
            ActiveLevelScenes.Remove(sceneName);
            SceneManager.UnloadSceneAsync(sceneName);
        }
        yield return null;

    }

    private List<AsyncOperation> ActivateLevelScenes(List<string> levelScenes)
    {
        List<AsyncOperation> asyncOps = new List<AsyncOperation>();
        foreach (string sceneName in levelScenes)
        {
            if (!ActiveLevelScenes.Contains(sceneName))
            {
                asyncOps.Add(SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive));
                ActiveLevelScenes.Add(sceneName);
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
        foreach (var activeScene in ActiveLevelScenes)
        {
            var manager = SceneObjectManagers[activeScene];
            totalSceneData[activeScene] = new SceneSaveData(manager.npcs, manager.containers, manager.GroundObjects);
            totalSceneData[activeScene].loaded = true;
        }
        return totalSceneData;
    }

    public void LoadFromData(GameSaveData saveData)
    {
        Debug.Log("Load from Data " + saveData.levelName);
        SceneData = saveData.SceneData;
        PartyController.Instance.InstantiateFromData(saveData.partyData);
        Debug.Log("Party Instantiated");
        StartCoroutine(ActivateLevel(saveData.levelName));
    }
}
