using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public TMP_Text itemDescriptionText;  // TODO: figure out all info to display here - attacks/damage/range/radius/AP, price, weight, flavor/description of effect, duration, DT  (maybe display as a sort of note pinned to page)
    public Sprite emptySprite;

    private int selectedSlotId;

    public void Awake()
    {
        Instance = this;
    }

    public override void DeactivateMenu()
    {
        //SelectionController.Instance.Deselect();
        inventoryMenu.SetActive(false);
    }

    public override void ActivateMenu()
    {
        inventoryMenu.SetActive(true);
        
        playerInventorySlots.PopulateInventory(PartyController.Instance.selectedPartyMember.inventory);
        equipmentSlots.PopulateInventory(PartyController.Instance.selectedPartyMember.inventory);
    }

    public override void SelectItem(InventoryData itemData, InventoryPanel slotGroup, int slotID)
    {
        DeselectAllSlots();
        if (itemData != null)
        {
            itemDescriptionImage.sprite = itemData.sprite;
            itemDescriptionNameText.text = itemData.itemName;
            itemDescriptionText.text = itemData.description;  // TODO: get "fullText" which might differ based on type (BG3: 
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
                if (consumeData.consumeStat == CharStats.StatVal.health) PartyController.Instance.selectedPartyMember.charStats.updateHealth(consumeData.value);
                else if (consumeData.consumeStat == CharStats.StatVal.magick) PartyController.Instance.selectedPartyMember.charStats.updateMagick(consumeData.value);
                else
                {
                    PartyController.Instance.selectedPartyMember.charStats.addModifier(consumeData.consumeStat, consumeData.value, consumeData.duration);
                }
            }
            CombatManager.Instance.SpendActionPoints(itemData.APCost);
            Debug.Log("prev count " + PartyController.Instance.selectedPartyMember.inventory.inventory[slotId].count);
            Debug.Log("Remove 1 from slot " + slotId);
            PartyController.Instance.selectedPartyMember.inventory.UpdateInvStack(slotId, -1);
            Debug.Log("post count " + PartyController.Instance.selectedPartyMember.inventory.inventory[slotId].count);
        }
        else if (EntityInventory.defaultEquipSlot.ContainsKey(itemData.itemType))
        {
            var playerInventory = PartyController.Instance.selectedPartyMember.inventory;
            Debug.Log("Equipment type");
            if (slotGroup == playerInventorySlots) // Equip the item from the inventory
            {
                Debug.Log("Equip from inventory");
                if (!CombatManager.Instance.CheckActionPoints(itemData.APCost)) return;
                var oldEquip = playerInventory.SwapOutEquipment(itemData);
                playerInventory.SetInventory(slotId, oldEquip);
                CombatManager.Instance.SpendActionPoints(itemData.APCost);
            }
            else
            {
                Debug.Log("De-equip");
                var clickedEquipSlot = (EntityInventory.EquipmentInvType)slotId;
                var leftover = playerInventory.AddNewItem(playerInventory.GetEquipment(clickedEquipSlot));
                if (leftover == 0) playerInventory.SetEquipment(clickedEquipSlot, null);
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
