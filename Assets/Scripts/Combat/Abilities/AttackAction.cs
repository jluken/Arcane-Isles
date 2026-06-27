using System;
using System.Collections;
using UnityEngine;
using static CharStats;

public class AttackAction : InteractionAction
{
    public int damageDie;
    public StatVal modifier;
    public bool precisionAttack;

    public AttackAction(int attackCost, int damageDie, StatVal modifier, bool precisionAttack = false, string name = "", Sprite icon = null, float range = 0f, Character actor = null, Selectable target = null) : base(name: name, actionCost: attackCost, icon: icon, range: range, actor: actor, target: target)
    {
        this.damageDie = damageDie;
        this.modifier = modifier;
        this.precisionAttack = precisionAttack;
    }

    public override bool CheckValidTarget(Selectable target)
    {
        if (target == null) return false;
        var dist = Vector3.Distance(actor.gameObject.transform.position, target.gameObject.transform.position);
        if (target.GetComponent<Character>() != null && dist < range)
        {
            return Utils.LineOfSight(actor.gameObject, target.gameObject);
        }
        return false;
    }

    public override IEnumerator UseAbility()
    {
        var victim = target.GetComponent<Character>();
        var damage = Dice.RollDie(damageDie);

        CombatManager.Instance.LockAction(this);
        CombatManager.Instance.SpendActionPoints(actionCost); // account for floating point and wiggle room
        yield return new WaitForSeconds(1.0f);

        var diceRoll = Dice.RollDie(6) + Dice.RollDie(6);
        var hitCalc = (precisionAttack ? actor.charStats.GetCurrStat(StatVal.precision) : actor.charStats.GetCurrStat(StatVal.finesse)) + diceRoll - (victim.charStats.GetCurrStat(StatVal.finesse) + 6);
        bool crit = diceRoll == 12 || Dice.RollDie(12) <= hitCalc;
        if (crit)
        {
            damage = (int)Math.Floor(damage * 1.5f);
            victim.charStats.updateHealth(-1 * damage); // bypass armor
            DialogueInterface.Instance.LogLine(actor.charStats.charName + " critically hits " + victim.charStats.charName + " for " + damage);
        }
        else if (hitCalc >= 0) {
            victim.takeDamage(damage);
            DialogueInterface.Instance.LogLine(actor.charStats.charName + " hits " + victim.charStats.charName + " for " + damage);
        }
        else DialogueInterface.Instance.LogLine(actor.charStats.charName + " misses.");
        CombatManager.Instance.FinishAction();
        yield break;
    }

    public override int GetActionCost()
    {
        return actionCost;
    }

    public override void DisplayTarget()
    {
        if (CanUseAbility()) Cursor.SetCursor(CombatManager.Instance.attackCursor, Vector2.zero, CursorMode.Auto);
        else if (target != null) Cursor.SetCursor(CombatManager.Instance.attackCursorNull, Vector2.zero, CursorMode.Auto);
        else Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
