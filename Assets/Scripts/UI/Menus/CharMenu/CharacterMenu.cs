using PixelCrushers.DialogueSystem.Articy.Articy_4_0;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
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
    private Dictionary<StatVal, int> statIncreases;
    public TMP_Text vigor;
    public TMP_Text finesse;
    public TMP_Text psyche;

    public Button levelUpButton;
    public TMP_Text skillPointText;
    public TMP_Text skillPoints;

    public Image portrait;
    public TMP_Text nameField;
    public TMP_Text gender;

    public TMP_Text level;
    public TMP_Text levelChange;
    public TMP_Text HP;
    public TMP_Text hpChange;
    public TMP_Text MP;
    public TMP_Text mpChange;
    public TMP_Text AP;
    public TMP_Text apChange;
    public TMP_Text speed;
    public TMP_Text speedChange;
    //TODO: add section for displaying "traits" (similar to perks) for things like weapon training or trade-offs - hovering (or always display) describes what they do

    public int availPoints;

    private Character currChar;

    public Sprite naSprite;
    public Sprite filledSprite;
    public Sprite tempFilledSprite;
    public Sprite emptySprite;
    public Sprite availableSprite;
    public Sprite extraAvailableSprite;
    public Sprite extraTempFilledSprite;
    public Sprite bonusSprite;
    public Sprite penaltySprite;

    public GameObject LevelWarning;

    public GameObject etchedSigils;
    public List<GameObject> etchedSigilObjs;
    public GameObject preparedSigils;
    public List<GameObject> preparedSigilObjs;
    public TMP_Text magickCost;
    public Button prepareSigils;

    public GameObject emptyButtonPrefab;

    //private CharStats charStats;

    private void Awake()
    {
        Instance = this;
        //etchedSigilObjs = new List<GameObject>();
        //preparedSigilObjs = new List<GameObject>();

        statIncreases = new Dictionary<StatVal, int>();
    }

    private void Start()
    {
        levelUpButton.onClick.AddListener(AttemptLevel);
    }

    public override void DeactivateMenu()
    {
        if(currChar != null) currChar.sigils.ClearExcessSigils();
        CharMenu.SetActive(false);
        UIActive = false;
    }

    public override void ActivateMenu()
    {
        UIActive = true;
        currChar = PartyController.Instance.selectedPartyMember;

        int xpLevel = PartyController.Instance.GetLevelByXP();
        bool levelUp = xpLevel > currChar.charStats.GetCurrStat(StatVal.level) && currChar == PartyController.Instance.playerChar;

        //UpdateStats();
        CharMenu.SetActive(true);
        Debug.Log("Set char menu");

        foreach (SkillBar s in skillBars) s.Populate(currChar, levelUp);
        foreach (StatVal skill in CharStats.Skills) statIncreases[skill] = 0;
        vigor.text = currChar.charStats.GetCurrStat(StatVal.vigor, false).ToString();
        finesse.text = currChar.charStats.GetCurrStat(StatVal.finesse, false).ToString();
        psyche.text = currChar.charStats.GetCurrStat(StatVal.psyche, false).ToString();

        portrait.sprite = currChar.charStats.charImage;
        nameField.text = currChar.charStats.charName;
        gender.text = currChar.charStats.gender;

        level.text = currChar.charStats.GetRawStat(StatVal.level).ToString();
        HP.text = currChar.charStats.GetRawStat(StatVal.maxHealth).ToString();
        MP.text = currChar.charStats.GetRawStat(StatVal.maxMagick).ToString();
        AP.text = currChar.charStats.GetRawStat(StatVal.actionPoints).ToString();
        speed.text = (Math.Truncate(currChar.charStats.runModifier *10) /10).ToString();

        levelChange.text = "";
        hpChange.text = "";
        mpChange.text = "";
        apChange.text = "";
        speedChange.text = "";

        levelUpButton.gameObject.SetActive(levelUp);
        levelUpButton.interactable = availPoints < (PartyController.Instance.GetLevelByXP() - currChar.charStats.GetCurrStat(StatVal.level));
        skillPointText.gameObject.SetActive(levelUp);
        skillPoints.gameObject.SetActive(levelUp);
        if (levelUp) {
            availPoints = (2 * xpLevel) - currChar.charStats.GetTotalSkillPoints(); // 2 skill points per level up, but skill bar won't let two on the same skill
            skillPoints.text = availPoints.ToString();
            PreviewStats();
        }

        DisplaySigils();
    }

    public void SpendPoints(StatVal skill, int amount = 1)
    {
        availPoints -= amount;
        if (amount > 0) statIncreases[skill] += 1;
        else statIncreases[skill] -= 1;
        skillPoints.text = availPoints.ToString();
        foreach (SkillBar s in skillBars) s.UpdateBoxes(currChar);
        PreviewStats();
    }

    public void PreviewStats()
    {
        var newLevel = PartyController.Instance.GetLevelByXP();
        // TODO: possibly refactor to avoid mismatch
        var newHp = 10 + newLevel * currChar.charStats.GetCurrStat(StatVal.vigor);
        var newMp = newLevel + 2 * (currChar.charStats.GetCurrStat(StatVal.arcana) + statIncreases[StatVal.arcana]);
        var newAp = 6 + currChar.charStats.GetCurrStat(StatVal.finesse);
        var newSpeed = (currChar.charStats.GetCurrStat(StatVal.athletics) + statIncreases[StatVal.athletics]) / 3.0f;

        if (newLevel > currChar.charStats.level) levelChange.text = "+" + (newLevel - currChar.charStats.level);
        else levelChange.text = "";
        if (newHp > currChar.charStats.GetRawStat(StatVal.maxHealth)) hpChange.text = "+" + (newHp - currChar.charStats.GetRawStat(StatVal.maxHealth));
        else hpChange.text = "";
        if (newMp > currChar.charStats.GetRawStat(StatVal.maxMagick)) mpChange.text = "+" + (newMp - currChar.charStats.GetRawStat(StatVal.maxMagick));
        else mpChange.text = "";
        if (newAp > currChar.charStats.GetRawStat(StatVal.actionPoints)) apChange.text = "+" + (newAp - currChar.charStats.GetRawStat(StatVal.actionPoints));
        else apChange.text = "";
        if (newSpeed > currChar.charStats.runModifier) speedChange.text = "+" + (Math.Truncate((newSpeed - currChar.charStats.runModifier) * 10) / 10);
        else speedChange.text = "";
    }

    public void AttemptLevel()
    {
        if (availPoints == 0) LevelUp();
        else LevelWarning.SetActive(true);
    }

    public void CloseWarning()
    {
        LevelWarning.SetActive(false);
    }

    public void LevelUp()
    {
        CloseWarning();
        int xpLevel = PartyController.Instance.GetLevelByXP();
        //int fullLevel = xpLevel - currChar.charStats.GetCurrStat(StatVal.level);
        //int gainedLevels = fullLevel - availPoints;
        //Debug.Log("gained levels: " + gainedLevels);
        currChar.charStats.SetStat(StatVal.level, xpLevel);
        foreach (SkillBar s in skillBars) { s.ApplyChanges(currChar); s.Populate(currChar); }
        currChar.charStats.setDerivedStats();
        ActivateMenu();
    }

    private void DisplaySigils()
    {
        Debug.Log("displaying sigils");
        foreach (var sigil in etchedSigilObjs)
        {
            sigil.GetComponent<Button>().onClick.RemoveAllListeners();
            sigil.SetActive(false);
        }
        //etchedSigilObjs.Clear();
        for(int i = 0; i < currChar.sigils.etchedSigils.Count; i++)
        {
            var charSigil = currChar.sigils.etchedSigils[0];
            //var nextActionButton = Instantiate(emptyButtonPrefab, etchedSigils.transform);
            //etchedSigilObjs.Add(nextActionButton);
            etchedSigilObjs[i].GetComponent<Image>().sprite = charSigil.sprite;
            etchedSigilObjs[i].GetComponent<Button>().onClick.AddListener(() => PrePrepSigil(charSigil));
            etchedSigilObjs[i].SetActive(true);
        }

        //foreach (var sigil in preparedSigilObjs) Destroy(sigil);
        //preparedSigilObjs.Clear();
        foreach (var sigil in preparedSigilObjs)
        {
            sigil.GetComponent<Button>().onClick.RemoveAllListeners();
            sigil.SetActive(false);
        }
        var lastSigilSlot = currChar.sigils.preparedSigils.Count > 0 ? currChar.sigils.preparedSigils.Keys.Max() : -1;
        for (int i = 0; i <= lastSigilSlot; i++)
        {
            Debug.Log("preparing sigil");
            //var nextActionButton = Instantiate(emptyButtonPrefab, preparedSigils.transform);
            //preparedSigilObjs.Add(nextActionButton);
            if(!currChar.sigils.preparedSigils.ContainsKey(i) || currChar.sigils.preparedSigils[i].Item1 == null) // empty slot
            {
                Debug.Log("empty sigil");
                preparedSigilObjs[i].GetComponent<Image>().sprite = null;
            }
            else if(currChar.sigils.preparedSigils[i].Item2 == false)  // preprepped sigil
            {
                Debug.Log("preprepped sigil");
                var sigil = currChar.sigils.preparedSigils[i].Item1;
                var buttonImage = preparedSigilObjs.Last().GetComponent<Image>();
                buttonImage.sprite = sigil.sprite;

                Color tempColor = buttonImage.color;
                tempColor.a = 0.5f;
                buttonImage.color = tempColor;
                int currIdx = i;
                preparedSigilObjs[i].GetComponent<Button>().onClick.AddListener(() => UnPrepSigil(currIdx));
                preparedSigilObjs[i].SetActive(true);
            }
            else  // prepped sigil
            {
                Debug.Log("prepped sigil");
                var sigil = currChar.sigils.preparedSigils[i].Item1;
                var buttonImage = preparedSigilObjs[i].GetComponent<Image>();
                buttonImage.sprite = sigil.sprite;
                preparedSigilObjs[i].GetComponent<Button>().onClick.AddListener(() => UnPrepSigil(i));
                preparedSigilObjs[i].SetActive(true);
                // TODO: maybe way to remove prepared sigils to clear up slots other than just spending them?
            }
        }

        magickCost.text = currChar.sigils.CostToPrepare() + "/" + currChar.charStats.GetCurrStat(StatVal.magick);
    }

    private void PrePrepSigil(Sigil sigil) // TODO: maybe replace these with events that update the display
    {
        if (currChar.sigils.PreprepSigil(sigil)) DisplaySigils();
    }

    private void UnPrepSigil(int i)
    {
        Debug.Log("unprep " + i);
        if (currChar.sigils.UnprepSigil(i)) DisplaySigils();
    }

    public void PrepSigils()
    {
        if (currChar.sigils.PrepareSigils()) DisplaySigils();
    }

    public override bool IsActive()
    {
        return UIActive;
    }
}
