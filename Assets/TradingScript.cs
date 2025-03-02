using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;
using System.ComponentModel;
using System.Linq;


public class TradingScript : MonoBehaviour
{
    public GameObject tradeMenu;

    public EntityInventory playerInventory; // TODO: to be handled by player handler
    private EntityInventory merchantInventory;

    // TODO: merchant trading menu to activate if container has merchant flag - only allow "buying" goods - will also need to add "price" to game object data

    public Controller controller;
    public GameObject ui;

    private void Awake()
    {
        tradeMenu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (tradeMenu.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            DeactivateInventory();
        }
    }

    public void DeactivateInventory()
    {
        tradeMenu.SetActive(false);
        ui.GetComponent<UIScript>().ActivateUI();
        Time.timeScale = 1; // Unpause
    }

    public void ActivateTrade(EntityInventory sellerInventory)
    {
        Time.timeScale = 0; // Pauses time
        merchantInventory = sellerInventory;
        tradeMenu.SetActive(true);
        ui.GetComponent<UIScript>().DeactivateUI();
        tradeMenu.GetComponent<InventoryMenu>().ActivateInventory(sellerInventory, false);
    }



    // TODO: save a lot of this for big menu refactor!!!
    // allow clicking on multiple items at once, gold for either player or merchant automatically fills to make up the difference, trade button greyed out if not enough gold (with pop up?)



    //public void ClearInventorySlots()
    //{
    //    tradeMenu.GetComponent<InventoryMenu>().ClearInventorySlots();
    //    containerMenu.GetComponent<InventoryMenu>().ClearInventorySlots();
    //}

    //public int AddNewItem(InventoryData itemData, int newStackSize)
    //{
    //    for (int i = 0; i < EntityInventory.invSize; i++)
    //    {
    //        if (playerInventory.GetInventory(i).Item2 == 0)
    //        {
    //            Debug.Log("empty stack " + i);
    //            int transferStack = Math.Min(itemData.maxStackSize, newStackSize);
    //            //ItemSlots[i].AddItem(itemData, transferStack, true);
    //            playerInventory.SetInventory(i, itemData, transferStack);
    //            newStackSize -= transferStack;
    //        }
    //        else if (playerInventory.GetInventory(i).Item1 == itemData)
    //        {
    //            Debug.Log("Same data in " + i + " for " + itemData.itemName);
    //            int currentStack = playerInventory.GetInventory(i).Item2;
    //            int freeSpace = itemData.maxStackSize - currentStack;
    //            int transferStack = Math.Min(freeSpace, newStackSize);
    //            //ItemSlots[i].AddItem(itemData, transferStack);
    //            playerInventory.SetInventory(i, itemData, currentStack + transferStack);
    //            newStackSize -= transferStack;
    //        }

    //        if (newStackSize <= 0) return 0;
    //    }
    //    //if (menuActivated != null) { ActivateInventory(currentContainer); } // Update menu if currently open
    //    return newStackSize;
    //}

    //public void CollectAll()
    //{
    //    if (currentInventory == null)
    //    {
    //        Debug.LogError("This should never be null in the collect all method");
    //    }
    //    var containerEntity = currentInventory;
    //    for (int i = 0; i < containerEntity.inventoryTypes.Length; i++)
    //    {
    //        var invItem = containerEntity.GetInventory(i);
    //        if (invItem.Item1 == null) continue;

    //        int leftover = AddNewItem(invItem.Item1, invItem.Item2);
    //        containerEntity.SetInventory(i, invItem.Item1, leftover);
    //    }

    //    ActivateInventory(currentInventory);
    //}

    //public void UpdateEntity()
    //{
    //    menuActivated.GetComponent<InventoryMenu>().UpdateEntity(currentInventory);
    //}

    //public void DeselectAllSlots()
    //{
    //    tradeMenu.GetComponent<InventoryMenu>().DeselectAllSlots();
    //    containerMenu.GetComponent<InventoryMenu>().DeselectAllSlots();
    //}
}
