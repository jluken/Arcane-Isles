using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : ItemSlot
{
    public EntityInventory.EquipmentInvType equipType;

    public override void Start()
    {
        base.Start();
    }

    public override void OnDrop(PointerEventData eventData)
    {
        Debug.Log("EQUIPMENT DROP");
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        Debug.Log(equipType);
        Debug.Log(draggableItem.inventoryData.itemType);
        if (EntityInventory.equippables[equipType].Contains(draggableItem.inventoryData.itemType))
        {
            Debug.Log("Not full");
            if (!CombatManager.Instance.CheckActionPoints(draggableItem.inventoryData.EquipAPCost)) return;
            CombatManager.Instance.SpendActionPoints(draggableItem.inventoryData.EquipAPCost);
            Debug.Log("Cleared AP");

            draggableItem.parentAfterDrag = transform;
            draggableItem.invSlot = slotID;
        }
    }

    //public override void AddItem(InventoryData itemData, int newStackSize = 1, bool applyChanges = true)
    //{
    //    base.AddItem(itemData, newStackSize);
    //}

    //public override InventoryData RemoveItem(int amount = 1, bool destroyDrag = false)
    //{
    //    var remove = base.RemoveItem(amount, destroyDrag);
    //    return remove;
    //}

    //public override InventoryData ClearItem(bool destroyDrag = false, bool applyChanges = true)
    //{
    //    var clear = base.ClearItem(destroyDrag, applyChanges);
    //    return clear;
    //}
}
