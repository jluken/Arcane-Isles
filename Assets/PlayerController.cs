using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // TODO: allow moving ordering of players using draggable object code; handle in UI code?
    private int maxParty = 4;
    public List<(CharStats, bool)> party;  // Char and if selected

    public UIScript uIScript;


    public bool AddPlayer(CharStats player, int playerSlot = 3)
    {
        if (party.Count == maxParty) return false;
        if (playerSlot > party.Count) playerSlot = party.Count;

        party.Insert(playerSlot, (player, false));
        uIScript.UpdateStats();  // TODO: event subscription instead?
        return true;
    }

    public CharStats RemovePlayer(int playerSlot)
    {
        if (playerSlot >= party.Count || playerSlot <= 0) return null;
        CharStats player = party[playerSlot].Item1;
        party.RemoveAt(playerSlot);
        uIScript.UpdateStats();
        return player;
    }

    public void SelectChar(int playerSlot)
    {
        if (playerSlot < party.Count) party[playerSlot] = (party[playerSlot].Item1, true);
    }

    public void DeselectChar(int playerSlot)
    {
        if (playerSlot < party.Count) party[playerSlot] = (party[playerSlot].Item1, false);
    }

    public List<CharStats> selectedParty()
    {
        return party.Where(c => c.Item2 == true).Select(c => c.Item1).ToList();
    }

    public GameObject CurrentLeader()
    {
        return party[0].Item1.gameObject;
    }

    public GameObject CurrentSelectedLeader()
    {
        return selectedParty()[0].gameObject;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
