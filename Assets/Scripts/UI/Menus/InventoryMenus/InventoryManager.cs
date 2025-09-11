//using NUnit.Framework.Interfaces;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System;
//using UnityEditor;
//using System.ComponentModel;
//using System.Linq;


//public class InventoryManager : MenuScreen
//{
//    public GameObject inventoryMenu;
//    private GameObject menuActivated;

//    public GameObject player;
//    private EntityInventory playerInventory;
//    private CharStats playerStats;

//    public GameObject containerMenu;
//    Container currentContainer;
//    EntityInventory currentInventory;

//    // TODO: merchant trading menu to activate if container has merchant flag - only allow "buying" goods - will also need to add "price" to game object data

//    public Controller controller;
//    //public GameObject ui;

//    private void Awake()
//    {
//        playerInventory = player.GetComponent<EntityInventory>();
//        playerStats = player.GetComponent<CharStats>();
//        inventoryMenu.SetActive(false);
//        containerMenu.SetActive(false);
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //if (menuActivated != inventoryMenu && Input.GetKeyDown(KeyCode.I))
//        //{
//        //    DeactivateInventory();
//        //    ActivateInventory();
//        //}
//        //else if (menuActivated == inventoryMenu && Input.GetKeyDown(KeyCode.I))
//        //{
//        //    DeactivateInventory();
//        //}
//        //else if (menuActivated != null && Input.GetKeyDown(KeyCode.Escape))
//        //{
//        //    DeactivateInventory();
//        //}
//    }

//    public override void DeactivateMenu()
//    {
//        controller.NewSelection();
//        currentInventory = null;
//        menuActivated?.SetActive(false);
//        menuActivated = null;
//        Time.timeScale = 1; // Unpause
//    }

//    public override void ActivateMenu()
//    {
//        menuActivated.SetActive(true);
//        menuActivated.GetComponent<InventoryMenu>().ActivateInventory(currentInventory);
//    }

//    public void SetInventory(EntityInventory inventory = null)
//    {
//        if (inventory == null)
//        {
//            menuActivated = inventoryMenu;
//            Time.timeScale = 0; // Pauses time
//        }
//        else
//        {
//            menuActivated = containerMenu;
//            currentInventory = inventory;
//        }
//    }

//    public override bool IsActive()
//    {
//        return menuActivated != null;  //TODO: possibly revisit how this is handling both inventory screens (possibly separate)
//    }

//    public override bool closeable => true;

//    public void ClearInventorySlots() {
//        inventoryMenu.GetComponent<InventoryMenu>().ClearInventorySlots();
//        containerMenu.GetComponent<InventoryMenu>().ClearInventorySlots();
//    }

//    public void UseItem(InventoryData itemData)
//    {
//        // Currently only triggering for consumables, and only thing consumables do is change stats
//        if (itemData.itemType == InventoryData.ItemType.consumable)
//        {
            
//            for (int i = 0; i < itemData.statsToChange.Length; i++)
//            {
//                // TODO: this logic should happen in the item itself, not the inventory
//                if (itemData.statsToChange[i] == InventoryData.StatToChange.health) playerStats.updateHealth((int)itemData.statChanges[i]);
//                else if (itemData.statsToChange[i] == InventoryData.StatToChange.stamina) playerStats.updateStamina((int)itemData.statChanges[i]);
//                else
//                {
//                    playerStats.addModifier(itemData.statsToChange[i], (int)itemData.statChanges[i]); // TODO: what to do with returned id
//                }

//            }
//        }
//    }

//    public int AddNewItem(InventoryData itemData, int newStackSize)
//    {
//        for (int i = 0; i < EntityInventory.invSize; i++)
//        {
//            if (playerInventory.GetInventory(i).Item2 == 0)
//            {
//                Debug.Log("empty stack " + i);
//                int transferStack = Math.Min(itemData.maxStackSize, newStackSize);
//                //ItemSlots[i].AddItem(itemData, transferStack, true);
//                playerInventory.SetInventory(i, itemData, transferStack);
//                newStackSize -= transferStack;
//            }
//            else if (playerInventory.GetInventory(i).Item1 == itemData)
//            {
//                Debug.Log("Same data in " + i + " for " + itemData.itemName);
//                int currentStack = playerInventory.GetInventory(i).Item2;
//                int freeSpace = itemData.maxStackSize - currentStack;
//                int transferStack = Math.Min(freeSpace, newStackSize);
//                //ItemSlots[i].AddItem(itemData, transferStack);
//                playerInventory.SetInventory(i, itemData, currentStack + transferStack);
//                newStackSize -= transferStack;
//            }
            
//            if (newStackSize <= 0) return 0;
//        }
//        //if (menuActivated != null) { ActivateInventory(currentContainer); } // Update menu if currently open
//        return newStackSize;
//    }

//    public void CollectAll()
//    {
//        if (currentInventory == null)
//        {
//            Debug.LogError("This should never be null in the collect all method");
//        }
//        var containerEntity = currentInventory;
//        for (int i = 0; i < containerEntity.inventoryTypes.Length; i++)
//        {
//            var invItem = containerEntity.GetInventory(i);
//            if (invItem.Item1 == null) continue;

//            int leftover = AddNewItem(invItem.Item1, invItem.Item2);
//            containerEntity.SetInventory(i, invItem.Item1, leftover);
//        }

//        menuActivated.GetComponent<InventoryMenu>().ActivateInventory(currentInventory);
//    }

//    //public InventoryData RemoveItem(int slotID, int amount = 1, bool destroyDrag = false)
//    //{
//    //    playerInventory.SetInventory(slotID, null, 0);
//    //    DeselectAllSlots();
//    //    return ItemSlots[slotID].RemoveItem(amount, destroyDrag);
//    //}

//    public void UpdateEntity()
//    {
//        menuActivated.GetComponent<InventoryMenu>().UpdateEntityInventories(currentInventory);

//        //currentMenu.PlayerSlots.ToList().ForEach(slot => playerInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
//        //currentMenu.ContainerSlots.ToList().ForEach(slot => currentContainer.GetComponent<EntityInventory>().SetInventory(slot.slotID, slot.itemData, slot.currentStack));

//        //for (int i = 0; i < currentPlayerSlots.Length; i++)
//        //{
//        //    //Debug.Log("item slot data " + i + ": " + ItemSlots[i].itemData);
//        //    playerInventory.SetInventory(i, currentPlayerSlots[i].itemData, currentPlayerSlots[i].currentStack);
//        //}
//        //if (menuActivated == inventoryMenu)
//        //{
//        //    // Take updates to the UI and alters the player inv to adjust
//        //    for (int i = 0; i < ItemSlots.Length; i++)
//        //    {
//        //        //Debug.Log("item slot data " + i + ": " + ItemSlots[i].itemData);
//        //        playerInventory.SetInventory(i, ItemSlots[i].itemData, ItemSlots[i].currentStack);
//        //    }
//        //}
//        //else if (menuActivated == containerMenu)
//        //{
//        //    for (int i = 0; i < ItemSlots.Length; i++)
//        //    {
//        //        playerInventory.SetInventory(i, PlayerSlots[i].itemData, PlayerSlots[i].currentStack);
//        //        currentContainer.GetComponent<EntityInventory>().SetInventory(i, ContainerSlots[i].itemData, ContainerSlots[i].currentStack);
//        //    }
//        //}
//    }

//    public void DeselectAllSlots()
//    {
//        inventoryMenu.GetComponent<InventoryMenu>().DeselectAllSlots();
//        containerMenu.GetComponent<InventoryMenu>().DeselectAllSlots();
//    }
//}
