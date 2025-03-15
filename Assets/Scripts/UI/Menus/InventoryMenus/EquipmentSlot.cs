using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : ItemSlot
{
    public InventoryData.ItemType equipType;
    public GameObject player; // TODO: these assignments will be handled with a controller once swapping players is functional
    private CharStats charStats;

    public override void Start()
    {
        base.Start();
        charStats = player.GetComponent<CharStats>();
    }

    public override void OnDrop(PointerEventData eventData)
    {
        Debug.Log("EQUIPMENT DROP");
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (draggableItem.inventoryData.itemType == equipType)
        {
            Debug.Log("Not full");
            draggableItem.parentAfterDrag = transform;
            draggableItem.invSlot = -1;
        }
    }

    public override void AddItem(InventoryData itemData, int newStackSize = 1, bool createDrag = false)
    {
        base.AddItem(itemData, newStackSize, createDrag);
        //charStats.UpdateStats();  //TODO: find better management of inventory stuff and where updates happen.
    }

    public override InventoryData RemoveItem(int amount = 1, bool destroyDrag = false)
    {
        var remove = base.RemoveItem(amount, destroyDrag);
        //charStats.UpdateStats();
        return remove;
    }

    public override InventoryData ClearItem(bool destroyDrag = false)
    {
        var clear = base.ClearItem(destroyDrag);
        //charStats.UpdateStats();
        return clear;
    }

    public void ItemAction()
    {
        // TODO: return action based on type - weapon data will also have weapon script (to write), etc
        // TODO: Create manager for player stats and have it check all items in equipment slots
    }
}
