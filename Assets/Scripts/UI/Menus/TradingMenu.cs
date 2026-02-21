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


public class TradingMenu : InventoryMenu
{
    public static TradingMenu Instance { get; private set; }
    public GameObject inventoryMenu;
    EntityInventory merchantInventory;

    public InventoryPanel playerInventorySlots;
    public TMP_Text playerName;
    public TMP_Text playerGold;

    public InventoryPanel merchantInventorySlots;
    public TMP_Text merchantName;
    public TMP_Text merchantGold;

    public static int barterSlotNum = 10;
    public EntityInventory playerBarterInv;
    public InventoryPanel playerBarterSlots;
    public EntityInventory merchantBarterInv;
    public InventoryPanel merchantBarterSlots;

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
    //public List<ItemSlot> MerchantInventorySlots;

    //public List<ItemSlot> PlayerBarterSlots;
    //public List<ItemSlot> MerchantBarterSlots;

    public TMP_Text merchantBarterValueText;
    public TMP_Text playerBarterValueText;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventHandler.Instance.inventoryUpdate += UpdateText;
    }

    public override void DeactivateMenu()
    {
        if (!IsActive()) return;
        SelectionController.Instance.Deselect();
        var playerInventory = PartyController.Instance.leader.GetComponent<EntityInventory>();
        for (int i = 0; i < playerBarterInv.maxInv; i++)
        {
            var slotData = playerBarterInv.GetInventory(i);
            if (slotData.type != null) { playerInventory.AddNewItem(slotData.type, slotData.count); }
            playerBarterInv.SetInventory(i, null, 0);
        }
        for (int i = 0; i < merchantBarterInv.maxInv; i++)
        {
            var slotData = merchantBarterInv.GetInventory(i);
            if (slotData.type != null) { merchantInventory.AddNewItem(slotData.type, slotData.count); }
            merchantBarterInv.SetInventory(i, null, 0);
        }
        //UpdateEntity();
        inventoryMenu.SetActive(false);
        merchantInventory = null;
    }

    public void SetInventory(EntityInventory inventory = null)
    {
        merchantInventory = inventory;
    }

    public override void ActivateMenu()
    {
        inventoryMenu.SetActive(true);

        //ClearInventorySlots();

        var activePlayer = PartyController.Instance.leader;
        playerInventorySlots.PopulateInventory(activePlayer.inventory, new List<InventoryPanel>() { playerBarterSlots });
        playerName.text = activePlayer.charStats.charName;
        playerGold.text = activePlayer.inventory.money.ToString();

        merchantInventorySlots.PopulateInventory(merchantInventory, new List<InventoryPanel>() { merchantBarterSlots });

        playerBarterSlots.PopulateInventory(playerBarterInv, new List<InventoryPanel>() { playerInventorySlots });

        merchantBarterSlots.PopulateInventory(merchantBarterInv, new List<InventoryPanel>() { merchantInventorySlots });
        UpdateText();
    }

    public void UpdateText()
    {
        if (!IsActive()) return;
        merchantName.text = ""; // TODO: Vis - display merchant char data?
        merchantGold.text = merchantInventory.money.ToString();
        playerBarterValueText.text = playerSaleVal().ToString();
        merchantBarterValueText.text = merchantSaleVal().ToString();
    }

    public override bool IsActive()
    {
        return inventoryMenu.activeSelf;
    }

    public override void ActivateItem(InventoryData itemData, InventoryPanel slotGroup, int slotId)
    {
        Debug.Log("Trading activate");
        var playerInventory = PartyController.Instance.leader.inventory;
        if (slotGroup == playerInventorySlots)
        {
            Debug.Log("player slots");
            var grabbedInv = playerInventory.GetInventory(slotId);
            var leftover = playerBarterInv.AddNewItem(itemData, grabbedInv.count);
            playerInventory.SetInventory(slotId, itemData, leftover);
            Debug.Log("player slots done");
        }
        else if (slotGroup == playerBarterSlots)
        {
            var grabbedInv = playerBarterInv.GetInventory(slotId);
            var leftover = playerInventory.AddNewItem(itemData, grabbedInv.count);
            playerBarterInv.SetInventory(slotId, itemData, leftover);
        }
        else if (slotGroup == merchantInventorySlots)
        {
            var grabbedInv = merchantInventory.GetInventory(slotId);
            var leftover = merchantBarterInv.AddNewItem(itemData, grabbedInv.count);
            merchantInventory.SetInventory(slotId, itemData, leftover);
        }
        else if (slotGroup == merchantBarterSlots)
        {
            var grabbedInv = merchantBarterInv.GetInventory(slotId);
            var leftover = merchantInventory.AddNewItem(itemData, grabbedInv.count);
            merchantBarterInv.SetInventory(slotId, itemData, leftover);
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

    public void Trade() // TODO: what if not enough space?
    {
        var playerInventory = PartyController.Instance.leader.GetComponent<EntityInventory>();

        int playerCost = merchantSaleVal() - playerSaleVal();

        if ((playerCost >= 0 && playerCost <= playerInventory.money) ||
            (playerCost < 0 && playerCost <= merchantInventory.money)) {
            merchantInventory.ConsumeInventory(playerBarterInv);
            playerInventory.ConsumeInventory(merchantBarterInv);
            playerInventory.money -= playerCost;
            merchantInventory.money += playerCost;
        }  
        ActivateMenu();
    }

    private int playerSaleVal()
    {
        int baseVal = playerBarterInv.getTotalValue() / 2;
        int modifier = 1;  // TODO: figure out how persuasion should affect sale and/or buy price
        return baseVal * modifier;
    }

    private int merchantSaleVal()
    {
        int baseVal = merchantBarterInv.getTotalValue();
        int modifier = 1;
        return baseVal / modifier;
    }

    public override void DeselectAllSlots()
    {
        playerInventorySlots.DeselectPanelSlots();
        merchantInventorySlots.DeselectPanelSlots();
        playerBarterSlots.DeselectPanelSlots();
        merchantBarterSlots.DeselectPanelSlots();

        selectedSlotId = -1;
        itemDescriptionImage.sprite = emptySprite;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
    }
}
