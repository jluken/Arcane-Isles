using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class EntityInventory : MonoBehaviour
{
    public static int invSize = 40;  // TODO: lock certain "containers" to a set size 

    public InventoryData[] inventoryTypes = new InventoryData[invSize];
    public int[] inventoryCounts = new int[invSize];

    public bool hasEquip;
    public InventoryData[] equipment = new InventoryData[4]; // hardcoded slot indices for now

    public int money;
    public bool merchant;

    public void SetInventory(int idx, InventoryData itemData, int count)
    {
        if (count == 0) itemData = null;
        inventoryTypes[idx] = itemData;
        inventoryCounts[idx] = count;
    }

    public (InventoryData, int) GetInventory(int idx)
    {
        return (inventoryTypes[idx], inventoryCounts[idx]);
    }

    public void SetEquipment(int idx, InventoryData itemData)
    {
        equipment[idx] = itemData;
    }

    public InventoryData GetEquipment(int idx)
    {
        return equipment[idx];
    }

    public Dictionary<InventoryData.StatToChange, int> GetStatMods()
    {
        var modifiers = new Dictionary<InventoryData.StatToChange, int>();
        modifiers[InventoryData.StatToChange.stamina] = 0;
        modifiers[InventoryData.StatToChange.finesse] = 0;
        modifiers[InventoryData.StatToChange.wit] = 0;
        modifiers[InventoryData.StatToChange.health] = 0;
        modifiers[InventoryData.StatToChange.vigor] = 0;
        for (int i = 0; i < equipment.Length; i++)
        {
            if (equipment[i] == null) continue;
            for (int j = 0; j < equipment[i].statsToChange.Length; j++)
            {
                modifiers[equipment[i].statsToChange[j]] += (int)equipment[i].statChanges[j];
            }
        }
        return modifiers;
    }

    public void Deselect()
    {

    }
}
