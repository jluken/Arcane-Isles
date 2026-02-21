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

    public void ActivateSceneClick(string levelName)
    {
        SceneLoader.Instance.SetToLevelSpawn(levelName, spawnLoc:0);
    }
}
