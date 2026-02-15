using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static CharStats;

[RequireComponent(typeof(CharStats))]
public class Companion : PartyMember
{
    public override bool mainChar { get; } = false;

    public CompanionData companionData;

    public bool recruited;

    public IdleState UnrecruitedState;

    public override void Start()
    {
        base.Start();
        charStats.charName = companionData.charName;
        charStats.charImage = companionData.charImage;
        charStats.SetStat(StatVal.vigor, companionData.Vigor);
        charStats.SetStat(StatVal.finesse, companionData.Finesse);
        charStats.SetStat(StatVal.psyche, companionData.Psyche);
        SetSkills();
        // TODO: take char model
    }

    public override void SetStates()
    {
        base.SetStates();

        var unrecruitedActs = new List<SelectionData>() { talk, inspectSelection, recruit };
        UnrecruitedState = new UnrecruitedState(this, StateMachine, unrecruitedActs);

        StateMachine.Initialize(recruited ? IdleState : UnrecruitedState);
    }

    public void SetSkills()
    {
        charStats.ResetSkills();
        var playerLevel = PartyController.Instance.playerChar.charStats.GetCurrStat(StatVal.level);
        charStats.SetStat(StatVal.level, playerLevel);
        for (int i = 0; i < playerLevel; i++)
        {
            var stat = companionData.statList[i];
            Debug.Log("Build companion skill: " + stat);
            charStats.SetStat(stat, charStats.GetCurrStat(stat, false) + 1);
        }
    }

}

