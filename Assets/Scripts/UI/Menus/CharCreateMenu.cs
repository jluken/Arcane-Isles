using PixelCrushers.DialogueSystem;
using System;
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

    public int remainingPoints { get; private set; } = 7;

    private Dictionary<StatVal, int> attrPoints = new Dictionary<StatVal, int>()
    {
        { StatVal.vigor, 1},
        { StatVal.finesse, 1},
        { StatVal.psyche, 1}
    };

    public int age;
    public TMP_Text ageTxt;
    public Button ageUp;
    public Button ageDown;
    public int maxAge = 30;
    public int minAge = 18;

    public TMP_InputField firstNameField;
    public TMP_InputField lastNameField;
    public TMP_Dropdown genderField;
    public Image portrait;

    public TMP_Text pointLeftTxt;
    public List<AbilityBar> abilityBars;

    public List<Material> colors;
    public List<GameObject> models;
    public List<Mesh> meshes;
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

    public void ResetStats()
    {
        attrPoints = new Dictionary<StatVal, int>()
        {
            { StatVal.vigor, 1},
            { StatVal.finesse, 1},
            { StatVal.psyche, 1}
        };
        remainingPoints = 7;
        PartyController.Instance.playerChar.renderBody.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshes[0];
        PartyController.Instance.playerChar.renderBody.GetComponent<SkinnedMeshRenderer>().material = colors[0];

        UICharModel.Instance.SetChar(PartyController.Instance.playerChar.renderBody);
        portrait.sprite = portraits[0];
    }

    public override void ActivateMenu()
    {
        Debug.Log("Activate create");
        charCreateMenu.SetActive(true);
        active = true;

        foreach (AbilityBar a in abilityBars) a.Populate(attrPoints[a.ability]);
        CheckRemaining();

        ageTxt.text = age.ToString();

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
        CheckRemaining();
    }

    public void DecreaseAttr(StatVal attr)
    {
        if (!attrPoints.ContainsKey(attr)) Debug.LogError("Invalid attribute " + attr);
        if (attrPoints[attr] <= 1) return;
        remainingPoints += 1;
        attrPoints[attr] -= 1;
        CheckRemaining();
    }

    public void IncreaseAge()
    {
        age++;
        ageDown.interactable = true;  // TODO: make this into a "button" util
        ageDown.GetComponent<CanvasGroup>().alpha = 1f;
        if (age >= maxAge)
        {
            age = maxAge;
            ageUp.interactable = false;  // TODO: make this into a "button" util
            ageUp.GetComponent<CanvasGroup>().alpha = 0.5f;
        }
        ageTxt.text = age.ToString();
    }

    public void DecreaseAge()
    {
        age--;
        ageUp.interactable = true;  // TODO: make this into a "button" util
        ageUp.GetComponent<CanvasGroup>().alpha = 1f;
        if (age <= minAge)
        {
            age = minAge;
            ageDown.interactable = false;  // TODO: make this into a "button" util
            ageDown.GetComponent<CanvasGroup>().alpha = 0.5f;
        }
        ageTxt.text = age.ToString();
    }

    public void CheckRemaining()
    {
        pointLeftTxt.text = remainingPoints.ToString();
        var nameFilled = !string.IsNullOrEmpty(firstNameField.text) || !string.IsNullOrEmpty(lastNameField.text);
        if (remainingPoints > 0 || !nameFilled)
        {
            proceed.interactable = false;  // TODO: make this into a "button" util
            proceed.GetComponent<CanvasGroup>().alpha = 0.5f;
        }
        else
        {
            proceed.interactable = true;
            proceed.GetComponent<CanvasGroup>().alpha = 1f;
        }
    }

    public void UpdateAppearance()
    {
        var selectedModel = models[modelSelection.value];
        var selectedMat = colors[colorSelection.value];
        var oldModel = PartyController.Instance.playerChar.renderBody;

        //var newModel = Instantiate(selectedModel, PartyController.Instance.playerChar.animator.transform);
        //newModel.GetComponent<SkinnedMeshRenderer>().bones = oldModel.GetComponent<SkinnedMeshRenderer>().bones;
        //newModel.GetComponent<SkinnedMeshRenderer>().rootBone = oldModel.GetComponent<SkinnedMeshRenderer>().rootBone;
        oldModel.GetComponent<SkinnedMeshRenderer>().material = selectedMat;
        oldModel.GetComponent<SkinnedMeshRenderer>().sharedMesh = meshes[modelSelection.value];
        //Destroy(oldModel);  // TODO: maybe only swap out skinnedMeshRenderer
        //newModel.SetActive(true);
        //PartyController.Instance.playerChar.renderBody = newModel;
        UICharModel.Instance.SetChar(PartyController.Instance.playerChar.renderBody);

        int portraitIdx = (colors.Count * modelSelection.value) + colorSelection.value;
        portrait.sprite = portraits[portraitIdx];
    }

    public void AssignAttributes()
    {
        Debug.Log("Finish creation");
        Debug.Log("Remaining points: " + remainingPoints);

        if (remainingPoints > 0 || (string.IsNullOrEmpty(firstNameField.text) && string.IsNullOrEmpty(lastNameField.text))) return;
        PartyController.Instance.playerChar.charStats.SetStat(StatVal.vigor, attrPoints[StatVal.vigor]);
        PartyController.Instance.playerChar.charStats.SetStat(StatVal.finesse, attrPoints[StatVal.finesse]);
        PartyController.Instance.playerChar.charStats.SetStat(StatVal.psyche, attrPoints[StatVal.psyche]);
        PartyController.Instance.playerChar.charStats.setDerivedStats();
        var mononym = string.IsNullOrEmpty(firstNameField.text) ? lastNameField.text : string.IsNullOrEmpty(lastNameField.text) ? firstNameField.text : null;
        PartyController.Instance.playerChar.charStats.charName = mononym ?? (firstNameField.text + " " + lastNameField.text);
        PartyController.Instance.playerChar.charStats.charImage = portrait.sprite;
        string gender = genderField.options[genderField.value].text;
        if (gender == "M") {
            DialogueLua.SetVariable("PlayerThey", "he");
            DialogueLua.SetVariable("PlayerThem", "him");
            DialogueLua.SetVariable("PlayerTheir", "his");
            DialogueLua.SetVariable("PlayerTheirs", "his");
            DialogueLua.SetVariable("PlayerThey're", "he's");
            DialogueLua.SetVariable("PlayerFormal", "gentleman");
            DialogueLua.SetVariable("PlayerInformal", "lad");
        }
        else if (gender == "F")
        {
            DialogueLua.SetVariable("PlayerThey", "she");
            DialogueLua.SetVariable("PlayerThem", "her");
            DialogueLua.SetVariable("PlayerTheir", "her");
            DialogueLua.SetVariable("PlayerTheirs", "hers");
            DialogueLua.SetVariable("PlayerThey're", "she's");
            DialogueLua.SetVariable("PlayerFormal", "lady");
            DialogueLua.SetVariable("PlayerInformal", "lass");
        }
        else if (gender == "X")
        {
            DialogueLua.SetVariable("PlayerThey", "they");
            DialogueLua.SetVariable("PlayerThem", "them");
            DialogueLua.SetVariable("PlayerTheir", "their");
            DialogueLua.SetVariable("PlayerTheirs", "theirs");
            DialogueLua.SetVariable("PlayerThey're", "they're");
            DialogueLua.SetVariable("PlayerFormal", "gentleperson");
            DialogueLua.SetVariable("PlayerInformal", "kid");
        }
        DialogueLua.SetVariable("PlayerFirstName", mononym ?? firstNameField.text);
        DialogueLua.SetVariable("PlayerLastName", mononym ?? lastNameField.text);
        PartyController.Instance.playerChar.charStats.gender = gender;  // TODO: maybe just create function to get pronouns instead of Lua variables
        PartyController.Instance.playerChar.charStats.age = age;

        //DeactivateMenu();
        UIController.Instance.ActivateInitCharMenu();
    }
}
