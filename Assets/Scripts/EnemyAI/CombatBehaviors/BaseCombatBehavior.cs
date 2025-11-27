using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "BaseCombatBehavior", menuName = "Scriptable Objects/BaseCombatBehavior")]
public class BaseCombatBehavior : ScriptableObject
{
    public virtual NPC ChooseTarget(NPC attacker)
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
