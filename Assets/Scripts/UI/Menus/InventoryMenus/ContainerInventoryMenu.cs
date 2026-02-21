using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class ContainerInventoryMenu : InventoryMenu
{
    public static ContainerInventoryMenu Instance { get; private set; }
    public GameObject inventoryMenu;
    EntityInventory currentInventory;

    public InventoryPanel playerInventorySlots;
    public InventoryPanel containerInventorySlots;
    public GameObject slotPrefab;

    //private SelectionController selectionController;

    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public Sprite emptySprite;
    private int selectedSlotId;
    //public GameObject ui;

    //private void Awake()
    //{
    //    playerInventory = player.GetComponent<EntityInventory>();
    //    playerStats = player.GetComponent<CharStats>();
    //    inventoryMenu.SetActive(false);
    //    containerMenu.SetActive(false);
    //}

    //public List<ItemSlot> PlayerInventorySlots;
    //public List<ItemSlot> ContainerInventorySlots;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //selectionController = SelectionController.Instance;
    }

    public override void DeactivateMenu()
    {
        if (!IsActive()) return;
        SelectionController.Instance.Deselect();
        //UpdateEntity();
        inventoryMenu.SetActive(false);
        currentInventory = null;
    }

    public void SetInventory(EntityInventory inventory = null)
    {
        currentInventory = inventory;
        Debug.Log("current Inventory: " + currentInventory);
    }

    public override void ActivateMenu()
    {
        inventoryMenu.SetActive(true);

        //ClearInventorySlots();
        playerInventorySlots.PopulateInventory(PartyController.Instance.leader.inventory);
        Debug.Log("Populating container");
        containerInventorySlots.PopulateInventory(currentInventory);
    }

    

    public override bool IsActive()
    {
        return inventoryMenu.activeSelf;
    }

    public override void ActivateItem(InventoryData itemData, InventoryPanel slotGroup, int slotId)
    {
        var playerInventory = PartyController.Instance.leader.inventory;
        if (slotGroup == playerInventorySlots)
        {
            var grabbedInv = playerInventory.GetInventory(slotId);
            var leftover = currentInventory.AddNewItem(itemData, grabbedInv.count);
            playerInventory.SetInventory(slotId, itemData, leftover);
        }
        else
        {
            var grabbedInv = currentInventory.GetInventory(slotId);
            var leftover = playerInventory.AddNewItem(itemData, grabbedInv.count);
            currentInventory.SetInventory(slotId, itemData, leftover);
        }
        ActivateMenu(); // Reactivate menu after resetting through entity data
    }

    public override void SelectItem(InventoryData itemData, InventoryPanel slotGroup, int slotID)
    {
        DeselectAllSlots();
        if (itemData != null)
        {
            itemDescriptionImage.sprite = itemData.sprite;
            itemDescriptionNameText.text = itemData.itemName;
            itemDescriptionText.text = itemData.description;
        }
        selectedSlotId = slotID;
    }

    public void CollectAll()
    {
        var playerInventory = PartyController.Instance.leader.GetComponent<EntityInventory>();
        playerInventory.ConsumeInventory(currentInventory);
 
        ActivateMenu(); // Reactivate menu after collecting all through entity data
    }

    public override void DeselectAllSlots()
    {
        playerInventorySlots.DeselectPanelSlots();
        containerInventorySlots.DeselectPanelSlots();

        selectedSlotId = -1;
        itemDescriptionImage.sprite = emptySprite;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
    }
}
