using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "BugCombatBehavior", menuName = "Scriptable Objects/BugCombatBehavior")]
public class BugCombatBehavior : BaseCombatBehavior
{
    private AttackAction Bite = new AttackAction(attackCost: 4, damageDie: 3, modifier: CharStats.StatVal.survival, name: "bite", icon: null, range: 1.5f);
    private MoveToObject MoveTo = new MoveToObject(name: "bite", icon: null, range: float.PositiveInfinity);
    private MoveToPoint MoveTowards = new MoveToPoint("bite", null);

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
        var inBiteRange = false;
        var dist = Vector3.Distance(attacker.gameObject.transform.position, target.gameObject.transform.position);
        if (target.GetComponent<Character>() != null && dist < 1)
        {
            inBiteRange = Utils.LineOfSight(attacker.gameObject, target.gameObject);
        }
        Bite.SetActor(attacker);
        if (inBiteRange && !Bite.CheckValidTarget(target)) return false;
        return true;
    }

    public override IEnumerator DoNextAction(Character attacker)
    {
        Character target = ChooseTarget(attacker);
        if (target == null) yield break;  
        target.Select();

        Bite.SetActor(attacker);
        MoveTo.SetActor(attacker);
        MoveTowards.SetActor(attacker);

        var inBiteRange = false;
        var dist = Vector3.Distance(attacker.gameObject.transform.position, target.gameObject.transform.position);
        if (target.GetComponent<Character>() != null && dist < 1)
        {
            inBiteRange =  Utils.LineOfSight(attacker.gameObject, target.gameObject);
        }


        if (Bite.CheckValidTarget(target))
        {
            yield return BugBite(target);
        }
        else if (MoveTo.CheckValidTarget(target))
        {
            yield return MoveToTarget(target);
        }
        else if (inBiteRange)
        {
            yield return MoveTowardsTarget(target.transform.position);
        }
        while (CombatManager.Instance.inAction) yield return null;
    }

    public IEnumerator BugBite(Character target)
    {
        Bite.SetTarget(target);
        CombatManager.Instance.UseCombatAbility(Bite);
        //while (CombatManager.Instance.inAction) yield return null;
        yield break;
    }

    public IEnumerator MoveToTarget(Character target)
    {
        Debug.Log("Bug Move To");
        MoveTo.SetTarget(target);
        CombatManager.Instance.UseCombatAbility(MoveTo);
        //while (CombatManager.Instance.inAction) yield return null;
        yield break;
    }

    public IEnumerator MoveTowardsTarget(Vector3 targetPos)
    {
        MoveTowards.SetTarget(targetPos);
        CombatManager.Instance.UseCombatAbility(MoveTowards);
        //while (CombatManager.Instance.inAction) yield return null;
        yield break;
    }

    //private IEnumerator HandleNextTarget(NPC attacker, NPC target)
    //{
    //    var combatMover = attacker.GetComponent<MoveToClick>();
    //    if (Bite.CheckValidTarget(attacker, target)) { while (Bite.CheckValidTarget(attacker, target)) { Bite.UseAbility(attacker, target); } }
    //    else if (MoveTo.CheckValidTarget(attacker, target))
    //    {
    //        MoveTo.UseAbility(attacker, target);
    //        while (combatMover.IsMoving())
    //        {
    //            yield return null; // Wait for the next frame
    //        }
    //        while (Bite.CheckValidTarget(attacker, target)) { Bite.UseAbility(attacker, target); }
    //    }
    //    else
    //    {
    //        MoveTowards.UseAbility(attacker, target);
    //        while (combatMover.IsMoving())
    //        {
    //            yield return null; // Wait for the next frame
    //        }
    //    }
    //    //return null;

    //        //while (Bite.CheckValidTarget(attacker, target)) { Bite.UseAbility(attacker, target); }
    //}

    public override void AttackTarget(Character attacker)
    {

    }
}
