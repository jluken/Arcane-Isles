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
using static InventoryData;


public class PlayerInventoryMenu : InventoryMenu
{
    public GameObject inventoryMenu;
    public static PlayerInventoryMenu Instance;


    public InventoryPanel playerInventorySlots;
    public EquipmentPanel equipmentSlots;

    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public Sprite emptySprite;

    private int selectedSlotId;

    public void Awake()
    {
        Instance = this;
    }

    public override void DeactivateMenu()
    {
        SelectionController.Instance.Deselect();
        inventoryMenu.SetActive(false);
    }

    public override void ActivateMenu()
    {
        inventoryMenu.SetActive(true);
        
        playerInventorySlots.PopulateInventory(PartyController.Instance.leader.inventory);
        equipmentSlots.PopulateInventory(PartyController.Instance.leader.inventory);
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

    public override bool IsActive()
    {
        return inventoryMenu.activeSelf;
    }

    public override void ActivateItem(InventoryData itemData, InventoryPanel slotGroup, int slotId)
    {
        Debug.Log("Activate Item " + itemData.itemType);
        if (itemData.itemType == ItemType.consumable)
        {
            if (!CombatManager.Instance.CheckActionPoints(itemData.APCost)) return;
            Debug.Log("Pass Check");
            foreach (var consumeData in itemData.consumeStats)
            {
                if (consumeData.consumeStat == CharStats.StatVal.health) PartyController.Instance.leader.charStats.updateHealth(consumeData.value);
                else if (consumeData.consumeStat == CharStats.StatVal.magick) PartyController.Instance.leader.charStats.updateMagick(consumeData.value);
                else
                {
                    PartyController.Instance.leader.charStats.addModifier(consumeData.consumeStat, consumeData.value, consumeData.duration);
                }
            }
            CombatManager.Instance.SpendActionPoints(itemData.APCost);
            Debug.Log("prev count " + PartyController.Instance.leader.inventory.inventory[slotId].count);
            Debug.Log("Remove 1 from slot " + slotId);
            PartyController.Instance.leader.inventory.UpdateInvStack(slotId, -1);
            Debug.Log("post count " + PartyController.Instance.leader.inventory.inventory[slotId].count);
        }
        else if (equipmentTypes.Contains(itemData.itemType))
        {
            var playerInventory = PartyController.Instance.leader.inventory;
            Debug.Log("Equipment type");
            if (slotGroup == playerInventorySlots) // Equip the item from the inventory
            {
                Debug.Log("Equip from inventory");
                if (!CombatManager.Instance.CheckActionPoints(itemData.APCost)) return;
                var oldEquip = playerInventory.GetEquipment(itemData.itemType);
                playerInventory.SetEquipment(itemData.itemType, itemData);
                playerInventory.SetInventory(slotId, oldEquip);
                CombatManager.Instance.SpendActionPoints(itemData.APCost);
            }
            else
            {
                Debug.Log("De-equip");
                var leftover = playerInventory.AddNewItem(playerInventory.GetEquipment(itemData.itemType));
                if (leftover == 0) playerInventory.SetEquipment(itemData.itemType, null);
            }
        }
        ActivateMenu(); // Reactivate menu after resetting through entity data
    }

    //public override void UpdateEntity()
    //{
    //    Debug.Log("Update player entity");
    //    var playerInventory = PartyController.Instance.leader.inventory;
    //    PlayerInventorySlots.ToList().ForEach(slot => playerInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
    //    equipMapping.ToList().ForEach(slotKVP => playerInventory.SetEquipment(slotKVP.Key, slotKVP.Value.itemData));
    //}

    public override void DeselectAllSlots()
    {
        playerInventorySlots.DeselectPanelSlots();
        equipmentSlots.DeselectPanelSlots();
        
        selectedSlotId = -1;
        itemDescriptionImage.sprite = emptySprite;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
    }
}
