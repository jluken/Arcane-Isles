using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static CharStats;

[CreateAssetMenu(fileName = "Sword", menuName = "Scriptable Objects/Sword")]
public class Sword : WeaponItem
{
    public int damageDie;
    public int swingAp;
    public int modifier;
    public bool power;
    public bool precision;

    public Sprite swordIcon;

    public override AbilityAction DefaultAttack()
    {
        return new AttackAction(name: "Swing", icon: swordIcon, range: 1.5f, attackCost: swingAp, damageDice: damageDie, modifier: modifier, animation: "sword", powerAttack: power, precisionAttack: precision);
    }

    public override List<AbilityAction> ItemActions()
    {
        return new List<AbilityAction>() { new AttackAction(name: "Swing", icon: swordIcon, range: 1.5f, attackCost: swingAp, damageDice: damageDie, modifier: modifier, animation: "sword", powerAttack: power, precisionAttack: precision) };
        // TODO: steal ideas from 5e24's knick/etc; have minor action can take even if not proper attack
    }
}
