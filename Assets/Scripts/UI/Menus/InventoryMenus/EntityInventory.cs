using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static InventoryData;

public class EntityInventory : MonoBehaviour
{
    public string containerId;
    
    public int maxInv = 42;

    // For the purposes of setting initial inventory through editor
    public List<InventoryData> initInv;
    public List<int> initInvStacks;

    public InventoryData initHoldMainHand;
    public InventoryData initHoldOffHand;
    public InventoryData initTorso;
    public InventoryData initCoat;
    public InventoryData initHead;
    public InventoryData initFace;
    public InventoryData initLegs;
    public InventoryData initBoots;
    public InventoryData initNecklace;
    public InventoryData initHands;
    //public InventoryData initLeftHand;

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


    public enum EquipmentInvType
    {
        holdMainHand,
        holdOffHand,
        torso,
        coat,
        head,
        face,
        legs,
        boots,
        neck,
        hands,
        //leftHand,
        na
    };

    public static Dictionary<EquipmentInvType, HashSet<ItemType>> equippables = new() {
        { EquipmentInvType.holdMainHand, new () { ItemType.weapon, ItemType.consumable, ItemType.misc } },
        { EquipmentInvType.holdOffHand, new () { ItemType.consumable, ItemType.misc } }, // TODO: feat that lets you put some weapons in offhand (cheaper AP than two mainhand attacks)
        { EquipmentInvType.torso, new () { ItemType.torso } },
        { EquipmentInvType.coat, new () { ItemType.coat } },
        { EquipmentInvType.head, new () { ItemType.headwear } },
        { EquipmentInvType.face, new () { ItemType.face } },
        { EquipmentInvType.legs, new () { ItemType.legwear } },
        { EquipmentInvType.boots, new () { ItemType.footwear } },
        { EquipmentInvType.neck, new () { ItemType.neckwear } },
        { EquipmentInvType.hands, new () { ItemType.handwear } },
        //{ EquipmentInvType.leftHand, new () { ItemType.handwear } }
    };

    public static Dictionary<ItemType, List<EquipmentInvType>> defaultEquipSlot = new()
    {
        { ItemType.weapon, new() { EquipmentInvType.holdMainHand } }, // TODO: add offhand when perk is on
        { ItemType.torso, new() { EquipmentInvType.torso } },
        { ItemType.coat, new() { EquipmentInvType.coat } },
        { ItemType.headwear, new() { EquipmentInvType.head } },
        { ItemType.face, new() { EquipmentInvType.face } },
        { ItemType.legwear, new() { EquipmentInvType.legs } },
        { ItemType.footwear, new() { EquipmentInvType.boots } },
        { ItemType.neckwear, new() { EquipmentInvType.neck } },
        { ItemType.handwear, new() { EquipmentInvType.hands } }
    };

    public Dictionary<EquipmentInvType, InventoryData> equipment = new Dictionary<EquipmentInvType, InventoryData>() {
        { EquipmentInvType.holdMainHand, null },
        { EquipmentInvType.holdOffHand, null },
        { EquipmentInvType.torso, null },
        { EquipmentInvType.coat, null },
        { EquipmentInvType.head, null },
        { EquipmentInvType.face, null },
        { EquipmentInvType.legs, null },
        { EquipmentInvType.boots, null },
        { EquipmentInvType.neck, null },
        { EquipmentInvType.hands, null },
        //{ EquipmentInvType.leftHand, null }
    };

    public int money;
    public bool merchant;

    public void Start()
    {
        SetInitInventory();
    }

