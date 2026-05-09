using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AoeAttackAction : PointAction
{
    public int attackCost;
    public int damageDie;
    public float radius;

    public bool weaponAttack;

    public AoeAttackAction(int attackCost, int damageDie, float radius, bool weaponAttack, string name, Sprite icon, float range, Character actor = null, Vector3 point = new Vector3()) : base(name, icon, range: range, actor: actor, point: point)
    {
        this.attackCost = attackCost;
        this.damageDie = damageDie;
        this.radius = radius;
        this.weaponAttack = weaponAttack;
    }

    public override bool CheckValidTarget(Vector3 target)
    {
        Debug.Log("Check AOE valid");
        var dist = Vector3.Distance(actor.gameObject.transform.position, target);
        Debug.Log("dist: " + dist + " out of " + range);
        if (dist < range)
        {
            Debug.Log("Check line of sight " + Utils.LineOfSight(actor.gameObject, target));
            return Utils.LineOfSight(actor.gameObject, target) || Utils.LineOfSight(actor.gameObject, targetObject.gameObject);
        }
        return false;
    }

    public override IEnumerator UseAbility()
    {
        Collider[] hitColliders = Physics.OverlapSphere(target, radius);
        List<Character> victims = new();
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != null && hitCollider.gameObject.GetComponent<Character>() != null && Utils.LineOfSight(hitCollider.gameObject, target))
            {
                victims.Add(hitCollider.gameObject.GetComponent<Character>());
            }
        }
        var damage = Random.Range(1, damageDie);

        CombatManager.Instance.LockAction(this);
        CombatManager.Instance.SpendActionPoints(attackCost);
        if(weaponAttack) actor.inventory.UseWeapon();
        yield return new WaitForSeconds(1.0f);
        foreach(Character victim in victims) victim.takeDamage(damage);
        CombatManager.Instance.FinishAction();
        yield break;
    }

    public override int GetActionCost()
    {
        return attackCost;
    }

    public override void DisplayTarget()
    {
        CombatManager.Instance.abilityEffectMarker.SetActive(true);
        CombatManager.Instance.abilityEffectMarker.transform.position = target + new Vector3(0, 1, 0);
        CombatManager.Instance.abilityEffectMarker.GetComponent<DecalProjector>().size = new Vector3(radius * 2, radius * 2, 3f);
    }
}
