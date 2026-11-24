using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "CivieCombatBehavior", menuName = "Scriptable Objects/CivieCombatBehavior")]  // TODO: should this be a scriptable?
public class CivieCombatBehavior : BaseCombatBehavior  // TODO: this still keeps same behavior as bug and just runs up to nearest target
{
    public AbilityAction defaultPunch;
    public MoveToPoint defaultMove = new MoveToPoint("run", null); // TODO: make this (and combatManager version) point to same scriptable
    public InventoryData equipWeapon;

    private AbilityAction activeAction;

    //private AttackAction Bite = new AttackAction(attackCost: 6, damageDice: 1, modifier: -2, name: "bite", icon: null, range: 1.5f, animation: "bite");
    //private MoveToPoint MoveTo = new MoveToPoint(name: "bite", icon: null);
    //private MoveToPoint MoveTowards = new MoveToPoint("bite", null);

    public override Character ChooseTarget(Character attacker)
    {
        return FindClosestTarget(attacker);
    }

    public override bool CanAct(Character attacker)
    {
        if (CombatManager.Instance.ActionPoints == 0) return false;
        defaultPunch = attacker.defaultWeapon.DefaultAttack();
        if (activeAction == null) activeAction = defaultPunch; // TODO: maybe have inherited "setup" method that gets called

        Character target = ChooseTarget(attacker);
        if(target == null) return false;
        activeAction.SetActor(attacker);
        activeAction.SetTarget(target);
        return activeAction.CanUseAbility() || !activeAction.CheckValidAction();
    }

    public override IEnumerator DoNextAction(Character attacker)
    {
        if (equipWeapon != null && attacker.inventory.GetEquipment(EntityInventory.EquipmentInvType.holdMainHand) != equipWeapon)
        {
            attacker.inventory.SwapOutEquipment(equipWeapon);  // TODO: doesn't check if in inventory; might do smarter "check"
            activeAction = equipWeapon.DefaultAttack();
        }
        

        Character target = ChooseTarget(attacker);
        if (target == null) yield break;  
        SelectionController.Instance.Select(target);

        activeAction.SetActor(attacker);
        activeAction.SetTarget(target);
        defaultMove.SetActor(attacker);
        defaultMove.SetTarget(target);
        //MoveTowards.SetActor(attacker);


        if (activeAction.CanUseAbility())
        {
            yield return DefaultAttack(target);
        }
        else if (!activeAction.CheckValidAction() && defaultMove.CheckValidAction())  // Can be outside of AP and just get as close as possible
        {
            yield return MoveToTarget(target);
        }
        while (CombatManager.Instance.InAction()) yield return null;
    }

    public IEnumerator DefaultAttack(Character target)
    {
        activeAction.SetTarget(target);
        CombatManager.Instance.UseCombatAbility(-1, activeAction);
        //while (CombatManager.Instance.inAction) yield return null;
        yield break;
    }

    public IEnumerator MoveToTarget(Character target)
    {
        defaultMove.SetTarget(target);
        
        CombatManager.Instance.UseCombatAbility(-1, defaultMove);
        //while (CombatManager.Instance.inAction) yield return null;
        yield break;
    }

    public override void AttackTarget(Character attacker)
    {

    }
}
