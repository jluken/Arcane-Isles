using System.Collections;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "BugCombatBehavior", menuName = "Scriptable Objects/BugCombatBehavior")]
public class BugCombatBehavior : BaseCombatBehavior
{
    public AbilityAction Bite;
    public AbilityAction MoveTo;
    public AbilityAction MoveTowards;

    public override NPC ChooseTarget(NPC attacker)
    {
        return FindClosestTarget(attacker);
    }

    public override IEnumerator CombatTurn(NPC attacker)
    {
        Debug.Log("Bug Combat, AP: " + CombatManager.Instance.ActionPoints);
        NPC target = null;
        var combatMover = attacker.mover;

        while (target == null)  // TODO: handle better when death is settled
        {
            Debug.Log("new target");
            target = ChooseTarget(attacker);
            if (target == null) break;  // TODO: handle better when death is settled
            target.Select();
            //yield return StartCoroutine(HandleNextTarget(attacker, target));

            if (Bite.CheckValidTarget(attacker, target)) { 
                Debug.Log("Bite Now"); 
                while (Bite.CheckValidTarget(attacker, target)) { 
                    //Bite.UseAbility(attacker, target);
                    CombatManager.Instance.SetCurrentAction(Bite);
                    CombatManager.Instance.UseCombatAbility(target, CombatManager.CombatActionType.Attack);
                    while (CombatManager.Instance.inAction) yield return null;
                } 
            }
            else if (MoveTo.CheckValidTarget(attacker, target))
            {
                Debug.Log("Move to, AP: " + CombatManager.Instance.ActionPoints);
                CombatManager.Instance.SetCurrentAction(MoveTo);
                CombatManager.Instance.UseCombatAbility(target, CombatManager.CombatActionType.Run);
                //yield return StartCoroutine(MoveTo.UseAbility(attacker, target));
                int ctr = 0;
                while (CombatManager.Instance.inAction && ctr < 100)//CombatManager.Instance.inAction)
                {
                    //ctr++;
                    //Debug.Log("waiting...");
                    yield return null;
                }
                Debug.Log("Done moving, AP left: " + CombatManager.Instance.ActionPoints);
                while (Bite.CheckValidTarget(attacker, target)) {
                    Debug.Log("Bite");
                    Debug.Log(CombatManager.Instance.ActionPoints);
                    //Bite.UseAbility(attacker, target);
                    CombatManager.Instance.SetCurrentAction(Bite);
                    CombatManager.Instance.UseCombatAbility(target, CombatManager.CombatActionType.Attack);
                    while (CombatManager.Instance.inAction) yield return null;
                }
            }
            else
            {
                Debug.Log("Move towards, AP: " + CombatManager.Instance.ActionPoints);
                //MoveTowards.UseAbility(attacker, target);
                CombatManager.Instance.SetCurrentAction(MoveTowards);
                CombatManager.Instance.UseCombatAbility(target, CombatManager.CombatActionType.Run);
                while (CombatManager.Instance.inAction) yield return null;
            }
        }
        CombatManager.Instance.NextTurn();
    }

    private IEnumerator HandleNextTarget(NPC attacker, NPC target)
    {
        var combatMover = attacker.GetComponent<MoveToClick>();
        if (Bite.CheckValidTarget(attacker, target)) { while (Bite.CheckValidTarget(attacker, target)) { Bite.UseAbility(attacker, target); } }
        else if (MoveTo.CheckValidTarget(attacker, target))
        {
            MoveTo.UseAbility(attacker, target);
            while (combatMover.IsMoving())
            {
                yield return null; // Wait for the next frame
            }
            while (Bite.CheckValidTarget(attacker, target)) { Bite.UseAbility(attacker, target); }
        }
        else
        {
            MoveTowards.UseAbility(attacker, target);
            while (combatMover.IsMoving())
            {
                yield return null; // Wait for the next frame
            }
        }
        //return null;

            //while (Bite.CheckValidTarget(attacker, target)) { Bite.UseAbility(attacker, target); }
    }

    public override void AttackTarget(NPC attacker)
    {

    }
}
