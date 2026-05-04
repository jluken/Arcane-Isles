using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharStats))]
public class PlayerChar : PartyMember
{
    public static PlayerChar Instance;

    public override bool mainChar { get; } = true;

    public override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    public override void Die()
    {
        Debug.Log("Player die");
        StateMachine.ChangeState(DeadState);
        EventHandler.Instance.TriggerDeathEvent(this);
        //CombatManager.Instance.EndCombat();
        Debug.Log("bout to activate game over");
        UIController.Instance.ActivateGameOver();
    }

}
