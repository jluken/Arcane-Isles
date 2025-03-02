using System;
using System.Data;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject menu;
    public GameObject ui;

    private bool menuOpen;

    void Start()
    {
        DeactivateMenu();
    }

    void Update()
    {
        if (ui.GetComponent<UIScript>().UIActive && !menuOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ActivateMenu();
        }
        else if (menuOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            DeactivateMenu();
        }
    }

    public void DeactivateMenu()
    {
        menu.SetActive(false);
        menuOpen = false;
    }

    public void ActivateMenu()
    {
        menu.SetActive(true);
        menuOpen = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
