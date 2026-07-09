using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    public GameObject slotPrefab;
    public InventoryMenu inventoryMenu;
    
    protected EntityInventory inventory;
    public List<ItemSlot> InventorySlots;

    void Awake()
    {
        //InventorySlots = new List<ItemSlot>();
    }

    public virtual void PopulateInventory(EntityInventory entityinventory, List<InventoryPanel> dragMatches = null)
    {
        if (entityinventory.maxInv != InventorySlots.Count) Debug.LogError("Inventory does not match menu size");
        ClearInventory();
        inventory = entityinventory;
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            var slotData = inventory.GetInventory(i);
            //var slotItem = Instantiate(slotPrefab, transform);
            //InventorySlots.Add(slotItem.GetComponent<ItemSlot>());
            InventorySlots[i].slotID = i;
            InventorySlots[i].slotPanel = this;
            InventorySlots[i].dragMatches = dragMatches;
            //Debug.Log("Inv slot " + i + ": type: " + slotData.type + " count: " + slotData.count);

            if (slotData.type != null) { Debug.Log("Adding item at slot " + i);  InventorySlots[i].AddItem(slotData.type, slotData.count, false); }
        }
    }

    public ItemSlot NextEmpty()
    {
        return InventorySlots.FirstOrDefault(slot => slot.itemData == null);
    }

    public void SelectItem(InventoryData itemData, int slotId)
    {
        inventoryMenu.SelectItem(itemData, this, slotId);
    }

    public void ActivateItem(InventoryData itemData, int slotId)
    {
        inventoryMenu.ActivateItem(itemData, this, slotId);
    }

    public virtual void ClearInventory()
    {
        DeselectPanelSlots();
        InventorySlots.ToList().ForEach(slot => slot.ClearItem(true, false));  // TODO: Fix destroy leak
        //InventorySlots = new List<ItemSlot>();
    }

    public virtual void DeselectPanelSlots()
    {
        InventorySlots.ToList().ForEach(slot => { slot.selectedShader.SetActive(false); slot.itemSelected = false; });
    }

    public void DeselectMenuSlots()
    {
        inventoryMenu.DeselectAllSlots();
    }

    public virtual void UpdateEntity()
    {
        InventorySlots.ToList().ForEach(slot => inventory.SetInventory(slot.slotID, slot.itemData, slot.currentStack));
        EventHandler.Instance.TriggerInventoryUpdate();
    }
}
