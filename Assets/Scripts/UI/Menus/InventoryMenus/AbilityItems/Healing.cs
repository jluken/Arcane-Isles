using System.Collections.Generic;
using UnityEngine;
using static CharStats;

[CreateAssetMenu(fileName = "Healing", menuName = "Scriptable Objects/Healing")]
public class Healing : WeaponItem
{
    public int healingSlots;
    public int healAp;

    public Sprite healingIcon;

    public override List<AbilityAction> ItemActions()
    {
        return new List<AbilityAction>() { new HealAction(name: "Heal", icon: healingIcon, range: 1.5f, healCost: healAp, slots: healingSlots, modifier: StatVal.physick) };
    }
}
