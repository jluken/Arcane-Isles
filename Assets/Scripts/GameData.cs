using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    public float gameTime;

    private static long secsInDay = 86400;
    private static long daysInMonth = 30;
    private static long monthsInYear = 12;

    public static string[] months = {  // TODO: make more months
        "Month 1",
        "Month 2",
        "Month 3",
        "Month 4",
        "Month 5",
        "Month 6",
        "Theri",
        "Month 8",
        "Month 9",
        "Month 10",
        "Month 11",
        "Month 12",
    };

    // TODO: make this a json/scriptable?
    public int startYear = 715;
    public int startMonth = 7;
    public int startDay = 30; // 31; (only 30 days a month for now)
    public float startSecs = 28800;

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameTime = DateToSec(startYear, startMonth, startDay, startSecs);
    }

    public float DateToSec(long year, long month, long day, float seconds)
    {
        Debug.Log("Year secs: " + (year * monthsInYear * daysInMonth * secsInDay));
        Debug.Log("Month secs: " + (month * daysInMonth * secsInDay));
        Debug.Log("Day secs: " + (day * secsInDay));
        return ((year - 1) * monthsInYear * daysInMonth * secsInDay) + ((month - 1) * daysInMonth * secsInDay) + ((day - 1) * secsInDay) + seconds;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CombatManager.Instance.combatActive) gameTime += Time.deltaTime;  // Time incremented by CombatManager during combat
    }

    public static string DateString(float time)  // TODO: make calendar system
    {
        var daysSinceStart = (int)(time / secsInDay);
        var monthsSinceStart = daysSinceStart / daysInMonth;
        var yearsSinceStart = monthsSinceStart / monthsInYear;

        var year = 1 + yearsSinceStart;
        var month = months[(monthsSinceStart % monthsInYear)];
        var day = 1 + (daysSinceStart % daysInMonth);

        return month + " " + day + ", " + year;
    }
}
