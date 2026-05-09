using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static CharStats;

public abstract class AbilityAction
{
    public string actionName;
    public Sprite icon;
    public float range;

    public Character actor;

    public AbilityAction(string name="", Sprite icon = null, float range=-1f, Character actor=null)
    {
        actionName = name;
        this.icon = icon;
        this.actor = actor;
        this.range = range;
    }

    public void SetActor(Character actor)
    {
        this.actor = actor;
    }

    public abstract void SetTarget(Vector3 point);

    public abstract void SetTarget(Selectable target);

    public abstract void DisplayTarget();

    public abstract bool CheckValidAction();

    public abstract IEnumerator UseAbility();

    public abstract int GetActionCost();

    public bool CanUseAbility()
    {
        return CheckValidAction() && GetActionCost() <= CombatManager.Instance.GetCurrentAP(actor);
    }

    public static bool RunningAction(AbilityAction action)
    {
        return action is MoveToPoint || action is MoveToObject;
    }
}
