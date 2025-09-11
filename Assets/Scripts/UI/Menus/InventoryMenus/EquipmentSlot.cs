using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : ItemSlot
{
    public InventoryData.ItemType equipType;
    public GameObject player;

    public override void Start()
    {
        base.Start();
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
    }

    public override InventoryData RemoveItem(int amount = 1, bool destroyDrag = false)
    {
        var remove = base.RemoveItem(amount, destroyDrag);
        return remove;
    }

    public override InventoryData ClearItem(bool destroyDrag = false)
    {
        var clear = base.ClearItem(destroyDrag);
        return clear;
    }
}
