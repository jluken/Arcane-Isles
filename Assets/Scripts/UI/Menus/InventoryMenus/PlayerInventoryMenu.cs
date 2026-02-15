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


    public GameObject playerInventorySlots;
    public GameObject slotPrefab;

    public List<ItemSlot> PlayerInventorySlots;
    public EquipmentSlot HeadSlot;
    public EquipmentSlot ArmorSlot;
    public EquipmentSlot WeaponSlot;
    public EquipmentSlot BootSlot;

    private Dictionary<ItemType, EquipmentSlot> equipMapping;

    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public Sprite emptySprite;

    private int selectedSlotId;

    private void Awake()
    {
        //playerInventory = player.GetComponent<EntityInventory>();
        //playerStats = player.GetComponent<CharStats>();
        //Debug.Log("player inv awake: " + playerInventory);
        //for (int i = 0; i < PlayerInventorySlots.Length; i++)
        //{
        //    PlayerInventorySlots[i].slotID = i;
        //    PlayerInventorySlots[i].slotGroup = "inventory";
        //    PlayerInventorySlots[i].parentMenu = this;
        //}
        //for (int i = 0; i < EquipSlots.Length; i++)
        //{
        //    EquipSlots[i].slotID = i;
        //    EquipSlots[i].slotGroup = "equipment";
        //    EquipSlots[i].parentMenu = this;
        //}
        PlayerInventorySlots = new List<ItemSlot>();
        equipMapping = new Dictionary<ItemType, EquipmentSlot>()
        {
            { ItemType.headwear, HeadSlot },
            { ItemType.armor, ArmorSlot },
            { ItemType.weapon, WeaponSlot },
            { ItemType.boots, BootSlot }
        };
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
        SelectionController.Instance.Deselect();
        inventoryMenu.SetActive(false);
        Time.timeScale = 1; // Unpause
        UpdateEntity();
    }

    public override void ActivateMenu()
    {
        inventoryMenu.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Pause Time");

        ClearInventorySlots();

        for(int i = 0; i < PartyController.Instance.leader.inventory.maxInv; i++)
        {
            var slotData = PartyController.Instance.leader.inventory.GetInventory(i);
            var slotItem = Instantiate(slotPrefab, playerInventorySlots.transform);
            PlayerInventorySlots.Add(slotItem.GetComponent<ItemSlot>());
            PlayerInventorySlots[i].slotID = i;
            PlayerInventorySlots[i].parentMenu = this;
            PlayerInventorySlots[i].slotGroup = PlayerInventorySlots;
            if (slotData.type != null) { PlayerInventorySlots[i].AddItem(slotData.type, slotData.count); }
        }

        foreach (KeyValuePair<ItemType, EquipmentSlot> kvp in equipMapping)
        {
            kvp.Value.parentMenu = this;
            var equipSlot = PartyController.Instance.leader.inventory.GetEquipment(kvp.Key);
            if (equipSlot != null) { kvp.Value.AddItem(equipSlot, 1); }
        }
    }

    public override void SelectItem(InventoryData itemData, List<ItemSlot> slotGroup, int slotID)
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
        PlayerInventorySlots.ToList().ForEach(slot => Destroy(slot.gameObject));
        PlayerInventorySlots = new List<ItemSlot>();
        foreach (KeyValuePair<ItemType, EquipmentSlot> kvp in equipMapping) kvp.Value.ClearItem(true);
    }

    public override void ActivateItem(InventoryData itemData, List<ItemSlot> slotGroup, int slotId)
    {
        if (itemData.itemType == ItemType.consumable)
        {
            if (!CombatManager.Instance.CheckActionPoints(itemData.APCost)) return;
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
            PlayerInventorySlots[slotId].RemoveItem(1, true);
        }
        else if (equipMapping.ContainsKey(itemData.itemType))
        {
            var playerInventory = PartyController.Instance.leader.inventory;
            var equipSlot = equipMapping[itemData.itemType];
            Debug.Log("Setting equipment");
            Debug.Log(itemData.itemType);
            Debug.Log(equipSlot);
            if (equipMapping.ContainsValue(equipSlot)) // Equip the item from the inventory
            {
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
            ActivateMenu(); // Reactivate menu after resetting through entity data
        }
    }

    public override void UpdateEntity()
    {
        Debug.Log("Update player entity");
        var playerInventory = PartyController.Instance.leader.inventory;
        PlayerInventorySlots.ToList().ForEach(slot => playerInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
        equipMapping.ToList().ForEach(slotKVP => playerInventory.SetEquipment(slotKVP.Key, slotKVP.Value.itemData));
    }

    public override void DeselectAllSlots()
    {
        Debug.Log("Deselect");
        PlayerInventorySlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
        equipMapping.Values.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
        
        selectedSlotId = -1;
        itemDescriptionImage.sprite = emptySprite;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
    }
}
