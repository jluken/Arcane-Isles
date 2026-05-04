using System.Collections.Generic;
using UnityEngine;

public class CharState
{
    protected Character character;
    protected CharStateMachine charStateMachine;
    protected List<SelectionData> actions;

    public CharState(Character character, CharStateMachine npcStateMachine, List<SelectionData> actions)
    {
        //Debug.Log("Define State: " + npc.charStats.charName + " " + this);
        //Debug.Log("actions: ");
        //foreach (var action in actions) {
        //    Debug.Log(action);
        //}
        this.character = character;
        this.charStateMachine = npcStateMachine;
        this.actions = actions;
    }

    public string stateName => GetType().Name;

    public virtual bool isActive => false;

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void FrameUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void SetActiveChar() { }

    public virtual void SetIdle() { }

    public virtual void MoveTo(Vector3 destination) { }

    public virtual void EnterCombat() {
        Debug.Log(character.charStats.charName + "entering combat");
        charStateMachine.ChangeState(character.IdleCombatState);
        character.mover.StopMoving();
        CombatManager.Instance.insertIntoInitiative(character, character.combatantType);
    }

    public virtual void EndCombat()
    {
        Debug.Log(character.charStats.charName + "ending combat");
        charStateMachine.ChangeState(character.IdleState);
    }

    public List<SelectionData> GetActions()
    {
        return actions;
    }

    public virtual void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) { }

}
