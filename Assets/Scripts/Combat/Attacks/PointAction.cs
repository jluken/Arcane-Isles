using System.Collections;
using UnityEngine;

public class PointAction : AbilityAction
{
    protected Vector3 target;
    protected Selectable targetObject;

    public PointAction(string name, Sprite icon,  NPC actor, float range = 0f, Vector3 point = new Vector3()) : base(name: name, icon: icon, range: range, actor: actor)
    {
        SetTarget(point);
    }

    public PointAction(string name, Sprite icon) : base(name, icon) { }

    public override bool CheckValidAction()
    {
        return CheckValidTarget(target);
    }

    public virtual bool CheckValidTarget(Vector3 target)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator UseAbility()
    {
        throw new System.NotImplementedException();
    }

    public override void SetTarget(Vector3 point)
    {
        targetObject = null;
        target = point;
    }

    public override void SetTarget(Selectable newTarget)
    {
        targetObject = newTarget;
        target = newTarget.transform.position;
    }
}
