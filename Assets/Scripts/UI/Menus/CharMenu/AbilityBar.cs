using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static CharStats;

public class AbilityBar : SkillBar
{
    public StatVal ability;

    private int storedAbilityVal;

    public void Populate(int startVal)
    {
        storedAbilityVal = startVal;
        for (int i = 0; i < SkillBoxes.Count; i++) { SkillBoxes[i].boxId = i; SkillBoxes[i].Populate(SkillBox.BoxState.empty); }
        for (int i = 0; i < startVal; i++) { SkillBoxes[i].Populate(SkillBox.BoxState.filled); }
    }

    //public void UpdateBoxes(Character character)
    //{
    //    int currStatVal = character.charStats.GetRawStat(ability);

    //    if (levelUp && CharacterMenu.Instance.availPoints > 0)
    //    {
    //        if (SkillBoxes[currStatVal].boxState == SkillBox.BoxState.na) SkillBoxes[currStatVal].Populate(SkillBox.BoxState.newBoxAvailable);
    //        else if (SkillBoxes[currStatVal].boxState == SkillBox.BoxState.empty) SkillBoxes[currStatVal].Populate(SkillBox.BoxState.available);
    //    }
    //    else if (levelUp)
    //    {
    //        if (SkillBoxes[currStatVal].boxState == SkillBox.BoxState.newBoxAvailable) SkillBoxes[currStatVal].Populate(SkillBox.BoxState.na);
    //        else if (SkillBoxes[currStatVal].boxState == SkillBox.BoxState.available) SkillBoxes[currStatVal].Populate(SkillBox.BoxState.empty);
    //    }
    //}

    public override void BoxClicked(int boxId)
    {
        bool anotherBox = boxId < CharStats.MaxSkillVal;
        if (boxId != storedAbilityVal && boxId != (storedAbilityVal - 1)) return;
        //if (boxId != lastMarked && boxId != lastMarked + 1) return;

        if (CharCreateMenu.Instance.remainingPoints > 0 && SkillBoxes[boxId].boxState == SkillBox.BoxState.empty)
        {
            storedAbilityVal++;
            SkillBoxes[boxId].Populate(SkillBox.BoxState.filled);
            CharCreateMenu.Instance.IncreaseAttr(ability);
        }
        else if (SkillBoxes[boxId].boxState == SkillBox.BoxState.filled) // Undo skill point selection
        {
            storedAbilityVal--;
            SkillBoxes[boxId].Populate(SkillBox.BoxState.empty);
            CharCreateMenu.Instance.DecreaseAttr(ability);
        }
    }

    //public void ApplyChanges(Character character)
    //{
    //    character.charStats.SetStat(ability, character.charStats.GetRawStat(ability) + storedAbilityVal);
    //    storedAbilityVal = 0;
    //}

    //private void ChangeSkillGrowth(StatVal stat, bool open)
    //{
    //    if (!CharStats.IsSkill(stat)) Debug.LogError(stat + " is not a skill");
    //    if (open) skillGrowthOpen[stat] = true;
    //    else skillGrowthOpen[stat] = false;
    //}

    //public bool CheckOpenSkillGrowth(StatVal stat)
    //{
    //    if (skillGrowthOpen.ContainsKey(stat) && skillGrowthOpen[stat]) return true;
    //    return false;
    //}

}
