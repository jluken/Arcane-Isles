using System;
using System.Collections.Generic;
using static EntityInventory;
using static InventoryData;

[System.Serializable]
public class EntityInventorySaveData
{
    public string containerId;

    public int invSize;

    public List<(string, int)> inventory = new List<(string, int)>();

    public bool hasEquip;
    public Dictionary<string, string> equipment = new Dictionary<string, string>();

    public int money;
    public bool merchant;

    public EntityInventorySaveData(EntityInventory entInv)
    {
        containerId = entInv.containerId;
        inventory = new List<(string, int)>();
        for (int j = 0; j < entInv.inventory.Count; j++)
        {
            inventory.Add(entInv.inventory[j].type == null ? ("", 0) : (entInv.GetInventory(j).type.name, entInv.GetInventory(j).count));
        }

        hasEquip = entInv.hasEquip;
        foreach (KeyValuePair<ItemType, InventoryData> kvp in entInv.equipment)
        {
            equipment[kvp.Key.ToString()] = kvp.Value == null ? "" : kvp.Value.name;
        }
        money = entInv.money;
        merchant = entInv.merchant;
    }
}
