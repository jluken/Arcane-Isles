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

    public GameObject playerInventorySlots; // TODO: standardize this as its own data type and use for different inventory menus
    public TMP_Text playerName;
    public TMP_Text playerGold;

    public GameObject merchantInventorySlots;
    public TMP_Text merchantName;
    public TMP_Text merchantGold;

    public static int barterSlotNum = 10;
    public EntityInventory playerBarterInv;
    public GameObject playerBarterSlots;
    public EntityInventory merchantBarterInv;
    public GameObject merchantBarterSlots;

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

    public List<ItemSlot> PlayerInventorySlots;  // TODO: rename and differentiate from panel object (for all inv menus)
    public List<ItemSlot> MerchantInventorySlots;

    public List<ItemSlot> PlayerBarterSlots;
    public List<ItemSlot> MerchantBarterSlots;

    public TMP_Text merchantBarterValueText;
    public TMP_Text playerBarterValueText;

    private void Awake()
    {
        Instance = this;

        PlayerInventorySlots = new List<ItemSlot>();
        MerchantInventorySlots = new List<ItemSlot>();

        PlayerBarterSlots = new List<ItemSlot>();
        MerchantBarterSlots = new List<ItemSlot>();
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
        UpdateEntity();
        inventoryMenu.SetActive(false);
        merchantInventory = null;
    }

    public void SetInventory(EntityInventory inventory = null)
    {
        merchantInventory = inventory;
    }

    public override void ActivateMenu() // TODO: reevaluate constantly calling "ActivateMenu" for every update once panel object is defined
    {
        inventoryMenu.SetActive(true);

        ClearInventorySlots();

        var activePlayer = PartyController.Instance.leader;
        for (int i = 0; i < activePlayer.inventory.maxInv; i++)  // TODO: this initialization also gets moved to Inventory panel type
        {
            var slotData = activePlayer.inventory.GetInventory(i);
            var slotItem = Instantiate(slotPrefab, playerInventorySlots.transform);
            PlayerInventorySlots.Add(slotItem.GetComponent<ItemSlot>());
            PlayerInventorySlots[i].slotID = i;
            PlayerInventorySlots[i].parentMenu = this;
            PlayerInventorySlots[i].slotGroup = PlayerInventorySlots;

            if (slotData.type != null) { PlayerInventorySlots[i].AddItem(slotData.type, slotData.count); }
        }
        playerName.text = activePlayer.charStats.charName;
        playerGold.text = activePlayer.inventory.money.ToString();

        for (int i = 0; i < merchantInventory.maxInv; i++)
        {
            var slotData = merchantInventory.GetInventory(i);
            var slotItem = Instantiate(slotPrefab, merchantInventorySlots.transform);
            MerchantInventorySlots.Add(slotItem.GetComponent<ItemSlot>());
            MerchantInventorySlots[i].slotID = i;
            MerchantInventorySlots[i].parentMenu = this;
            MerchantInventorySlots[i].slotGroup = MerchantInventorySlots;
            if (slotData.type != null) { MerchantInventorySlots[i].AddItem(slotData.type, slotData.count); }
        }
        merchantName.text = ""; // TODO: display merchant char data?
        Debug.Log("Merchant gold for " + merchantInventory + ": " + merchantInventory.money);
        merchantGold.text = merchantInventory.money.ToString();

        for (int i = 0; i < playerBarterInv.maxInv; i++)
        {
            var slotData = playerBarterInv.GetInventory(i);
            var slotItem = Instantiate(slotPrefab, playerBarterSlots.transform);
            PlayerBarterSlots.Add(slotItem.GetComponent<ItemSlot>());
            PlayerBarterSlots[i].slotID = i;
            PlayerBarterSlots[i].parentMenu = this;
            PlayerBarterSlots[i].slotGroup = PlayerBarterSlots;
            if (slotData.type != null) { PlayerBarterSlots[i].AddItem(slotData.type, slotData.count); }
        }
        playerBarterValueText.text = playerBarterInv.getTotalValue().ToString();  // TODO: adjust sale price based on reputation, barter, etc?
        for (int i = 0; i < merchantBarterInv.maxInv; i++)
        {
            var slotData = merchantBarterInv.GetInventory(i);
            var slotItem = Instantiate(slotPrefab, merchantBarterSlots.transform);
            MerchantBarterSlots.Add(slotItem.GetComponent<ItemSlot>());
            MerchantBarterSlots[i].slotID = i;
            MerchantBarterSlots[i].parentMenu = this;
            MerchantBarterSlots[i].slotGroup = MerchantBarterSlots;
            if (slotData.type != null) { MerchantBarterSlots[i].AddItem(slotData.type, slotData.count); }
        }
        merchantBarterValueText.text = merchantBarterInv.getTotalValue().ToString();
    }



    public override bool IsActive()
    {
        return inventoryMenu.activeSelf;
    }

    public override bool overlay => true;

    public override void ClearInventorySlots()  // TODO: combine duplicate code of inventory menus
    {
        DeselectAllSlots();
        PlayerInventorySlots.ToList().ForEach(slot => Destroy(slot.gameObject));
        PlayerInventorySlots = new List<ItemSlot>();
        MerchantInventorySlots.ToList().ForEach(slot => Destroy(slot.gameObject));
        MerchantInventorySlots = new List<ItemSlot>();
        MerchantBarterSlots.ToList().ForEach(slot => Destroy(slot.gameObject));
        MerchantBarterSlots = new List<ItemSlot>();
        PlayerBarterSlots.ToList().ForEach(slot => Destroy(slot.gameObject));
        PlayerBarterSlots = new List<ItemSlot>();
    }

    public override void ActivateItem(InventoryData itemData, List<ItemSlot> slotGroup, int slotId)
    {
        var playerInventory = PartyController.Instance.leader.inventory;
        if (slotGroup == PlayerInventorySlots)
        {
            var grabbedInv = playerInventory.GetInventory(slotId);
            var leftover = playerBarterInv.AddNewItem(itemData, grabbedInv.count);
            playerInventory.SetInventory(slotId, itemData, leftover);
        }
        else if (slotGroup == PlayerBarterSlots)
        {
            var grabbedInv = playerBarterInv.GetInventory(slotId);
            var leftover = playerInventory.AddNewItem(itemData, grabbedInv.count);
            playerBarterInv.SetInventory(slotId, itemData, leftover);
        }
        else if (slotGroup == MerchantInventorySlots)
        {
            var grabbedInv = merchantInventory.GetInventory(slotId);
            var leftover = merchantBarterInv.AddNewItem(itemData, grabbedInv.count);
            merchantInventory.SetInventory(slotId, itemData, leftover);
        }
        else if (slotGroup == MerchantBarterSlots)
        {
            var grabbedInv = merchantBarterInv.GetInventory(slotId);
            var leftover = merchantInventory.AddNewItem(itemData, grabbedInv.count);
            merchantBarterInv.SetInventory(slotId, itemData, leftover);
        }
        ActivateMenu(); // Reactivate menu after resetting through entity data
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

    public void Trade() // TODO: what if not enough space?
    {
        var playerInventory = PartyController.Instance.leader.GetComponent<EntityInventory>();

        int playerCost = merchantBarterInv.getTotalValue() - playerBarterInv.getTotalValue();

        if ((playerCost >= 0 && playerCost <= playerInventory.money) ||
            (playerCost < 0 && playerCost <= merchantInventory.money)) {
            for (int i = 0; i < playerBarterInv.maxInv; i++)
            {
                var slotData = playerBarterInv.GetInventory(i); // TODO: create container function to transfer all contents to another container
                if (slotData.type != null) { merchantInventory.AddNewItem(slotData.type, slotData.count); }
                playerBarterInv.SetInventory(i, null, 0);
            }
            for (int i = 0; i < merchantBarterInv.maxInv; i++)
            {
                var slotData = merchantBarterInv.GetInventory(i);
                if (slotData.type != null) { playerInventory.AddNewItem(slotData.type, slotData.count); }
                merchantBarterInv.SetInventory(i, null, 0);
            }
            playerInventory.money -= playerCost;
            merchantInventory.money += playerCost;
        }  
        ActivateMenu();
    }

    //public InventoryData RemoveItem(int slotID, int amount = 1, bool destroyDrag = false)
    //{
    //    playerInventory.SetInventory(slotID, null, 0);
    //    DeselectAllSlots();
    //    return ItemSlots[slotID].RemoveItem(amount, destroyDrag);
    //}

    public override void UpdateEntity()
    {
        var playerInventory = PartyController.Instance.leader.GetComponent<EntityInventory>();
        PlayerInventorySlots.ToList().ForEach(slot => playerInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));

        if (merchantInventory != null)
            MerchantInventorySlots.ToList().ForEach(slot => merchantInventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));

        PlayerBarterSlots.ToList().ForEach(slot => playerBarterInv.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
        MerchantBarterSlots.ToList().ForEach(slot => merchantBarterInv.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
    }

    public override void DeselectAllSlots()
    {
        PlayerInventorySlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
        MerchantInventorySlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
        PlayerBarterSlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
        MerchantBarterSlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });

        selectedSlotId = -1;
        itemDescriptionImage.sprite = emptySprite;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
    }
}
