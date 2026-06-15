using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    public float gameTime;

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CombatManager.Instance.combatActive) gameTime += Time.deltaTime;  // Time incremented by CombatManager during combat
    }

    public static string DateString(double time)  // TODO: make calendar system
    {
        var daysSinceStart = (int)(time / 86400);
        var monthsSinceStart = daysSinceStart / 30;
        var yearsSinceStart = monthsSinceStart / 12;

        var year = 1 + yearsSinceStart;
        var month = 1 + (monthsSinceStart % 12);
        var day = 1 + (daysSinceStart % 30);

        return "Month" + month + " " + day + ", " + year;
    }
}
