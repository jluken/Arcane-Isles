using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;
using System.ComponentModel;
using System.Linq;


public class TradingMenu : InventoryMenu
{
    public GameObject inventoryMenu;
    EntityInventory currentInventory;

    public GameObject player;
    private EntityInventory playerInventory;
    private CharStats playerStats;

    // TODO: merchant trading menu to activate if container has merchant flag - only allow "buying" goods - will also need to add "price" to game object data

    //private SelectionController selectionController;
    //public GameObject ui;

    //private void Awake()
    //{
    //    playerInventory = player.GetComponent<EntityInventory>();
    //    playerStats = player.GetComponent<CharStats>();
    //    inventoryMenu.SetActive(false);
    //    containerMenu.SetActive(false);
    //}

    public ItemSlot[] PlayerInventorySlots;
    public ItemSlot[] NPCInventorySlots;

    private void Awake()
    {
        playerInventory = player.GetComponent<EntityInventory>();
        Debug.Log("player inv awake: " + playerInventory);
        for (int i = 0; i < PlayerInventorySlots.Length; i++)
        {
            PlayerInventorySlots[i].slotID = i;
            PlayerInventorySlots[i].parentMenu = this;
        }
        for (int i = 0; i < NPCInventorySlots.Length; i++)
        {
            NPCInventorySlots[i].slotID = i;
            NPCInventorySlots[i].parentMenu = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //selectionController = SelectionController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //if (menuActivated != inventoryMenu && Input.GetKeyDown(KeyCode.I))
        //{
        //    DeactivateInventory();
        //    ActivateInventory();
        //}
        //else if (menuActivated == inventoryMenu && Input.GetKeyDown(KeyCode.I))
        //{
        //    DeactivateInventory();
        //}
        //else if (menuActivated != null && Input.GetKeyDown(KeyCode.Escape))
        //{
        //    DeactivateInventory();
        //}
    }

    public override void DeactivateMenu()
    {
        SelectionController.Instance.NewSelection();
        inventoryMenu.SetActive(false);
        currentInventory = null;
        UpdateEntity();
        Time.timeScale = 1; // Unpause
    }

    public void SetInventory(EntityInventory inventory = null)
    {
        currentInventory = inventory;
    }

    public override void ActivateMenu()
    {
        inventoryMenu.SetActive(true);
        Time.timeScale = 0;

        ClearInventorySlots();

        PlayerInventorySlots.ToList().ForEach(slot => {
            var playerSlot = playerInventory.GetInventory(slot.slotID);
            if (playerSlot.Item1 != null) { slot.AddItem(playerSlot.Item1, playerSlot.Item2, true); }
        });
        NPCInventorySlots.ToList().ForEach(slot => {
            var npcSlot = currentInventory.GetInventory(slot.slotID);
            if (npcSlot.Item1 != null) { slot.AddItem(npcSlot.Item1, npcSlot.Item2, true); }
        });
    }



    public override bool IsActive()
    {
        return inventoryMenu.activeSelf;
    }

    public override bool overlay => true;

    public override void ClearInventorySlots()
    {
        DeselectAllSlots();
        PlayerInventorySlots.ToList().ForEach(slot => slot.ClearItem(true));
        NPCInventorySlots.ToList().ForEach(slot => slot.ClearItem(true));
    }

    //public InventoryData RemoveItem(int slotID, int amount = 1, bool destroyDrag = false)
    //{
    //    playerInventory.SetInventory(slotID, null, 0);
    //    DeselectAllSlots();
    //    return ItemSlots[slotID].RemoveItem(amount, destroyDrag);
    //}

    public void UpdateEntity()
    {
        PlayerInventorySlots.ToList().ForEach(slot => playerInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
        if (currentInventory != null)
            NPCInventorySlots.ToList().ForEach(slot => currentInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));

        //currentMenu.PlayerSlots.ToList().ForEach(slot => playerInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
        //currentMenu.ContainerSlots.ToList().ForEach(slot => currentContainer.GetComponent<EntityInventory>().SetInventory(slot.slotID, slot.itemData, slot.currentStack));

        //for (int i = 0; i < currentPlayerSlots.Length; i++)
        //{
        //    //Debug.Log("item slot data " + i + ": " + ItemSlots[i].itemData);
        //    playerInventory.SetInventory(i, currentPlayerSlots[i].itemData, currentPlayerSlots[i].currentStack);
        //}
        //if (menuActivated == inventoryMenu)
        //{
        //    // Take updates to the UI and alters the player inv to adjust
        //    for (int i = 0; i < ItemSlots.Length; i++)
        //    {
        //        //Debug.Log("item slot data " + i + ": " + ItemSlots[i].itemData);
        //        playerInventory.SetInventory(i, ItemSlots[i].itemData, ItemSlots[i].currentStack);
        //    }
        //}
        //else if (menuActivated == containerMenu)
        //{
        //    for (int i = 0; i < ItemSlots.Length; i++)
        //    {
        //        playerInventory.SetInventory(i, PlayerSlots[i].itemData, PlayerSlots[i].currentStack);
        //        currentContainer.GetComponent<EntityInventory>().SetInventory(i, ContainerSlots[i].itemData, ContainerSlots[i].currentStack);
        //    }
        //}
    }

    public override void DeselectAllSlots()
    {
        PlayerInventorySlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
        NPCInventorySlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
    }

    public override void ActivateItem(InventoryData itemData, string slotGroup, int slotID)
    {
        throw new NotImplementedException(); //TODO
    }

    public override void SelectItem(InventoryData itemData, string slotGroup, int slotID)
    {
        throw new NotImplementedException(); //TODO
    }
}
