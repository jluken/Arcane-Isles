using System;
using TMPro;
using UnityEngine;

public class StatMenu : MonoBehaviour
{
    public GameObject player; // TODO: these assignments will be handled with a controller once swapping players is functional
    private CharStats charStats;

    public TMP_Text playerName;

    public TMP_Text vigorText;
    public TMP_Text finesseText;
    public TMP_Text witText;

    public TMP_Text skillsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        charStats = player.GetComponent<CharStats>();
        UpdateStatData();
        charStats.updateStatEvent += UpdateStatData;
    }

    // Update is called once per frame
    void UpdateStatData()
    {
        Debug.Log("Update stat text");
        Debug.Log("Vigor: " + charStats.currVigor());
        playerName.text = charStats.charName;
        vigorText.text = "Vigor: " + charStats.currVigor();
        finesseText.text = "Finesse: " + charStats.currFinesse();
        witText.text = "Wit: " + charStats.currWit();

        string skillText = "";
        foreach (SkillCheckManager.Skill skill in Enum.GetValues(typeof(SkillCheckManager.Skill)))
        {
            skillText += skill.ToString() + " " + charStats.getSkill(skill) + "\n";
        }
        skillsText.text = skillText;

    }
}
