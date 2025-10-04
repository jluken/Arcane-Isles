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

    public ItemSlot[] PlayerInventorySlots;
    public ItemSlot[] ContainerInventorySlots;

    private void Awake()
    {
        Instance = this;


        for (int i = 0; i < PlayerInventorySlots.Length; i++)
        {
            PlayerInventorySlots[i].slotID = i; 
            PlayerInventorySlots[i].slotGroup = "player";
            PlayerInventorySlots[i].parentMenu = this;
        }
        for (int i = 0; i < ContainerInventorySlots.Length; i++)
        {
            ContainerInventorySlots[i].slotID = i;
            ContainerInventorySlots[i].slotGroup = "container";
            ContainerInventorySlots[i].parentMenu = this;
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
        UpdateEntity();
        inventoryMenu.SetActive(false);
        currentInventory = null;
    }

    public void SetInventory(EntityInventory inventory = null)
    {
        currentInventory = inventory;
    }

    public override void ActivateMenu()
    {
        inventoryMenu.SetActive(true);

        ClearInventorySlots();

        PlayerInventorySlots.ToList().ForEach(slot => {
            var playerSlot = PartyController.Instance.leader.GetComponent<EntityInventory>().GetInventory(slot.slotID);
            if (playerSlot.Item1 != null) { slot.AddItem(playerSlot.Item1, playerSlot.Item2, true); }
        });
        ContainerInventorySlots.ToList().ForEach(slot => {
            var containerSlot = currentInventory.GetInventory(slot.slotID);
            if (containerSlot.Item1 != null) { slot.AddItem(containerSlot.Item1, containerSlot.Item2, true); }
        });
    }

    

    public override bool IsActive()
    {
        return inventoryMenu.activeSelf;
    }

    public override bool overlay => true;

    public override void ClearInventorySlots() {
        DeselectAllSlots();
        PlayerInventorySlots.ToList().ForEach(slot => slot.ClearItem(true));
        ContainerInventorySlots.ToList().ForEach(slot => slot.ClearItem(true));
    }

    public override void ActivateItem(InventoryData itemData, string slotGroup, int slotId)
    {
        var playerInventory = PartyController.Instance.leader.GetComponent<EntityInventory>();
        if (slotGroup == "player")
        {
            var grabbedInv = playerInventory.GetInventory(slotId);
            var leftover = currentInventory.AddNewItem(itemData, grabbedInv.Item2);
            playerInventory.SetInventory(slotId, itemData, leftover);
        }
        else
        {
            var grabbedInv = currentInventory.GetInventory(slotId);
            var leftover = playerInventory.AddNewItem(itemData, grabbedInv.Item2);
            currentInventory.SetInventory(slotId, itemData, leftover);
        }
        ActivateMenu(); // Reactivate menu after resetting through entity data
    }

    public override void SelectItem(InventoryData itemData, string slotGroup, int slotID)
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
        if (currentInventory == null)
        {
            Debug.LogError("This should never be null in the collect all method");
        }
        var containerEntity = currentInventory;
        for (int i = 0; i < containerEntity.inventoryTypes.Length; i++)
        {
            var invItem = containerEntity.GetInventory(i);
            if (invItem.Item1 == null) continue;

            int leftover = playerInventory.AddNewItem(invItem.Item1, invItem.Item2);
            containerEntity.SetInventory(i, invItem.Item1, leftover);
        }

        //SetInventory(currentInventory);  
        ActivateMenu(); // Reactivate menu after collecting all through entity data
    }

    //public InventoryData RemoveItem(int slotID, int amount = 1, bool destroyDrag = false)
    //{
    //    playerInventory.SetInventory(slotID, null, 0);
    //    DeselectAllSlots();
    //    return ItemSlots[slotID].RemoveItem(amount, destroyDrag);
    //}

    public void UpdateEntity()
    {
        var playerInventory = PartyController.Instance.leader.GetComponent<EntityInventory>();
        PlayerInventorySlots.ToList().ForEach(slot => playerInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
        //Debug.Log("inv slots");
        //ContainerInventorySlots.ToList().ForEach(slot => Debug.Log(slot));
        if (currentInventory != null)
            ContainerInventorySlots.ToList().ForEach(slot => currentInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));

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
        ContainerInventorySlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });

        selectedSlotId = -1;
        itemDescriptionImage.sprite = emptySprite;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
    }
}
