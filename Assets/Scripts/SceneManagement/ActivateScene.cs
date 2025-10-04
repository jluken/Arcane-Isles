using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public Button collectButton;

    //public ContainerInventoryMenu inventoryMenu;

    void Start()
    {
        //inventoryMenu = ContainerInventoryMenu.Instance;
        //Button btn = collectButton.GetComponent<Button>();
        //btn.onClick.AddListener(CollectClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateSceneClick(string levelName)
    {
        SceneLoader.Instance.mainMenu.DeactivateMenu(); // TODO: move this somewhere else (UI update)
        SceneLoader.Instance.SetToLevelSpawn(levelName, spawnLoc:0);
    }
}
