using System;
using UnityEngine;

public class SwordPickup : SkillCheckItem
{
    //public GameObject player;
    //private CharStats playerStats;

    //private void Awake()
    //{
    //    playerStats = player.GetComponent<CharStats>();
    //}

    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //public override SkillCheckManager.Ability GetSkill()
    //{
    //    return SkillCheckManager.Ability.vigor;
    //}

    //public override int GetDC()
    //{
    //    return 6;
    //}

    //public override (string, int)[] GetModifiers()
    //{
    //    (string, int)[] mods = { 
    //        ("Vigor", playerStats.currVigor()),  // TODO: this along with GetSkil, and GetDC can be public fields in editor
    //        ("Just cuz", -1),
    //        ("skip", 0)
    //    };

    //    // TODO: get entries for attributes, skill modifiers, and special conditions
    //    return mods;
    //}

    //public override void FailCheck()
    //{
    //    Debug.Log("FAIL");  //TODO: if I want this to be damage, increase money, etc, I should make them Unity Events that call functions in inventory or wherever
    //}

    //public override void SucceedCheck()
    //{
    //    Debug.Log("SUCCEED");
    //}
}
