using System;
using System.Collections;
using UnityEngine;
using static CharStats;

public class HealAction : InteractionAction
{
    public int healthSlots;
    public StatVal modifier;

    public HealAction(int healCost, int slots, StatVal modifier, string name = "", Sprite icon = null, float range = 0f, Character actor = null, Selectable target = null, string animation = null) : base(name: name, actionCost: healCost, icon: icon, range: range, actor: actor, target: target, animation: animation)
    {
        this.healthSlots = slots;
        this.modifier = modifier;
    }

    public override bool CheckValidTarget(Selectable target)
    {
        if (target == null || CombatManager.Instance.combatActive) return false;  // TODO: look at greying out invalid actions in bar
        var dist = Vector3.Distance(actor.gameObject.transform.position, target.gameObject.transform.position);
        if (target.GetComponent<Character>() != null && dist < range)
        {
            return Utils.LineOfSight(actor.gameObject, target.gameObject);
        }
        return false;
    }

    public override IEnumerator UseAbility()  // TODO: have better ones consume magick?
    {
        var victim = target.GetComponent<Character>();
        var healthPool = actor.charStats.GetCurrStat(StatVal.physick);
        var totalHealing = Dice.RollDicePool(healthPool, healthSlots, 6);

        if (actor.animator != null) actor.animator.CrossFade(animation, 0.25f);

        CombatManager.Instance.LockAction(this);
        CombatManager.Instance.SpendActionPoints(actionCost);
        yield return new WaitForSeconds(1.0f); // TODO: first aid timeskip?

        victim.charStats.updateHealth(totalHealing);
        CombatManager.Instance.FinishAction();
        yield break;
    }

    public override int GetActionCost()
    {
        return actionCost;
    }

    public override void DisplayTarget()
    {
        if (CanUseAbility()) Cursor.SetCursor(CombatManager.Instance.attackCursor, Vector2.zero, CursorMode.Auto); // TODO: healing cursor
        else if (target != null) Cursor.SetCursor(CombatManager.Instance.attackCursorNull, Vector2.zero, CursorMode.Auto);
        else Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
