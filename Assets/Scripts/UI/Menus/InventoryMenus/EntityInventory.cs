using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using static InventoryData;

public class EntityInventory : MonoBehaviour
{
    public string containerId;
    
    public int maxInv = 40;

    // For the purposes of setting initial inventory through editor
    public List<InventoryData> initInv;
    public List<int> initInvStacks;

    public struct InventoryStack
    {
        public InventoryData type;
        public int count;

        public InventoryStack(InventoryData type, int count)
        {
            this.type = count > 0 ? type : null;
            this.count = type != null ? count : 0;
        }
    }

    public List<InventoryStack> inventory = new List<InventoryStack>();

    public bool hasEquip;


    public Dictionary<ItemType, InventoryData> equipment = new Dictionary<ItemType, InventoryData>() {
        { ItemType.headwear, null},
        { ItemType.armor, null},
        { ItemType.weapon, null},
        { ItemType.boots, null}
    };

    public int money;
    public bool merchant;

    public void Start()
    {
        if (initInv.Count > maxInv) Debug.LogError("Present inventory greater than maximum");
        inventory = new List<InventoryStack>();
        for (int i = 0; i < initInv.Count; i++) { 
            int stack = i < initInvStacks.Count ? initInvStacks[i] : 1;
            inventory.Add(new InventoryStack(initInv[i], stack));
        }
    }

    public void LoadFromSaveData(EntityInventorySaveData saveData)
    {
        inventory = new List<InventoryStack>();
        for (int j = 0; j < saveData.inventory.Count; j++)
        {
            inventory.Add(saveData.inventory[j].Item1 == "" ? new InventoryStack(null, 0) :
                new InventoryStack(Resources.Load<InventoryData>("Scriptables/" + saveData.inventory[j].Item1), saveData.inventory[j].Item2));
        }

        hasEquip = saveData.hasEquip;
        foreach (KeyValuePair<string, string> kvp in saveData.equipment)
        {
            if(!Enum.TryParse(kvp.Key, out ItemType equipType)) Debug.LogError("Invalid equipment type " + kvp.Key);
            equipment[equipType] = kvp.Value == "" ? null : Resources.Load<InventoryData>("Scriptables/" + kvp.Value);
        }

        money = saveData.money;
        merchant = saveData.merchant;
    }

    public void UpdateInvStack(int idx, int change)
    {
        var currStack = GetInventory(idx);
        SetInventory(idx, currStack.type, currStack.count + change);
    }

    public void SetInventory(int idx, InventoryData itemData, int count=1)
    {
        if (idx >= maxInv) Debug.LogError("Setting inventory outside of bounds");
        if (count <= 0) itemData = null;
        if (itemData == null) count = 0;

        while (inventory.Count <= idx) inventory.Add(new InventoryStack(null, 0));
        inventory[idx] = new InventoryStack(itemData, count);
    }

    public InventoryStack GetInventory(int idx)
    {
        if (idx >= maxInv) Debug.LogError("Getting inventory outside of bounds");
        if (idx >= inventory.Count) return new InventoryStack(null, 0);
        return inventory[idx];
    }

    public void SetEquipment(ItemType type, InventoryData itemData)
    {
        Debug.Log("Set Equipment type " + type + " to " + itemData);
        equipment[type] = itemData;
    }

    public InventoryData Dequip(ItemType equipSlot)
    {
        InventoryData leftover = null;
        if (equipment.ContainsKey(equipSlot) && equipment[equipSlot] != null)
        {
            int extra = AddNewItem(equipment[equipSlot]);
            if (extra > 0) leftover = equipment[equipSlot];
            equipment[equipSlot] = null;
        }
        return leftover;  // TODO: drop leftover equipment on the ground if no room
    }

    public InventoryData GetEquipment(ItemType type)
    {
        return equipment[type];
    }

    public Dictionary<CharStats.StatVal, int> GetEquipmentStatMods()
    {
        var modifiers = new Dictionary<CharStats.StatVal, int>();
        foreach (ItemType type in equipment.Keys)
        {
            if (equipment[type] == null) continue;
            foreach (var equipStat in equipment[type].equipStats)
            {
                if (!modifiers.ContainsKey(equipStat.equipStat)) modifiers[equipStat.equipStat] = 0;
                modifiers[equipStat.equipStat] += equipStat.value;
            }
        }
        return modifiers;
    }

    public int AddNewItem(InventoryData itemData, int newStackSize=1)
    {
        for (int i = 0; i < maxInv; i++)
        {
            if (GetInventory(i).count == 0)
            {
                //Debug.Log("empty stack " + i);
                int transferStack = Math.Min(itemData.maxStackSize, newStackSize);
                //ItemSlots[i].AddItem(itemData, transferStack, true);
                SetInventory(i, itemData, transferStack);
                newStackSize -= transferStack;
            }
            else if (GetInventory(i).type == itemData)
            {
                //Debug.Log("Same data in " + i + " for " + itemData.itemName);
                int currentStack = GetInventory(i).count;
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

    public void ConsumeInventory(EntityInventory newInventory)
    {
        // Empty everything from inventory into this inventory
        if (newInventory == null) Debug.LogError("This should never be null in the collect all method");
        for (int i = 0; i < newInventory.inventory.Count; i++)
        {
            var invItem = newInventory.GetInventory(i);
            if (invItem.type == null) continue;

            int leftover = AddNewItem(invItem.type, invItem.count);
            newInventory.SetInventory(i, invItem.type, leftover);
        }
    }

    public float getTotalWeight()
    {
        var invWeight = Enumerable.Range(0, inventory.Count).Where(i => GetInventory(i).count > 0).Select(i => GetInventory(i).type.weight * GetInventory(i).count).Sum();
        foreach (ItemType type in equipment.Keys) if (GetEquipment(type) != null) invWeight += GetEquipment(type).weight;
        return invWeight;
    }

    public int getTotalValue()
    {
        var invVal = Enumerable.Range(0, inventory.Count).Where(i => GetInventory(i).count > 0).Select(i => GetInventory(i).type.price * GetInventory(i).count).Sum();
        foreach (ItemType type in equipment.Keys) if (GetEquipment(type) != null) invVal += GetEquipment(type).price;
        Debug.Log("Total value for " + this + ": " + invVal + money);
        return invVal + money;
    }
}
