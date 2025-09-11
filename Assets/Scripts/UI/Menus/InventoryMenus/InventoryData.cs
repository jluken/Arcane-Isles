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
        public int duration; // can set to "None" for permanent
    }
    public List<ConsumeStatEntry> consumeStats;

    public InventoryData(string name, string description, Sprite sprite)
    {
        this.itemName = name;
        this.description = description;
        this.sprite = sprite;
    }

    //public float GetStatModifier(CharStats.StatVal stat)
    //{
    //    return Enumerable.Range(0, statsToChange.Length).Where(i => statsToChange[i] == stat).Select(i => statChanges[i]).Sum();
    //}

    //public void UseItem()
    //{
    //    //if (statsToChange.Length != statChanges.Length) {
    //    //    Debug.LogError("Stat list and changes do not match");
    //    //}
    //    for (int i = 0; i < statsToChange.Length; i++)
    //    {
    //        Debug.Log("Increase " + statsToChange[i] + " by " + statChanges[i]);
    //    }


    //    //if (statToChange == StatToChange.health)
    //    //{
    //    //    Debug.Log("Increase Health by " + amountToChange);
    //    //    //GameObject.Find("HealthManager").GetComponent<PlayerHealth>().ChangeHealth(amountToChange);
    //    //}
    //}

    //public void UseItem()
    //{
    //    // Currently only triggering for consumables, and only thing consumables do is change stats
    //    if (this.itemType == ItemType.consumable)
    //    {
    //        itemPrefab.GetComponent<UsableItem>()
    //    }
    //}

    public void DropItem(Vector3 location)
    {
        // TODO: call from inventory button that deletes as well
        Instantiate(itemPrefab, location, Quaternion.identity);
    }
}
