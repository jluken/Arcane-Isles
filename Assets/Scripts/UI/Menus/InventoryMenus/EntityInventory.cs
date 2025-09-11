using System;
using System.Collections.Generic;
using System.Linq;
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

    public void SetInventory(int idx, InventoryData itemData, int count=1)
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

    public Dictionary<CharStats.StatVal, int> GetEquipmentStatMods()
    {
        var modifiers = new Dictionary<CharStats.StatVal, int>();
        for (int i = 0; i < equipment.Length; i++)
        {
            if (equipment[i] == null) continue;
            foreach (var equipStat in equipment[i].equipStats)
            {
                if (!modifiers.ContainsKey(equipStat.equipStat)) modifiers[equipStat.equipStat] = 0;
                modifiers[equipStat.equipStat] += equipStat.value;
            }
        }
        return modifiers;
    }

    public void Deselect()
    {

    }

    public int AddNewItem(InventoryData itemData, int newStackSize=1)
    {
        for (int i = 0; i < EntityInventory.invSize; i++)
        {
            if (GetInventory(i).Item2 == 0)
            {
                Debug.Log("empty stack " + i);
                int transferStack = Math.Min(itemData.maxStackSize, newStackSize);
                //ItemSlots[i].AddItem(itemData, transferStack, true);
                SetInventory(i, itemData, transferStack);
                newStackSize -= transferStack;
            }
            else if (GetInventory(i).Item1 == itemData)
            {
                Debug.Log("Same data in " + i + " for " + itemData.itemName);
                int currentStack = GetInventory(i).Item2;
                int freeSpace = itemData.maxStackSize - currentStack;
                int transferStack = Math.Min(freeSpace, newStackSize);
                //ItemSlots[i].AddItem(itemData, transferStack);
                SetInventory(i, itemData, currentStack + transferStack);
                newStackSize -= transferStack;
            }

            if (newStackSize <= 0) return 0;
        }
        //if (menuActivated != null) { ActivateInventory(currentContainer); } // Update menu if currently open
        return newStackSize;
    }

    public float getEncumberance()
    {
        var invWeight = Enumerable.Range(0, invSize).Where(i => inventoryCounts[i] > 0).Select(i => inventoryTypes[i].weight * inventoryCounts[i]).Sum();
        var equippedWeight = Enumerable.Range(0, 4).Where(i => equipment[i] != null).Select(i => equipment[i].weight).Sum();
        return invWeight + equippedWeight;
    }
}
