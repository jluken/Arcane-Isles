using System.Collections;
using UnityEngine;
using static CharStats;

public class AttackAction : InteractionAction
{
    public int attackCost;
    public int damageDie;
    public StatVal modifier;
    public bool precisionAttack;

    public AttackAction(int attackCost, int damageDie, StatVal modifier, bool precisionAttack = false, string name = "", Sprite icon = null, float range = 0f, Character actor = null, Selectable target = null) : base(name: name, icon: icon, range: range, actor: actor, target: target)
    {
        this.attackCost = attackCost;
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
        var damage = Random.Range(1, damageDie);

        CombatManager.Instance.LockAction(this);
        CombatManager.Instance.SpendActionPoints(attackCost); // account for floating point and wiggle room
        yield return new WaitForSeconds(1.0f);

        var diceRoll = Dice.RollDie(6) + Dice.RollDie(6);
        var hitCalc = (precisionAttack ? actor.charStats.GetCurrStat(StatVal.precision) : actor.charStats.GetCurrStat(StatVal.finesse)) + diceRoll - (victim.charStats.GetCurrStat(StatVal.finesse) + 6);
        bool crit = diceRoll == 12 || Dice.RollDie(12) <= hitCalc;
        bool hit = (precisionAttack ? actor.charStats.GetCurrStat(StatVal.precision) : actor.charStats.GetCurrStat(StatVal.finesse)) + diceRoll >= victim.charStats.GetCurrStat(StatVal.finesse) + 6;
        if(crit) victim.charStats.updateHealth(-1 * damage); // bypass armor
        else if (hitCalc >= 0) victim.takeDamage(damage);
        CombatManager.Instance.FinishAction();
        yield break;
    }

    public override int GetActionCost()
    {
        return attackCost;
    }

    public override void DisplayTarget()
    {
        if (CanUseAbility()) Cursor.SetCursor(CombatManager.Instance.attackCursor, Vector2.zero, CursorMode.Auto);
        else if (target != null) Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);  // TODO: create grey out attack cursor
        else Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
