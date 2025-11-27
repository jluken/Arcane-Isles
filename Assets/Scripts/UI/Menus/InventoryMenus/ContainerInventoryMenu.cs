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

    public GameObject playerInventorySlots;
    public GameObject containerInventorySlots;
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

    public List<ItemSlot> PlayerInventorySlots;
    public List<ItemSlot> ContainerInventorySlots;

    private void Awake()
    {
        Instance = this;

        PlayerInventorySlots = new List<ItemSlot>();
        ContainerInventorySlots = new List<ItemSlot>();
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

        for (int i = 0; i < PartyController.Instance.leader.inventory.maxInv; i++)
        {
            var slotData = PartyController.Instance.leader.inventory.GetInventory(i);
            var slotItem = Instantiate(slotPrefab, playerInventorySlots.transform);
            PlayerInventorySlots.Add(slotItem.GetComponent<ItemSlot>());
            PlayerInventorySlots[i].slotID = i;
            PlayerInventorySlots[i].parentMenu = this;
            PlayerInventorySlots[i].slotGroup = PlayerInventorySlots;

            if (slotData.type != null) { PlayerInventorySlots[i].AddItem(slotData.type, slotData.count); }
        }
        for (int i = 0; i < currentInventory.maxInv; i++)
        {
            var slotData = currentInventory.GetInventory(i);
            var slotItem = Instantiate(slotPrefab, containerInventorySlots.transform);
            ContainerInventorySlots.Add(slotItem.GetComponent<ItemSlot>());
            ContainerInventorySlots[i].slotID = i;
            ContainerInventorySlots[i].parentMenu = this;
            ContainerInventorySlots[i].slotGroup = ContainerInventorySlots;
            if (slotData.type != null) { ContainerInventorySlots[i].AddItem(slotData.type, slotData.count); }
        }
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
        ContainerInventorySlots.ToList().ForEach(slot => Destroy(slot.gameObject));
        ContainerInventorySlots = new List<ItemSlot>();
    }

    public override void ActivateItem(InventoryData itemData, List<ItemSlot> slotGroup, int slotId)
    {
        var playerInventory = PartyController.Instance.leader.inventory;
        if (slotGroup == PlayerInventorySlots)
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

    public void CollectAll()
    {
        var playerInventory = PartyController.Instance.leader.GetComponent<EntityInventory>();
        if (currentInventory == null)
        {
            Debug.LogError("This should never be null in the collect all method");
        }
        var containerEntity = currentInventory;
        for (int i = 0; i < containerEntity.inventory.Count; i++)
        {
            var invItem = containerEntity.GetInventory(i);
            if (invItem.type == null) continue;

            int leftover = playerInventory.AddNewItem(invItem.type, invItem.count);
            containerEntity.SetInventory(i, invItem.type, leftover);
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

    public override void UpdateEntity()
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
