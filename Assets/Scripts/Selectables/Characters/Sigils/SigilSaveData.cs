using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SigilSaveData
{
    public List<string> etchedSigils;
    public Dictionary<int, string> preparedSigils;

    public SigilSaveData(CharSigils charSigils)
    {
        if (charSigils == null) return;  // Some characters won't have this component
        etchedSigils = new List<string>();
        for (int j = 0; j < charSigils.etchedSigils.Count; j++)
        {
            etchedSigils.Add(charSigils.etchedSigils[j].name);
        }

        preparedSigils = new Dictionary<int, string>();
        foreach(var slotNum in charSigils.preparedSigils.Keys)
        {
            if (charSigils.preparedSigils[slotNum].Item1 != null && charSigils.preparedSigils[slotNum].Item2 == true) preparedSigils[slotNum] =charSigils.preparedSigils[slotNum].Item1.name;
        }
    }
}
