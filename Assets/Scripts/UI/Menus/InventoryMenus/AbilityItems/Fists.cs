using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fists", menuName = "Scriptable Objects/Fists")]
public class Fists : WeaponItem
{

    public Sprite punchIcon;

    public override AbilityAction DefaultAttack()
    {
        return new AttackAction(name: "Punch", icon: punchIcon, range: 1.5f, damageDie: 4, attackCost: 4, modifier: CharStats.StatVal.survival);
    }

    public override List<AbilityAction> ItemActions()
    {
        return new List<AbilityAction>() { new AttackAction(name: "Punch", icon: punchIcon, range: 1.5f, damageDie: 4, attackCost: 4, modifier: CharStats.StatVal.survival) };
    }
}
