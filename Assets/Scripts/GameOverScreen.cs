using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class GameOverScreen : MenuScreen // TODO: combine with other menus when refactor happens
{
    public static GameOverScreen Instance { get; private set; }
    //public GameObject TextMenu;
    public bool UIActive;

    public GameObject GameOverMenu;


    //private CharStats charStats;

    private void Awake()
    {
        Instance = this;
    }

    public override void ActivateMenu()
    {
        Debug.Log("Activate Game Over");
        UIActive = true;
        GameOverMenu.SetActive(true);
    }

    public override void DeactivateMenu()
    {
        GameOverMenu.SetActive(false);
        UIActive = false;
    }

    public override bool IsActive()
    {
        return UIActive;
    }

    public override bool overlay => false;

    public void MainMenu()
    {
        DeactivateMenu();
        SceneLoader.Instance.NewGame();
    }
}