    public void SetInitInventory()
    {
        if (initInv.Count > maxInv) Debug.LogError("Present inventory greater than maximum");
        ClearInventory();
        for (int i = 0; i < initInv.Count; i++)
        {
            int stack = i < initInvStacks.Count ? initInvStacks[i] : 1;
            inventory.Add(new InventoryStack(initInv[i], stack));
        }
        equipment[EquipmentInvType.holdMainHand] = initHoldMainHand;
        equipment[EquipmentInvType.holdOffHand] = initHoldOffHand;
        equipment[EquipmentInvType.torso] = initTorso;
        equipment[EquipmentInvType.coat] = initCoat;
        equipment[EquipmentInvType.head] = initHead;
        equipment[EquipmentInvType.face] = initFace;
        equipment[EquipmentInvType.legs] = initLegs;
        equipment[EquipmentInvType.boots] = initBoots;
        equipment[EquipmentInvType.neck] = initNecklace;
        equipment[EquipmentInvType.hands] = initHands;
        //equipment[EquipmentInvType.leftHand] = initLeftHand;
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
            if(!Enum.TryParse(kvp.Key, out EquipmentInvType equipType)) Debug.LogError("Invalid equipment type " + kvp.Key);
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

    public void UseMainWeapon()
    {
        if (equipment[EquipmentInvType.holdMainHand] != null && equipment[EquipmentInvType.holdMainHand].consumeOnUse) {
            SetEquipment(EquipmentInvType.holdMainHand, null);
        }
    }

    public void SetInventory(int idx, InventoryData itemData, int count=1)
    {
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

    public InventoryData SwapOutEquipment(InventoryData inv)
    {
        if (!defaultEquipSlot.ContainsKey(inv.itemType)) return inv;
        var targetSlot = defaultEquipSlot[inv.itemType][0];
        if (equipment[targetSlot] != null) {  // Check to see if any of the alt slots are open, otherwise just use primary
            for (int i = 1; i < defaultEquipSlot[inv.itemType].Count; i++) {
                var altSlot = defaultEquipSlot[inv.itemType][i];
                if (equipment[altSlot] == null) { targetSlot = altSlot; break; }
            }
        }
        var retInv = GetEquipment(targetSlot);
        SetEquipment(targetSlot, inv);
        return retInv;
    }

    public void SetEquipment(EquipmentInvType type, InventoryData itemData)
    {
        Debug.Log("Set Equipment type " + type + " to " + itemData);
        equipment[type] = itemData;
    }

    public void Dequip(EquipmentInvType equipSlot)
    {
        InventoryData leftover = null;
        if (equipment.ContainsKey(equipSlot) && equipment[equipSlot] != null)
        {
            int extra = AddNewItem(equipment[equipSlot]);
            if (extra > 0) leftover = equipment[equipSlot];
            equipment[equipSlot] = null;
        }
        if (leftover != null) leftover.DropItem(transform.position);
    }

    public InventoryData GetEquipment(EquipmentInvType type)
    {
        return equipment[type];
    }

    public Dictionary<CharStats.StatVal, int> GetEquipmentStatMods()
    {
        var modifiers = new Dictionary<CharStats.StatVal, int>();
        foreach (EquipmentInvType type in equipment.Keys)
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

    public int GetEquipmentArmor()
    {
        var dt = 0;
        foreach (EquipmentInvType type in equipment.Keys)
        {
            if (equipment[type] == null) continue;
            dt += equipment[type].dt;
        }
        return dt;
    }

    public int AddNewItem(InventoryData itemData, int newStackSize=1)
    {
        for (int i = 0; i < maxInv; i++)
        {
            if (GetInventory(i).count == 0)
            {
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

    public void DropAllInventory()
    {
        foreach (var equipSlot in equipment.Keys) Dequip(equipSlot);
        foreach (var stack in inventory)
        {
            stack.type.DropItem(transform.position, stack.count);
        }
        ClearInventory();
    }

    public void ClearInventory()
    {
        inventory = new List<InventoryStack>();

        foreach (EquipmentInvType equippable in Enum.GetValues(typeof(EquipmentInvType))) equipment[equippable] = null;
    }

    public float getTotalWeight()
    {
        var invWeight = Enumerable.Range(0, inventory.Count).Where(i => GetInventory(i).count > 0).Select(i => GetInventory(i).type.weight * GetInventory(i).count).Sum();
        foreach (EquipmentInvType type in equipment.Keys) if (GetEquipment(type) != null) invWeight += GetEquipment(type).weight;
        return invWeight;
    }

    public float getEquippedWeight()
    {
        var weight = 0f;
        foreach (EquipmentInvType type in equipment.Keys)
        {
            if (equipment[type] == null) continue;
            weight += equipment[type].weight;
        }
        return weight;
    }

    public int getTotalValue()
    {
        var invVal = Enumerable.Range(0, inventory.Count).Where(i => GetInventory(i).count > 0).Select(i => GetInventory(i).type.price * GetInventory(i).count).Sum();
        foreach (EquipmentInvType type in equipment.Keys) if (GetEquipment(type) != null) invVal += GetEquipment(type).price;
        Debug.Log("Total value for " + this + ": " + (invVal + money));
        return invVal + money;
    }
}
