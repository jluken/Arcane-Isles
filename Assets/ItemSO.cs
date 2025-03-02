using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public StatToChange statToChange = new StatToChange();
    public int amountToChange;

    public enum StatToChange
    {
        none,
        health,
        mana,
        stamina
    };

    public void UseItem()
    {
        if (statToChange == StatToChange.health)
        {
            Debug.Log("Increase Health by " + amountToChange);
            //GameObject.Find("HealthManager").GetComponent<PlayerHealth>().ChangeHealth(amountToChange);
        }
    }
}
