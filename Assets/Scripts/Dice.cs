using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    public static int RollDie(int dVal)
    {
        return Random.Range(1, dVal);
    }

    public static int SkillCheck(int dVal, int modifier)
    {
        return RollDie(dVal) + modifier;
    }

    public static Dictionary<int, float> TwoDSixProbs = new Dictionary<int, float>() {
        {2, 1.0f}, {3, 0.972f}, {4, 0.917f}, {5, 0.833f}, {6, 0.722f},
        {7, 0.583f}, {8, 0.417f}, {9, 0.278f}, {10, 0.167f}, {11, 0.083f}, {12, 0.028f}
    };

    public static float TwoDSixProb(int val)
    {
        int boundVal = val < 2 ? 2 : val > 12 ? 12 : val;
        return TwoDSixProbs[boundVal];
    }
}
