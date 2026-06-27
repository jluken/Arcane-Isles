using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sigil", menuName = "Scriptable Objects/Sigil")]
public class Sigil : ScriptableObject
{
    public string sigilName;
    public string description;
    public Sprite sprite;
    public int etchPrice;
    public int arcanaCost;

    public virtual AbilityAction action => null;
}