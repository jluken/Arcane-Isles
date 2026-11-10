using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static CharStats;

public class SkillBar : MonoBehaviour
{
    public StatVal stat;
    public List<SkillBox> SkillBoxes;

    private int lastMarked;

    private bool levelUp;

    //private Dictionary<StatVal, bool> skillGrowthOpen = new Dictionary<StatVal, bool>();

    public void Populate(Character character, bool skillsAvailable = false, bool modifiers = false)
    {
        //Debug.Log("Populate skill bar " + stat);
        levelUp = skillsAvailable;
        for (int i = 0; i < SkillBoxes.Count; i++) { SkillBoxes[i].boxId = i; SkillBoxes[i].Populate(SkillBox.BoxState.na); }
        int currStat = character.charStats.GetRawStat(stat);
        for (int i = 0; i < currStat; i++) { SkillBoxes[i].Populate(SkillBox.BoxState.filled); }
        int currBoxes = character.charStats.GetStatBoxes(stat);
        for (int i = currStat; i < currBoxes; i++) { SkillBoxes[i].Populate(SkillBox.BoxState.empty); }

        lastMarked = currStat - 1;

        if (skillsAvailable)
        {
            if (currStat < currBoxes) SkillBoxes[currStat].Populate(SkillBox.BoxState.available);
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

    public virtual void UpdateBoxes(Character character)
    {
        int currStatVal = character.charStats.GetRawStat(stat);

        if (levelUp && CharacterMenu.Instance.availPoints > 0)
        {
            if (SkillBoxes[currStatVal].boxState == SkillBox.BoxState.na) SkillBoxes[currStatVal].Populate(SkillBox.BoxState.newBoxAvailable);
            else if (SkillBoxes[currStatVal].boxState == SkillBox.BoxState.empty) SkillBoxes[currStatVal].Populate(SkillBox.BoxState.available);
        }
        else if (levelUp)
        {
            if (SkillBoxes[currStatVal].boxState == SkillBox.BoxState.newBoxAvailable) SkillBoxes[currStatVal].Populate(SkillBox.BoxState.na);
            else if (SkillBoxes[currStatVal].boxState == SkillBox.BoxState.available) SkillBoxes[currStatVal].Populate(SkillBox.BoxState.empty);
        }
    }

    public virtual void BoxClicked(int boxId)
    {
        if (boxId != lastMarked && !(boxId == (lastMarked + 1) && CharacterMenu.Instance.availPoints > 0)) return;
        bool anotherBox = boxId < MaxSkillVal;
        //if (boxId != lastMarked && boxId != lastMarked + 1) return;
        int spendPoints = SkillBoxes[boxId].SetClickedBox(CharacterMenu.Instance.availPoints > 0);
        bool relinquishedBox = SkillBoxes[boxId].boxState == SkillBox.BoxState.available || SkillBoxes[boxId].boxState == SkillBox.BoxState.newBoxAvailable;
        lastMarked = relinquishedBox ? boxId - 1 : boxId;
        CharacterMenu.Instance.SpendPoints(stat, spendPoints);
        bool stillWorkingOnBox = SkillBoxes[boxId].boxState == SkillBox.BoxState.newBoxSet;
        if (anotherBox && CharacterMenu.Instance.availPoints > 0 && !stillWorkingOnBox) SkillBoxes[boxId + 1].SetNextBox(relinquishedBox);



        //if (SkillBoxes[boxId].boxState == SkillBox.BoxState.available)  // select new valid skill point
        //{
        //    storedRankUp++;
        //    SkillBoxes[boxId].Populate(SkillBox.BoxState.tempFilled);
        //    if(anotherBox) SkillBoxes[boxId + 1].

        //    CharacterMenu.Instance.SpendPoints(stat);
        //}
        //else if (SkillBoxes[boxId].boxState == SkillBox.BoxState.tempFilled) // Undo skill point selection
        //{
        //    storedRankUp--;
        //    SkillBoxes[boxId].Populate(SkillBox.BoxState.available);
        //    CharacterMenu.Instance.SpendPoints(stat, -1);
        //}
        //else if (CharacterMenu.Instance.availPoints > 1 && SkillBoxes[boxId].boxState == SkillBox.BoxState.newBoxAvailable)  // Select extension point
        //{
        //    storedRankUp++;
        //    SkillBoxes[boxId].Populate(SkillBox.BoxState.newBoxSet);
        //    CharacterMenu.Instance.SpendPoints(stat, 2);
        //}
        //else if (SkillBoxes[boxId].boxState == SkillBox.BoxState.newBoxSet)  // Undo extension point
        //{
        //    storedRankUp--;
        //    SkillBoxes[boxId].Populate(SkillBox.BoxState.newBoxAvailable);
        //    //if (anotherBox) SkillBoxes[boxId + 1].OnDeck();
        //    CharacterMenu.Instance.SpendPoints(stat, -2);
        //}
    }

    public int NewPoints()
    {
        SkillBox.BoxState[] newSkillPoints = { SkillBox.BoxState.tempFilled, SkillBox.BoxState.newBoxFilled };
        return SkillBoxes.Where(box => newSkillPoints.Contains(box.boxState)).Count();
    }

    public virtual int ApplyChanges(Character character)
    {
        // Returns the number of points added
        SkillBox.BoxState[] newSkillBoxes = { SkillBox.BoxState.newBoxSet, SkillBox.BoxState.newBoxFilled };

        int newPoints = NewPoints();
        int newBoxes = SkillBoxes.Where(box => newSkillBoxes.Contains(box.boxState)).Count();

        character.charStats.SetStat(stat, character.charStats.GetRawStat(stat) + newPoints);
        character.charStats.SetStatBoxes(stat, character.charStats.GetStatBoxes(stat) + newBoxes);
        //storedRankUp = 0;

        Populate(character);
        return newPoints;
    }

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
