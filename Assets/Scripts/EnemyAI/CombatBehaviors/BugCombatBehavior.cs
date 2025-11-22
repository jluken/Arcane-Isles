using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

[CreateAssetMenu(fileName = "BugCombatBehavior", menuName = "Scriptable Objects/BugCombatBehavior")]
public class BugCombatBehavior : BaseCombatBehavior
{
    public AbilityAction Bite;
    public AbilityAction MoveTo;
    public AbilityAction MoveTowards;

    public override NPC ChooseTarget(NPC attacker)  // TODO: maybe just have one "root" method that gets called and then each enemy type script could have its own logic? Maybe have some shared methods as part of that, like finding nearest enemy.
    {
        return FindClosestTarget(attacker);
    }

    public override IEnumerator CombatTurn(NPC attacker)
    {
        // TODO: will happen in loop until AP is exhausted
        Debug.Log("Bug Combat");
        //int ap = attacker.charStats.finesse + 10;  // TODO: build-in AP calculation to charstats (and maybe speed?)
        NPC target = null;
        var combatMover = attacker.GetComponent<MoveToClick>();

        while (target == null)  // TODO: handle better when death is settled
        {
            Debug.Log("new target");
            target = ChooseTarget(attacker);
            target.Select();
            //yield return StartCoroutine(HandleNextTarget(attacker, target));

            if (Bite.CheckValidTarget(attacker, target)) { Debug.Log("Bite Now"); while (Bite.CheckValidTarget(attacker, target)) { CombatManager.Instance.SpendActionPoints(Bite.UseAbility(attacker, target)); } }
            else if (MoveTo.CheckValidTarget(attacker, target))
            {
                Debug.Log("Move to");
                CombatManager.Instance.SpendActionPoints(MoveTo.UseAbility(attacker, target));
                
                while (combatMover.IsMoving())
                {
                    yield return null; // Wait for the next frame
                }
                Debug.Log("Done moving");
                while (Bite.CheckValidTarget(attacker, target)) {
                    Debug.Log("Bite");
                    Debug.Log(CombatManager.Instance.ActionPoints);
                    CombatManager.Instance.SpendActionPoints(Bite.UseAbility(attacker, target)); 
                }
            }
            else
            {
                Debug.Log("Move towards");
                CombatManager.Instance.SpendActionPoints(MoveTowards.UseAbility(attacker, target));
                while (combatMover.IsMoving())
                {
                    yield return null; // Wait for the next frame
                }
            }
        }

        //    var fullPath = combatMover.PathToPoint(target.gameObject.transform.position);
        //float dist = Mathf.Min(10, MoveToClick.PathDist(fullPath) - 1); //TODO: will need to handle max distance and unit conversion in AP stage
        //var dest = MoveToClick.PointAlongPath(fullPath, dist);
        //combatMover.SetDestination(dest);
        //while (combatMover.IsMoving())
        //{
        //    yield return null; // Wait for the next frame
        //}
        //Debug.Log("Ready to fight");
        //if (Bite.CheckValidTarget(attacker, target)) { Bite.UseAbility(attacker, target); };
        CombatManager.Instance.NextTurn();
        //yield return null;
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
