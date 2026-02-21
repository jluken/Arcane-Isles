using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CharStats;

public class CharCreateMenu : MenuScreen
{
    public GameObject charCreateMenu;
    public static CharCreateMenu Instance;

    private bool active;

    private int remainingPoints = 5;

    private Dictionary<string, int> attrPoints = new Dictionary<string, int>()
    {
        { "vigor", 1},
        { "finesse", 1},
        { "psyche", 1}
    };

    public TMP_InputField nameField;
    public TMP_Dropdown genderField;

    public TMP_Text pointLeftTxt;
    public TMP_Text vigorTxt;
    public TMP_Text finesseTxt;
    public TMP_Text psycheTxt;

    //TODO: Vis - add character avatar creation (choose between M/F model and array of portraits)
    public void Awake()
    {
        Instance = this;
    }

    public override void ActivateMenu()
    {
        charCreateMenu.SetActive(true);
        active = true;
    }

    public override void DeactivateMenu()
    {
        charCreateMenu.SetActive(false);
        active = false;
    }

    public override bool IsActive()
    {
        return active;
    }

    private void UpdateText()
    {
        pointLeftTxt.text = remainingPoints.ToString();
        vigorTxt.text = attrPoints["vigor"].ToString();
        finesseTxt.text = attrPoints["finesse"].ToString();
        psycheTxt.text = attrPoints["psyche"].ToString();
    }

    public void IncreaseAttr(string attr)
    {
        if (!attrPoints.ContainsKey(attr)) Debug.LogError("Invalid attribute " + attr);
        if (remainingPoints < 1) return;
        remainingPoints -= 1;
        attrPoints[attr] += 1;
        UpdateText();
    }

    public void DecreaseAttr(string attr)
    {
        if (!attrPoints.ContainsKey(attr)) Debug.LogError("Invalid attribute " + attr);
        if (attrPoints[attr] <= 1) return;
        remainingPoints += 1;
        attrPoints[attr] -= 1;
        UpdateText();
    }

    public void AssignAttributes()
    {
        if (remainingPoints > 0) return;
        PartyController.Instance.playerChar.charStats.SetStat(StatVal.vigor, attrPoints["vigor"]);
        PartyController.Instance.playerChar.charStats.SetStat(StatVal.finesse, attrPoints["finesse"]);
        PartyController.Instance.playerChar.charStats.SetStat(StatVal.psyche, attrPoints["psyche"]);
        PartyController.Instance.playerChar.charStats.charName = nameField.text;
        string gender = genderField.options[genderField.value].text;
        if (gender == "M") {
            DialogueLua.SetVariable("PlayerThey", "he");
            DialogueLua.SetVariable("PlayerThem", "him");
            DialogueLua.SetVariable("PlayerTheir", "his");
            DialogueLua.SetVariable("PlayerTheirs", "his");
        }
        else if (gender == "F")
        {
            DialogueLua.SetVariable("PlayerThey", "she");
            DialogueLua.SetVariable("PlayerThem", "her");
            DialogueLua.SetVariable("PlayerTheir", "her");
            DialogueLua.SetVariable("PlayerTheirs", "hers");
        }
        else if (gender == "X")
        {
            DialogueLua.SetVariable("PlayerThey", "they");
            DialogueLua.SetVariable("PlayerThem", "them");
            DialogueLua.SetVariable("PlayerTheir", "their");
            DialogueLua.SetVariable("PlayerTheirs", "theirs");
        }
        PlayerChar.Instance.charStats.gender = gender;

        DeactivateMenu();
        // TODO: Demo: close this menu and immediately "level up"
    }
}
