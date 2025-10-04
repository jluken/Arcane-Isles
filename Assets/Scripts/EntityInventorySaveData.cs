using System;
using System.Reflection;
using UnityEngine;
using static PartyData;

[System.Serializable]
public class EntityInventorySaveData
{
    public string containerId;

    public static int invSize = 40;  // TODO: lock certain "containers" to a set size 

    public string[] inventoryTypes = new string[invSize];
    public int[] inventoryCounts = new int[invSize];

    public bool hasEquip;
    public string[] equipment = new string[4]; // hardcoded slot indices for now

    public int money;
    public bool merchant;

    public EntityInventorySaveData(EntityInventory entInv)
    {
        containerId = entInv.containerId;
        for (int j = 0; j < entInv.inventoryTypes.Length; j++)
        {
            inventoryTypes[j] = entInv.inventoryTypes[j] == null ? "" : entInv.inventoryTypes[j].name;
        }
        Array.Copy(entInv.inventoryCounts, inventoryCounts, invSize);
        equipment = new string[4];
        for (int j = 0; j < entInv.equipment.Length; j++)
        {
            equipment[j] = entInv.equipment[j] == null ? "" : entInv.equipment[j].name;
        }

        hasEquip = entInv.hasEquip;
        money = entInv.money;
        merchant = entInv.merchant;
    }
}
