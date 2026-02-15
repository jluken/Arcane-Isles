using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour
{
    public CharStats.StatVal stat;
    public List<SkillBox> SkillBoxes;

    private int storedRankUp;
    private bool openSkill;
    private int lastMarked;

    public void Populate(NPC character, bool skillsAvailable = false, bool modifiers = false)
    {
        //Debug.Log("Populate skill bar " + stat);
        storedRankUp = 0;
        openSkill = false;
        for (int i = 0; i < SkillBoxes.Count; i++) { SkillBoxes[i].boxId = i; SkillBoxes[i].Populate(SkillBox.BoxState.na); }
        int currStat = character.charStats.GetRawStat(stat);
        for (int i = 0; i < currStat; i++) { SkillBoxes[i].Populate(SkillBox.BoxState.filled); }
        int currAbility = character.charStats.GetCurrStat(CharStats.GetSkillAbility(stat), false);
        for (int i = currStat; i < currAbility; i++) { SkillBoxes[i].Populate(SkillBox.BoxState.empty); }

        lastMarked = currStat - 1;

        if (skillsAvailable)
        {
            if (currStat < currAbility) SkillBoxes[currStat].Populate(SkillBox.BoxState.available);
            else if (currStat < CharStats.MaxSkillVal) SkillBoxes[currStat].Populate(SkillBox.BoxState.newBoxAvailable);
        }
        if (modifiers)
        {
            int moddedStat = character.charStats.currStatMods(stat);
            if (moddedStat > currStat) {
                for (int i = currStat; i < moddedStat; i++) SkillBoxes[i].Populate(SkillBox.BoxState.bonus);
            }
            else if (moddedStat < currStat)
            {
                for (int i = moddedStat; i < currStat; i++) SkillBoxes[i].Populate(SkillBox.BoxState.penalty);
            }
        }
    }

    public void BoxClicked(int boxId)
    {
        bool anotherBox = boxId < CharStats.MaxSkillVal;
        if (boxId != lastMarked && boxId != lastMarked + 1) return;
        {
            
        }
        if (CharacterMenu.Instance.availPoints > 0 && SkillBoxes[boxId].boxState == SkillBox.BoxState.available)
        {
            storedRankUp++;
            SkillBoxes[boxId].Populate(SkillBox.BoxState.tempFilled);
            if (anotherBox) SkillBoxes[boxId + 1].OnDeck();
            CharacterMenu.Instance.SpendPoints();
            lastMarked = boxId;
        }
        else if (SkillBoxes[boxId].boxState == SkillBox.BoxState.tempFilled)
        {
            storedRankUp--;
            SkillBoxes[boxId].Populate(SkillBox.BoxState.available);  // TODO: what if from tempavailable?
            if (anotherBox) SkillBoxes[boxId + 1].OffDeck();
            CharacterMenu.Instance.SpendPoints(-1);
            lastMarked--;
        }
        else if (CharacterMenu.Instance.availPoints > 0 && SkillBoxes[boxId].boxState == SkillBox.BoxState.newBoxAvailable)
        {
            openSkill = true;
            SkillBoxes[boxId].Populate(SkillBox.BoxState.tempAvailable);
            if (anotherBox) SkillBoxes[boxId + 1].OnDeck();
            CharacterMenu.Instance.SpendPoints();
            lastMarked = boxId;
        }
        else if (CharacterMenu.Instance.availPoints > 0 && SkillBoxes[boxId].boxState == SkillBox.BoxState.tempAvailable)  // TODO: how do I "undo" a temporarily opened square? How to handle openSkill?
        {
            openSkill = false;
            SkillBoxes[boxId].Populate(SkillBox.BoxState.tempFilled);
            if (anotherBox) SkillBoxes[boxId + 1].OnDeck();
            CharacterMenu.Instance.SpendPoints();
        }
    }

    public void ApplyChanges(NPC character)
    {
        character.charStats.SetStat(stat, character.charStats.GetRawStat(stat) + storedRankUp);
        if (openSkill) character.charStats.ChangeSkillGrowth(stat, true);
        storedRankUp = 0;
        openSkill = false;
    }
}
