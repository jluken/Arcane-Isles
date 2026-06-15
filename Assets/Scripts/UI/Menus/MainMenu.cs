using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MenuScreen
{
    public static MainMenu Instance;

    public GameObject mainMenu;

    public Button loadButton;
    public Button continueButton;

    private bool active;

    public void Awake()
    {
        Instance = this;
    }

    public void Settings()
    {
        UIController.Instance.ActivateSettings();
    }

    public void Load()
    {
        UIController.Instance.ActivateLoadMenu();
    }

    public void Continue()
    {
        var saveNames = SaveSystem.GetSaves();
        var saveVals = new List<Dictionary<string, string>>();
        foreach (string name in saveNames)
        {
            var saveVal = new Dictionary<string, string>();
            saveVal.Add("name", name);
            saveVal.Add("time", SaveSystem.LoadGame(name).saveTime);
            saveVals.Add(saveVal);
        }
        saveVals = saveVals.OrderByDescending(x => DateTime.Parse(x["time"])).ToList();
        StartCoroutine(SceneLoader.Instance.LoadFromData(SaveSystem.LoadGame(saveVals[0]["name"])));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public override void ActivateMenu()
    {
        mainMenu.SetActive(true);
        active = true;

        var saveNames = SaveSystem.GetSaves();
        if (saveNames == null || saveNames.Count == 0)
        {
            loadButton.interactable = false;
            continueButton.interactable = false;
        }
        else
        {
            loadButton.interactable = true;
            continueButton.interactable = true;
        }
    }

    public override void DeactivateMenu()
    {
        mainMenu.SetActive(false);
        active = false;
    }

    public override bool IsActive()
    {
        return active;
    }
}
