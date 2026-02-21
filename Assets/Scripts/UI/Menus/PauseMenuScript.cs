using System;
using System.Data;
using UnityEngine;

public class PauseMenuScript : MenuScreen
{
    public GameObject menu;
    public GameObject ui;

    public static MenuScreen Instance;

    private bool menuOpen;

    public void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        //if (ui.GetComponent<UIScript>().UIActive && !menuOpen && Input.GetKeyDown(KeyCode.Escape))
        //{
        //    ActivateMenu();
        //}
        //else if (menuOpen && Input.GetKeyDown(KeyCode.Escape))
        //{
        //    DeactivateMenu();
        //}
    }

    public override void DeactivateMenu()
    {
        menu.SetActive(false);
        menuOpen = false;
    }

    public override void ActivateMenu()
    {
        menu.SetActive(true);
        menuOpen = true;
    }

    public override bool IsActive()
    {
        return menuOpen;
    }

    public void ResumeGame()
    {
        UIController.Instance.CloseOverlays();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneLoader.Instance.ToMainMenu();
    }

    public void Settings()
    {
        UIController.Instance.ActivateSettings();
    }

    public void Save()
    {
        SaveSystem.SaveGame(PartyController.Instance, SceneLoader.Instance);
    }

    public void Load() // TODO: Implement full Save Menu/system
    {
        SceneLoader.Instance.LoadFromData(SaveSystem.LoadGame());
    }
}
