using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "BaseCombatBehavior", menuName = "Scriptable Objects/BaseCombatBehavior")]
public class BaseCombatBehavior : ScriptableObject
{
    // TODO: having a set amount of discrete "actions" should make this logic easier.


    public virtual NPC ChooseTarget(NPC attacker)  // TODO: maybe just have one "root" method that gets called and then each enemy type script could have its own logic? Maybe have some shared methods as part of that, like finding nearest enemy.
    {
        return null;
    }

    public virtual IEnumerator CombatTurn(NPC attacker) { 
        return null;
    }

    public virtual void AttackTarget(NPC attacker) { 
    
    }

    public NPC FindClosestTarget(NPC attacker)
    {
        float minDist = Mathf.Infinity;
        NPC closest = null;
        foreach(var target in CombatManager.Instance.Allies)
        {
            var path = attacker.GetComponent<MoveToClick>().PathToPoint(target.npc.transform.position);
            var dist = path != null ? MoveToClick.PathDist(path) : Mathf.Infinity;
            if (dist < minDist)
            {
                minDist = dist;
                closest = target.npc;
            }
        }
        return closest;
    }
}
