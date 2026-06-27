using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static CharStats;

[RequireComponent(typeof(Character), typeof(CharStats))]
public class CharSigils : MonoBehaviour  // TODO: save system
{
    public Character character => GetComponent<Character>();

    public List<Sigil> initEtchedSigils;

    public List<Sigil> etchedSigils;
    public Dictionary<int, (Sigil, bool)> preparedSigils;  // mapped to slotNum, boolean for locked in

    void Awake()
    {
        SetInitSigils();
        preparedSigils = new();
    }

    private void SetInitSigils()
    {
        etchedSigils = new();
        foreach (Sigil sigil in initEtchedSigils) { 
            if(sigil != null) etchedSigils.Add(sigil);
        }
    }

    public void LoadFromSaveData(SigilSaveData saveData)
    {
        etchedSigils = new();
        for (int j = 0; j < saveData.etchedSigils.Count; j++)
        {
            etchedSigils.Add(Resources.Load<Sigil>("Scriptables/" + saveData.etchedSigils[j]));
        }

        preparedSigils = new();
        foreach(var slotNum in saveData.preparedSigils.Keys)
        {
            preparedSigils[slotNum] = (Resources.Load<Sigil>("Scriptables/" + saveData.preparedSigils[slotNum]), true);
        }
    }

    public bool EtchSigil(Sigil sigil)
    {
        if (etchedSigils.Contains(sigil) || character.inventory == null || character.inventory.money < sigil.etchPrice) return false;
        
        character.inventory.money -= sigil.etchPrice;
        etchedSigils.Add(sigil);
        return true;
    }

    public bool RemoveSigil(Sigil sigil)
    {
        int removePrice = sigil.etchPrice / 2;
        if (!etchedSigils.Contains(sigil) || character.inventory == null || character.inventory.money < removePrice) return false;

        character.inventory.money -= removePrice;
        etchedSigils.Remove(sigil);
        return true;
    }

    public bool PreprepSigil(Sigil sigil)
    {
        int slotNum = Enumerable.Range(0, character.charStats.GetCurrStat(StatVal.sigilSlots)).
            Where(i => !preparedSigils.ContainsKey(i) || preparedSigils[i].Item1 == null).DefaultIfEmpty(-1).First();
        if (!etchedSigils.Contains(sigil) || slotNum == -1) return false;

        preparedSigils[slotNum] = (sigil, false);
        Debug.Log("prepped");

        return true;
    }

    public bool UnprepSigil(int i)
    {
        if(!preparedSigils.ContainsKey(i) || preparedSigils[i].Item2 == true) return false;
        preparedSigils.Remove(i);
        return true;
    }

    public int CostToPrepare()
    {
        Debug.Log("Cost to prepare");
        foreach(var sigil in preparedSigils.Values) Debug.Log(sigil.Item1.arcanaCost);
        return preparedSigils.Values.Where(sigil => sigil.Item2 == false).Sum(sigil => sigil.Item1.arcanaCost);
    }

    public bool PrepareSigils()
    {
        int arcanaCost = CostToPrepare();
        if(arcanaCost > character.charStats.GetCurrStat(StatVal.arcana)) return false;

        character.charStats.updateMagick(-arcanaCost);
        var sigilKeys = preparedSigils.Keys.ToList();
        foreach (int i in sigilKeys)
        {
            if (preparedSigils[i].Item2 == false) preparedSigils[i] = (preparedSigils[i].Item1, true);
        }

        return true;
    }

    public void ClearExcessSigils()
    {
        var sigilKeys = preparedSigils.Keys.ToList();
        foreach (int i in sigilKeys)
        {
            if(preparedSigils[i].Item2 == false) preparedSigils.Remove(i);
        }
    }

    public bool ActivateSigil(int slotNum)
    {
        Debug.Log("Activate sigil " + slotNum);
        if (!preparedSigils.ContainsKey(slotNum) || preparedSigils[slotNum].Item2 != true) return false;
        preparedSigils[slotNum] = (null, false);
        return true;
    }

    public List<AbilityAction> PreparedSigilAbilities()
    {
        return Enumerable.Range(0, character.charStats.GetCurrStat(StatVal.sigilSlots)).
            Select(i => preparedSigils.ContainsKey(i) && preparedSigils[i].Item2 ? preparedSigils[i].Item1.action : null).ToList();
    }
}
