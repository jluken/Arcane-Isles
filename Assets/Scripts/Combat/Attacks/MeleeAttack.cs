using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static CharStats;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Scriptable Objects/MeleeAttack")]
public class MeleeAttack : AbilityAction
{
    public int attackCost;

    public override bool CheckValidTarget(NPC actor, Selectable target)
    {
        if(attackCost > CombatManager.Instance.ActionPoints) return false;
        var dist = Vector3.Distance(actor.gameObject.transform.position, target.gameObject.transform.position);
        Debug.Log("attack dist: " + dist);
        if (target.GetComponent<NPC>() != null && dist < range)
        {
            return Utils.LineOfSight(actor.gameObject, target.gameObject);
        }
        return false;
    }

    public override IEnumerator UseAbility(NPC actor, Selectable target)
    {
        var victim = target.GetComponent<NPC>();
        var damage = actor.charStats.GetCurrStat(modifier) + Random.Range(1, damageDie);
        victim.charStats.updateHealth(-1 * damage);

        CombatManager.Instance.SpendActionPoints(attackCost); // account for floating point and wiggle room
        CombatManager.Instance.FinishAction();
        yield break;
    }
}
