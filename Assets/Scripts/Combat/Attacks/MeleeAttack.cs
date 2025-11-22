using Unity.Burst.CompilerServices;
using UnityEngine;
using static CharStats;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Scriptable Objects/MeleeAttack")]
public class MeleeAttack : AbilityAction
{
    public int attckCost;

    public override bool CheckValidTarget(NPC actor, Selectable target)
    {
        if(attckCost > CombatManager.Instance.ActionPoints) return false;  // TODO: should this be handled somewhere else?
        var dist = Vector3.Distance(actor.gameObject.transform.position, target.gameObject.transform.position);
        Debug.Log("attack dist: " + dist);
        if (target.GetComponent<NPC>() != null && dist < range)
        {
            RaycastHit hit;
            var rayDirection = target.gameObject.transform.position - actor.gameObject.transform.position;
            if (Physics.Raycast(actor.gameObject.transform.position, rayDirection, out hit) && hit.transform == target.gameObject.transform)
            {
                if (hit.transform == target.gameObject.transform)
                {  // Line of sight between player and target
                    return true;
                }

            }
        }
        return false;
    }

    public override int UseAbility(NPC actor, Selectable target)
    {
        var victim = target.GetComponent<NPC>();
        var damage = actor.charStats.GetCurrStat(modifier) + Random.Range(1, damageDie);
        victim.charStats.updateHealth(-1 * damage);  // TODO: null when enemy attacks?

        return attckCost;
    }
}
