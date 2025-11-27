using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SkillCheck", menuName = "Scriptable Objects/SkillCheck")]
public class SkillCheck : ScriptableObject
{
    public int dc;

    public CharStats.StatVal skill;
    public List<string> modifiers;

    private CharStats playerStats;
    private SkillCheckManager skillCheckManager;

    public void CheckSkill()
    {
        skillCheckManager = GameObject.Find("SkillCheckCanvas").GetComponent<SkillCheckManager>(); // TODO: put skill check into the UI manager (and right click select)
        playerStats = PartyController.Instance.leader.charStats;

        
        (string, int)[] abilitySkillScores = { (skill.ToString(), playerStats.GetCurrStat(skill)) };
        skillCheckManager.ActivateSkillCheck(skill, dc, abilitySkillScores.Concat(GetModifiers(modifiers)).ToArray());
    }


    public virtual (string, int)[] GetModifiers(List<string> modifierNames)
    {
        // TODO: get entries for special modifier conditions held in a flag system somewhere
        return new (string, int)[0];
    }
}
