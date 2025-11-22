using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Scriptable Objects/InventoryData")]
public class InventoryData : ScriptableObject
{

    public string itemName;
    [TextArea]
    public string description;
    public Sprite sprite;
    public GameObject itemPrefab; //TODO: hold scripts for droppable, usable/equippable
    public int price;
    public float weight;

    public int APCost; // Cost to use/equip the item

    public List<AbilityAction> abilities;

    public int maxStackSize = 1;

    public ItemType itemType;

    public enum ItemType
    {
        consumable,
        weapon,
        armor,
        headwear,
        boots,
        misc
    };

    [Serializable]
    public class EquipStatEntry
    {
        public CharStats.StatVal equipStat;
        public int value;
    }
    public List<EquipStatEntry> equipStats;

    [Serializable]
    public class ConsumeStatEntry
    {
        public CharStats.StatVal consumeStat;
        public int value;
        public int duration; // Set to 0 for permanent
    }
    public List<ConsumeStatEntry> consumeStats;

    public InventoryData(string name, string description, Sprite sprite)
    {
        this.itemName = name;
        this.description = description;
        this.sprite = sprite;
    }

    public void DropItem(Vector3 location)
    {
        // TODO: call from inventory button that deletes as well
        Instantiate(itemPrefab, location, Quaternion.identity);
    }
}
