using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "BugCombatBehavior", menuName = "Scriptable Objects/BugCombatBehavior")]
public class BugCombatBehavior : BaseCombatBehavior
{
    private AttackAction Bite = new AttackAction(attackCost: 4, damageDie: 3, modifier: CharStats.StatVal.vigor, name: "bite", icon: null, range: 1.5f, animation: "bite");
    private MoveToPoint MoveTo = new MoveToPoint(name: "bite", icon: null);
    //private MoveToPoint MoveTowards = new MoveToPoint("bite", null);

    public override Character ChooseTarget(Character attacker)
    {
        Debug.Log("Bug choosing target " + FindClosestTarget(attacker));
        return FindClosestTarget(attacker);
    }

    public override bool CanAct(Character attacker)
    {
        if (CombatManager.Instance.ActionPoints == 0) return false;
        Character target = ChooseTarget(attacker);
        if(target == null) return false;
        Bite.SetActor(attacker);
        Bite.SetTarget(target);
        return Bite.CanUseAbility() || !Bite.CheckValidAction();
    }

    public override IEnumerator DoNextAction(Character attacker)
    {
        Character target = ChooseTarget(attacker);
        if (target == null) yield break;  
        SelectionController.Instance.Select(target);

        Bite.SetActor(attacker);
        Bite.SetTarget(target);
        MoveTo.SetActor(attacker);
        MoveTo.SetTarget(target);
        //MoveTowards.SetActor(attacker);


        if (Bite.CanUseAbility())
        {
            yield return BugBite(target);
        }
        else if (!Bite.CheckValidAction() && MoveTo.CheckValidAction())  // Can be outside of AP and just get as close as possible
        {
            Debug.Log("Movetotarget");
            yield return MoveToTarget(target);
        }
        while (CombatManager.Instance.InAction()) yield return null;
    }

    public IEnumerator BugBite(Character target)
    {
        Bite.SetTarget(target);
        CombatManager.Instance.UseCombatAbility(-1, Bite);
        //while (CombatManager.Instance.inAction) yield return null;
        yield break;
    }

    public IEnumerator MoveToTarget(Character target)
    {
        Debug.Log("Bug Move To");
        MoveTo.SetTarget(target);
        
        CombatManager.Instance.UseCombatAbility(-1, MoveTo);
        //while (CombatManager.Instance.inAction) yield return null;
        yield break;
    }

    public override void AttackTarget(Character attacker)
    {

    }
}
