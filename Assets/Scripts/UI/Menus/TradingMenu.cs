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
    private Character merchant;
    private bool buying;

    public InventoryPanel playerInventorySlots;
    public TMP_Text playerName;
    public Image playerAvatar;
    public TMP_Text playerGold;

    public InventoryPanel merchantInventorySlots;
    public TMP_Text merchantName;
    public Image merchantAvatar;
    public TMP_Text merchantGold;

    public static int barterSlotNum = 14;
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
        //SelectionController.Instance.Deselect();
        var playerInventory = PartyController.Instance.selectedPartyMember.GetComponent<EntityInventory>();
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

    public void SetInventory(EntityInventory inventory = null, Character merchant = null, bool isBuying = true)
    {
        merchantInventory = inventory;
        this.merchant = merchant;
        buying = isBuying;
    }

    public override void ActivateMenu()
    {
        inventoryMenu.SetActive(true);

        //ClearInventorySlots();

        var activePlayer = PartyController.Instance.selectedPartyMember;
        playerInventorySlots.PopulateInventory(activePlayer.inventory, new List<InventoryPanel>() { playerBarterSlots });
        playerName.text = activePlayer.charStats.charName;
        playerAvatar.sprite = activePlayer.charStats.charImage;

        merchantInventorySlots.PopulateInventory(merchantInventory, new List<InventoryPanel>() { merchantBarterSlots });
        merchantName.text = merchant != null ? merchant.charStats.charName : "";
        merchantAvatar.sprite = merchant != null ? merchant.charStats.charImage : null; ;

        playerBarterSlots.gameObject.SetActive(buying);
        playerBarterSlots.PopulateInventory(playerBarterInv, new List<InventoryPanel>() { playerInventorySlots });

        merchantBarterSlots.PopulateInventory(merchantBarterInv, new List<InventoryPanel>() { merchantInventorySlots });
        UpdateText();
    }

    public void UpdateText()
    {
        if (!IsActive()) return;
        playerGold.text = PartyController.Instance.selectedPartyMember.inventory.money.ToString();
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
        var playerInventory = PartyController.Instance.selectedPartyMember.inventory;
        if (slotGroup == playerInventorySlots && buying)
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

    public void Trade()  // TODO: track "unique items" that need to be able to be bought back from someone (curio collector?) 
    {
        var playerInventory = PartyController.Instance.selectedPartyMember.GetComponent<EntityInventory>();

        int playerCost = merchantSaleVal() - playerSaleVal();

        if ((playerCost >= 0 && playerCost <= playerInventory.money) ||
            (playerCost < 0 && playerCost <= merchantInventory.money)) {
            //merchantInventory.ConsumeInventory(playerBarterInv);
            playerBarterInv.ClearInventory();
            playerInventory.ConsumeInventory(merchantBarterInv);
            playerInventory.money -= playerCost;
            merchantInventory.money += playerCost;

            merchantBarterInv.DropAllInventory();  // If player has no space, drop on ground
        }  

        ActivateMenu();
    }

    private int playerSaleVal()
    {
        int baseVal = playerBarterInv.getTotalValue() / 2;
        float modifier = PartyController.Instance.selectedPartyMember.charStats.barterModifier;
        Debug.Log("playersaleBaseVal: " + baseVal);
        Debug.Log("modifier: " + modifier);
        var minCoin = (baseVal * modifier) > 0 ? Math.Max(1, (baseVal * modifier)) : 0;
        Debug.Log("minCoin: " + minCoin);
        return Mathf.FloorToInt(minCoin);
    }

    private int merchantSaleVal()
    {
        int baseVal = merchantBarterInv.getTotalValue();
        float modifier = PartyController.Instance.selectedPartyMember.charStats.barterModifier;
        var minCoin = (baseVal * modifier) > 0 ? Math.Max(1, (baseVal * modifier)) : 0;
        return Mathf.FloorToInt(minCoin);
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
