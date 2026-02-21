using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CharStats;

public class CharacterMenu : MenuScreen
{
    public static CharacterMenu Instance { get; private set; }
    //public GameObject TextMenu;
    public bool UIActive;

    public GameObject CharMenu;
    public List<SkillBar> skillBars;
    public TMP_Text vigor;
    public TMP_Text finesse;
    public TMP_Text psyche;

    public Button levelUpButton;
    public TMP_Text skillPointText;
    public TMP_Text skillPoints;

    public Image portrait;
    public TMP_Text nameField;
    public TMP_Text gender;

    public int availPoints;

    private NPC currChar;


    //private CharStats charStats;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        levelUpButton.onClick.AddListener(LevelUp);
    }

    public override void DeactivateMenu()
    {
        Debug.Log("Deactivate char menu");
        CharMenu.SetActive(false);
        UIActive = false;
    }

    public override void ActivateMenu()
    {
        UIActive = true;
        currChar = PartyController.Instance.leader;

        int xpLevel = PartyController.Instance.GetLevelByXP();
        bool levelUp = xpLevel > currChar.charStats.GetCurrStat(StatVal.level) && currChar == PartyController.Instance.playerChar;

        //UpdateStats();
        CharMenu.SetActive(true);
        Debug.Log("Set char menu");

        foreach (SkillBar s in skillBars) s.Populate(currChar, levelUp);
        vigor.text = currChar.charStats.GetCurrStat(StatVal.vigor, false).ToString();
        finesse.text = currChar.charStats.GetCurrStat(StatVal.finesse, false).ToString();
        psyche.text = currChar.charStats.GetCurrStat(StatVal.psyche, false).ToString();

        portrait.sprite = currChar.charStats.charImage;
        nameField.text = currChar.charStats.charName;
        gender.text = currChar.charStats.gender;

        levelUpButton.gameObject.SetActive(levelUp);
        levelUpButton.interactable = availPoints < (PartyController.Instance.GetLevelByXP() - currChar.charStats.GetCurrStat(StatVal.level));
        skillPointText.gameObject.SetActive(levelUp);
        skillPoints.gameObject.SetActive(levelUp);
        if (levelUp) {
            availPoints = xpLevel - currChar.charStats.GetCurrStat(StatVal.level);
            skillPoints.text = availPoints.ToString();
        }
        //TextMenu.SetActive(true);


    }

    public void SpendPoints(int amount = 1)
    {
        availPoints -= amount;
        skillPoints.text = availPoints.ToString();
    }

    public void LevelUp()
    {
        int xpLevel = PartyController.Instance.GetLevelByXP();
        int fullLevel = xpLevel - currChar.charStats.GetCurrStat(StatVal.level);
        int gainedLevels = fullLevel - availPoints;
        Debug.Log("gained levels: " + gainedLevels);
        currChar.charStats.SetStat(StatVal.level, currChar.charStats.GetCurrStat(StatVal.level) + gainedLevels);
        foreach (SkillBar s in skillBars) { s.ApplyChanges(currChar); s.Populate(currChar); }
    }

    public override bool IsActive()
    {
        return UIActive;
    }
}
