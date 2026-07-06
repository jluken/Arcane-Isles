using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CharStats;

public class CharCreateMenu : MenuScreen
{
    public GameObject charCreateMenu;
    public static CharCreateMenu Instance;

    private bool active;

    public int remainingPoints { get; private set; } = 2;

    private Dictionary<StatVal, int> attrPoints = new Dictionary<StatVal, int>()
    {
        { StatVal.vigor, 3},
        { StatVal.finesse, 3},
        { StatVal.psyche, 3}
    };

    public TMP_InputField nameField;
    public TMP_Dropdown genderField;
    public Image portrait;

    public TMP_Text pointLeftTxt;
    public List<AbilityBar> abilityBars;

    public List<Material> colors;
    public List<GameObject> models;
    public List<Sprite> portraits;

    public TMP_Dropdown colorSelection;
    private Dictionary<string, Material> colorMap;
    public TMP_Dropdown modelSelection;
    private Dictionary<string, GameObject> modelMap;

    //public TMP_Text vigorTxt;
    //public TMP_Text finesseTxt;
    //public TMP_Text psycheTxt;

    public Button proceed;

    //TODO: Vis - add character avatar creation (choose between M/F model and array of portraits)
    public void Awake()
    {
        Instance = this;
        colorMap = new();
        modelMap = new();
    }

    public override void ActivateMenu()
    {
        charCreateMenu.SetActive(true);
        active = true;

        foreach (AbilityBar a in abilityBars) a.Populate(3);

        List<string> colorOptions = new();
        colorMap = new();
        for (int i = 0; i < colors.Count; i++)
        {
            var numString = (i + 1).ToString();
            colorOptions.Add(numString);
            colorMap.Add(numString, colors[i]);
        }
        colorSelection.AddOptions(colorOptions);
        colorSelection.RefreshShownValue();

        List<string> modelOptions = new();
        modelMap = new();
        for (int i = 0; i < models.Count; i++)
        {
            var numString = (i + 1).ToString();
            modelOptions.Add(numString);
            modelMap.Add(numString, models[i]);
        }
        modelSelection.AddOptions(modelOptions);
        modelSelection.RefreshShownValue();

        UpdateAppearance();
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

    //private void UpdateText()
    //{
    //    pointLeftTxt.text = remainingPoints.ToString();
    //    vigorTxt.text = attrPoints["vigor"].ToString();
    //    finesseTxt.text = attrPoints["finesse"].ToString();
    //    psycheTxt.text = attrPoints["psyche"].ToString();
    //}

    public void IncreaseAttr(StatVal attr)
    {
        if (!attrPoints.ContainsKey(attr)) Debug.LogError("Invalid attribute " + attr);
        if (remainingPoints < 1) return;
        remainingPoints -= 1;
        attrPoints[attr] += 1;
        pointLeftTxt.text = remainingPoints.ToString();
        proceed.interactable = remainingPoints > 0;
    }

    public void DecreaseAttr(StatVal attr)
    {
        if (!attrPoints.ContainsKey(attr)) Debug.LogError("Invalid attribute " + attr);
        if (attrPoints[attr] <= 1) return;
        remainingPoints += 1;
        attrPoints[attr] -= 1;
        pointLeftTxt.text = remainingPoints.ToString();
        proceed.interactable = remainingPoints > 0;
    }

    public void UpdateAppearance()
    {
        var selectedModel = models[modelSelection.value];
        var selectedMat = colors[colorSelection.value];
        var oldModel = PartyController.Instance.playerChar.renderBody;

        var newModel = Instantiate(selectedModel, PartyController.Instance.playerChar.animator.transform);
        newModel.GetComponent<SkinnedMeshRenderer>().bones = oldModel.GetComponent<SkinnedMeshRenderer>().bones;
        newModel.GetComponent<SkinnedMeshRenderer>().rootBone = oldModel.GetComponent<SkinnedMeshRenderer>().rootBone;
        newModel.GetComponent<SkinnedMeshRenderer>().material = selectedMat;
        Destroy(oldModel);
        newModel.SetActive(true);
        PartyController.Instance.playerChar.renderBody = newModel;
        UICharModel.Instance.SetChar(PartyController.Instance.playerChar.renderBody);

        int portraitIdx = (colors.Count * modelSelection.value) + colorSelection.value;
        portrait.sprite = portraits[portraitIdx];
    }

    public void AssignAttributes()
    {
        if (remainingPoints > 0) return;
        PartyController.Instance.playerChar.charStats.SetStat(StatVal.vigor, attrPoints[StatVal.vigor]);
        PartyController.Instance.playerChar.charStats.SetStat(StatVal.finesse, attrPoints[StatVal.finesse]);
        PartyController.Instance.playerChar.charStats.SetStat(StatVal.psyche, attrPoints[StatVal.psyche]);
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
        PlayerChar.Instance.charStats.gender = gender;  // TODO: maybe just create function to get pronouns instead of Lua variables

        DeactivateMenu();
        // TODO: Demo: close this menu and immediately "level up"
    }
}
