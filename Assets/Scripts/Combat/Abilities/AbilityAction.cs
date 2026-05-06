using System.Collections;
using UnityEngine;
using static CharStats;

public abstract class AbilityAction
{
    public string actionName;
    public Sprite icon;
    public float range;

    public Character actor;

    public AbilityAction(string name="", Sprite icon = null, float range=0f, Character actor=null)
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

    public abstract bool CheckValidAction();

    public abstract IEnumerator UseAbility();
}
