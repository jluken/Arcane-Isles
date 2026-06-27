using System.Collections;
using UnityEngine;

public class TestAction : AbilityAction
{

    public TestAction(Sprite icon, Character actor = null) : base(name: "test", actionCost: 1, icon: icon, range: -1, actor: actor)
    {
    }

    public override bool CheckValidAction()
    {
        return true;
    }

    public virtual bool CheckValidTarget(Selectable target)
    {
        return true;
    }

    public override IEnumerator UseAbility()
    {
        Debug.Log("Test action");
        CombatManager.Instance.SpendActionPoints(actionCost);
        CombatManager.Instance.FinishAction();
        yield break;
    }

    public override int GetActionCost()
    {
        return actionCost;
    }

    public override void SetTarget(Vector3 point)
    {
    }

    public override void SetTarget(Selectable newTarget)
    {
    }

    public override void DisplayTarget()
    {
    }
}
