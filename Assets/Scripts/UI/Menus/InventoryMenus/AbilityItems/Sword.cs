using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "Scriptable Objects/Sword")]
public class Sword : WeaponItem
{
    public int damageDie;
    public int swingAp;

    public Sprite swordIcon;

    public override AbilityAction DefaultAttack()
    {
        return new AttackAction(name: "Swing", icon: swordIcon, range: 1.5f, attackCost: swingAp, damageDie: damageDie, modifier:CharStats.StatVal.survival);
    }

    public override List<AbilityAction> ItemActions()
    {
        return new List<AbilityAction>() { new AttackAction(name: "Swing", icon: swordIcon, range: 1.5f, attackCost: swingAp, damageDie: damageDie, modifier: CharStats.StatVal.survival) };
        // TODO: steal ideas from 5e24's knick/etc; have minor action can take even if not proper attack
    }
}
