using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static CharStats;

[CreateAssetMenu(fileName = "CompanionData", menuName = "Scriptable Objects/CompanionData")]
public class CompanionData : ScriptableObject
{
    public Sprite charImage;
    public string charName;

    //Attributes
    public int Vigor;
    public int Finesse;
    public int Psyche;

    //Skill Level Ups in the order they're taken
    public List<StatVal> statList;
}
