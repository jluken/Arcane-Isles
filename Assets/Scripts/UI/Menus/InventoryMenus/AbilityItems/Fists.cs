using System.Collections.Generic;
using UnityEngine;
using static CharStats;

[CreateAssetMenu(fileName = "Fists", menuName = "Scriptable Objects/Fists")]
public class Fists : WeaponItem
{

    public Sprite punchIcon;

    public override AbilityAction DefaultAttack()
    {
        return new AttackAction(name: "Punch", icon: punchIcon, range: 1.5f, damageDice: 1, attackCost: 5, modifier: -5, powerAttack: true);
    }

    public override List<AbilityAction> ItemActions()
    {
        return new List<AbilityAction>() { new AttackAction(name: "Punch", icon: punchIcon, range: 1.5f, damageDice: 1, attackCost: 5, modifier: -5, powerAttack: true) };
    }
}
