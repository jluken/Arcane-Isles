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


public class PlayerInventoryMenu : InventoryMenu
{
    public GameObject inventoryMenu;

    public GameObject player;  // TODO: find all instances of player and find instance of player (separate from companions)
    private EntityInventory playerInventory;
    private CharStats playerStats;

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
    public EquipmentSlot[] EquipSlots;

    private Dictionary<InventoryData.ItemType, int> equipTypes;

    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public Sprite emptySprite;

    private int selectedSlotId;

    private void Awake()
    {
        playerInventory = player.GetComponent<EntityInventory>();
        playerStats = player.GetComponent<CharStats>();
        Debug.Log("player inv awake: " + playerInventory);
        for (int i = 0; i < PlayerInventorySlots.Length; i++)
        {
            PlayerInventorySlots[i].slotID = i;
            PlayerInventorySlots[i].slotGroup = "inventory";
            PlayerInventorySlots[i].parentMenu = this;
        }
        for (int i = 0; i < EquipSlots.Length; i++)
        {
            EquipSlots[i].slotID = i;
            EquipSlots[i].slotGroup = "equipment";
            EquipSlots[i].parentMenu = this;
        }

        equipTypes = new Dictionary<InventoryData.ItemType, int> { 
            { InventoryData.ItemType.weapon, 0 },
            { InventoryData.ItemType.armor, 1 },
            { InventoryData.ItemType.headwear, 2 },
            { InventoryData.ItemType.boots, 3 }
        }; // TODO: check if these are right; maybe make dynamic
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
        Time.timeScale = 1; // Unpause
        UpdateEntity();
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
        EquipSlots.ToList().ForEach(slot => {
            var equipSlot = playerInventory.GetEquipment(slot.slotID);
            if (equipSlot != null) { slot.AddItem(equipSlot, 1 , true); }
        });
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

    public override bool IsActive()
    {
        return inventoryMenu.activeSelf;
    }

    public override bool overlay => true;

    public override void ClearInventorySlots() {
        DeselectAllSlots();
        PlayerInventorySlots.ToList().ForEach(slot => slot.ClearItem(true));
        EquipSlots.ToList().ForEach(slot => slot.ClearItem(true));
    }

    public override void ActivateItem(InventoryData itemData, string slotGroup, int slotId)
    {
        if (itemData.itemType == InventoryData.ItemType.consumable)
        {
            foreach (var consumeData in itemData.consumeStats)
            {
                if (consumeData.consumeStat == CharStats.StatVal.health) playerStats.updateHealth(consumeData.value);
                else if (consumeData.consumeStat == CharStats.StatVal.magick) playerStats.updateMagick(consumeData.value);
                else
                {
                    playerStats.addModifier(consumeData.consumeStat, consumeData.value, consumeData.duration);
                }
            }
            PlayerInventorySlots[slotId].RemoveItem(1, true);
        }
        else if (equipTypes.ContainsKey(itemData.itemType))
        {
            var equipSlot = equipTypes[itemData.itemType];
            if (slotGroup == "inventory")
            {
                var oldEquip = playerInventory.GetEquipment(equipSlot);
                playerInventory.SetEquipment(equipSlot, itemData);
                playerInventory.SetInventory(slotId, oldEquip);
            }
            else
            {
                var leftover = playerInventory.AddNewItem(playerInventory.GetEquipment(equipSlot));
                if (leftover == 0) playerInventory.SetEquipment(equipSlot, null);
            }
            ActivateMenu(); // Reactivate menu after resetting through entity data
        }
    }

    //private ItemSlot GetItemSlot(int slotId)
    //{
    //    ItemSlot slot = Array.Find(PlayerInventorySlots, s => s.slotID == slotId);
    //    var matching = PlayerInventorySlots.Where(s => s.slotID == slotId);
    //    if (matching.Any()) return matching.First();
    //    matching = EquipSlots.Where(s => s.slotID == slotId);
    //    if (matching.Any()) return matching.First();
    //    throw new Exception("No slot found matching id " + slotId.ToString());
    //}

    //public InventoryData RemoveItem(int slotID, int amount = 1, bool destroyDrag = false)
    //{
    //    playerInventory.SetInventory(slotID, null, 0);
    //    DeselectAllSlots();
    //    return ItemSlots[slotID].RemoveItem(amount, destroyDrag);
    //}

    public void UpdateEntity()
    {
        Debug.Log("Update player entity");
        PlayerInventorySlots.ToList().ForEach(slot => playerInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
        EquipSlots.ToList().ForEach(slot => playerInventory.SetEquipment(slot.slotID, slot.itemData));

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
        EquipSlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
        
        selectedSlotId = -1;
        itemDescriptionImage.sprite = emptySprite;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
    }
}
