using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class MoveToObject : InteractionAction
{
    public MoveToObject(string name, Sprite icon, float range, Character actor = null) : base(name, icon, range, actor)
    {
    }

    protected float PathDist(Character actor, Selectable target)
    {
        var path = actor.mover.PathToObj(target);
        return MoveToClick.PathDist(path);
    }

    public override bool CheckValidAction()
    {
        return CheckValidTarget(target);
    }

    public override bool CheckValidTarget(Selectable target)
    {
        return !actor.mover.pathLocked;
    }

    public override IEnumerator UseAbility()
    {
        var cost = PathDist(actor, target);
        actor.mover.SetDestination(target);

        CombatManager.Instance.LockAction(this);
        while (actor.mover.IsMoving())
        {
            yield return null; // Wait for the next frame
        }
        //Debug.Log("Done moving moveToObj");
        CombatManager.Instance.FinishAction();  // handled by StopAction?
    }

    public override int GetActionCost()
    {
        Debug.Log("Move to obj path dist: " + PathDist(actor, target));
        return Mathf.CeilToInt(PathDist(actor, target) / actor.charStats.runModifier);
    }

    public override void DisplayTarget()
    {
        if (CheckValidAction()) actor.mover.DrawTo(target);
    }
}
