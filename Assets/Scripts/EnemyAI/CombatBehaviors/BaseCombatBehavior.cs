using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "BaseCombatBehavior", menuName = "Scriptable Objects/BaseCombatBehavior")]
public class BaseCombatBehavior : ScriptableObject
{
    public virtual NPC ChooseTarget(NPC attacker)
    {
        return null;
    }

    public IEnumerator CombatTurn(NPC attacker) {
        while (CanAct(attacker))
        {
            Debug.Log("Do new action");
            yield return DoNextAction(attacker);
        }
        CombatManager.Instance.NextTurn();
        yield return null;
    }

    public virtual bool CanAct(NPC attacker)
    {
        return false;
    }

    public virtual IEnumerator DoNextAction(NPC attacker)
    {
        yield return null;
    }

    public virtual IEnumerator TakeAction()
    {
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
