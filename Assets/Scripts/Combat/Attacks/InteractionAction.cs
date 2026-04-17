using System.Collections;
using UnityEngine;

public class InteractionAction : AbilityAction
{
    protected Selectable target;

    public InteractionAction(string name = "", Sprite icon = null, float range = 0f, NPC actor = null, Selectable target = null) : base(name: name, icon: icon, range: range, actor: actor)
    {
        if (target != null) SetTarget(target);
    }

    public override bool CheckValidAction()
    {
        return CheckValidTarget(target);
    }

    public virtual bool CheckValidTarget(Selectable target)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator UseAbility()
    {
        throw new System.NotImplementedException();
    }

    public override void SetTarget(Vector3 point)
    {
    }

    public override void SetTarget(Selectable newTarget)
    {
        target = newTarget;
    }
}
