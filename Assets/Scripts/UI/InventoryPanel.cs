using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    public GameObject slotPrefab;
    public InventoryMenu inventoryMenu;
    
    protected EntityInventory inventory;
    private List<ItemSlot> InventorySlots;

    void Awake()
    {
        InventorySlots = new List<ItemSlot>();
    }

    public virtual void PopulateInventory(EntityInventory entityinventory, List<InventoryPanel> dragMatches = null)
    {
        ClearInventory();
        inventory = entityinventory;
        Debug.Log("Populate Inventory");
        for (int i = 0; i < inventory.maxInv; i++)
        {
            var slotData = inventory.GetInventory(i);
            var slotItem = Instantiate(slotPrefab, transform);
            InventorySlots.Add(slotItem.GetComponent<ItemSlot>());
            InventorySlots[i].slotID = i;
            InventorySlots[i].slotPanel = this;
            InventorySlots[i].dragMatches = dragMatches;
            //Debug.Log("Inv slot " + i + ": type: " + slotData.type + " count: " + slotData.count);

            if (slotData.type != null) { InventorySlots[i].AddItem(slotData.type, slotData.count, false); }
        }
    }

    public void SelectItem(InventoryData itemData, int slotId)
    {
        inventoryMenu.SelectItem(itemData, this, slotId);
    }

    public void ActivateItem(InventoryData itemData, int slotId)
    {
        Debug.Log("panel activate");
        inventoryMenu.ActivateItem(itemData, this, slotId);
    }

    public virtual void ClearInventory()
    {
        DeselectPanelSlots();
        InventorySlots.ToList().ForEach(slot => Destroy(slot.gameObject));
        InventorySlots = new List<ItemSlot>();
    }

    public virtual void DeselectPanelSlots()
    {
        Debug.Log("Deselect all");
        InventorySlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
    }

    public void DeselectMenuSlots()
    {
        inventoryMenu.DeselectAllSlots();
    }

    public virtual void UpdateEntity()
    {
        Debug.Log("Update Entity");
        InventorySlots.ToList().ForEach(slot => inventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
        EventHandler.Instance.TriggerInventoryUpdate();
    }
}
