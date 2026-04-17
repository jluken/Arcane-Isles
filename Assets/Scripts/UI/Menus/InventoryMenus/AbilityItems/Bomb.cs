using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bomb", menuName = "Scriptable Objects/Bomb")]
public class Bomb : WeaponItem
{
    public int damageDie;
    public int swingAp;
    public float range;
    public float radius;

    public Sprite bombIcon;

    public override AbilityAction DefaultAttack()
    {
        return new AoeAttackAction(name: "Throw", icon: bombIcon, range: range, weaponAttack: true, attackCost: swingAp, damageDie: damageDie, radius: radius);
    }

    public override List<AbilityAction> ItemActions()
    {
        return new List<AbilityAction>() { new AoeAttackAction(name: "Throw", icon: bombIcon, range: range, weaponAttack: true, attackCost: swingAp, damageDie: damageDie, radius: radius) };
    }
}
