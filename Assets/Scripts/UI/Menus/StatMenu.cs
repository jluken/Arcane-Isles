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
    public TMP_Text psycheText;

    public TMP_Text skillsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        charStats = player.GetComponent<CharStats>();
        UpdateStatData();
    }

    // Update is called once per frame
    void UpdateStatData()
    {
        Debug.Log("Update stat text");
        Debug.Log("Vigor: " + charStats.GetCurrStat(CharStats.StatVal.vigor));
        playerName.text = charStats.charName;
        vigorText.text = "Vigor: " + charStats.GetCurrStat(CharStats.StatVal.vigor);
        finesseText.text = "Finesse: " + charStats.GetCurrStat(CharStats.StatVal.finesse);
        psycheText.text = "Psyche: " + charStats.GetCurrStat(CharStats.StatVal.psyche);

        string skillText = "";
        foreach (CharStats.StatVal skill in Enum.GetValues(typeof(CharStats.StatVal)))
        {
            skillText += skill.ToString() + " " + charStats.GetCurrStat(skill) + "\n";
        }
        skillsText.text = skillText;

    }
}
