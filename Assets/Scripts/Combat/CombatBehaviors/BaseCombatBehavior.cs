using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "BaseCombatBehavior", menuName = "Scriptable Objects/BaseCombatBehavior")]
public class BaseCombatBehavior : ScriptableObject
{
    public virtual Character ChooseTarget(Character attacker)
    {
        return null;
    }

    public IEnumerator CombatTurn(Character attacker) {
        CombatManager.Instance.UncarveTargets();
        yield return null;
        yield return new WaitForSeconds(0.5f);
        while (CanAct(attacker))
        {
            Debug.Log("Do new action");
            yield return DoNextAction(attacker);
        }
        CombatManager.Instance.RecarveTargets();
        yield return null;
        yield return new WaitForSeconds(0.5f);
        CombatManager.Instance.NextTurn();
        yield return null;
    }

    public virtual bool CanAct(Character attacker)
    {
        return false;
    }

    public virtual IEnumerator DoNextAction(Character attacker)
    {
        yield return null;
    }

    public virtual IEnumerator TakeAction()
    {
        return null;
    }

    public virtual void AttackTarget(Character attacker) { 
    
    }

    public Character FindClosestTarget(Character attacker)
    {
        float minDist = Mathf.Infinity;
        Character closest = null;
        foreach(var target in CombatManager.Instance.Allies)
        {
            var path = attacker.GetComponent<MoveToClick>().PathToObj(target.character);
            var dist = path != null ? MoveToClick.PathDist(path) : Mathf.Infinity;
            if (dist < minDist)
            {
                minDist = dist;
                closest = target.character;
            }
        }
        return closest;
    }
}
