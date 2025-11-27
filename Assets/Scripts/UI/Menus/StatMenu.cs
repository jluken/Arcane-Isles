using System;
using TMPro;
using UnityEngine;

public class StatMenu : MonoBehaviour  //TODO: used?
{

    public TMP_Text playerName;

    public TMP_Text vigorText;
    public TMP_Text finesseText;
    public TMP_Text psycheText;

    public TMP_Text skillsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        UpdateStatData();
    }

    // Update is called once per frame
    void UpdateStatData()
    {
        var playerStats = PartyController.Instance.leader.charStats;
        Debug.Log("Update stat text");
        Debug.Log("Vigor: " + playerStats.GetCurrStat(CharStats.StatVal.vigor));
        playerName.text = playerStats.charName;
        vigorText.text = "Vigor: " + playerStats.GetCurrStat(CharStats.StatVal.vigor);
        finesseText.text = "Finesse: " + playerStats.GetCurrStat(CharStats.StatVal.finesse);
        psycheText.text = "Psyche: " + playerStats.GetCurrStat(CharStats.StatVal.psyche);

        string skillText = "";
        foreach (CharStats.StatVal skill in Enum.GetValues(typeof(CharStats.StatVal)))
        {
            skillText += skill.ToString() + " " + playerStats.GetCurrStat(skill) + "\n";
        }
        skillsText.text = skillText;

    }
}
