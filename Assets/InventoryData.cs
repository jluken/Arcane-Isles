using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Scriptable Objects/InventoryData")]
public class InventoryData : ScriptableObject
{

    public string itemName;
    [TextArea]
    public string description;
    public Sprite sprite;
    public int price;

    public int maxStackSize= 1;

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

    public StatToChange[] statsToChange;
    public float[] statChanges;

    public enum StatToChange
    {
        none,
        health,  // TODO: different one for max health?
        stamina,
        vigor,
        finesse,
        wit
    };

    public InventoryData(string name, string description, Sprite sprite)
    {
        this.itemName = name;
        this.description = description;
        this.sprite = sprite;
    }

    public void UseItem()
    {
        if (statsToChange.Length != statChanges.Length) {
            Debug.LogError("Stat list and changes do not match");
        }
        for (int i = 0; i < statsToChange.Length; i++)
        {
            Debug.Log("Increase " + statsToChange[i] + " by " + statChanges[i]);
        }
        

        //if (statToChange == StatToChange.health)
        //{
        //    Debug.Log("Increase Health by " + amountToChange);
        //    //GameObject.Find("HealthManager").GetComponent<PlayerHealth>().ChangeHealth(amountToChange);
        //}
    }
}
