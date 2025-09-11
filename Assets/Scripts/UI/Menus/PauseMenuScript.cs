using System;
using System.Data;
using UnityEngine;

public class PauseMenuScript : MenuScreen
{
    public GameObject menu;
    public GameObject ui;

    private bool menuOpen;

    void Start()
    {
        //DeactivateMenu();
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

    public override bool overlay => true;

    public void QuitGame()
    {
        Application.Quit();
    }
}
