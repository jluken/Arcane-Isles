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
}
