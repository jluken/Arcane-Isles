using UnityEngine;

public class Container : Selectable
{
    
    private InventoryManager inventoryManager;
    private EntityInventory inventory;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        inventory = gameObject.GetComponent<EntityInventory>();
        base.Start();
    }

    public override void Activate()
    {
        inventoryManager.ActivateInventory(inventory);
        base.Deselect();
        base.Activate();
    }
}
