using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PartyController : MonoBehaviour
{
    public static PartyController Instance { get; private set; }
    // TODO: allow moving ordering of players using draggable object code; [UI]

    public static int maxParty = 4;
    public List<PartyMember> party;
    public PartyMember playerChar;

    public PartyMember selectedPartyMember { get; private set; }

    public event Action updatePartyEvent;

    public int xp;
    public static int[] levelThresholds = { 0, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200 };

    private void Awake()
    {
        Instance = this;
        updatePartyEvent = null;
    }

    public void Start()
    {
        party = new List<PartyMember>();
        selectedPartyMember = playerChar;
        Debug.Log("Main char: " + selectedPartyMember);
        AddCompanion(selectedPartyMember);
    }

    public void InstantiateFromData(PartyData partyData)
    {
        if (partyData == null) { return; }
        xp = partyData.xp;

        party = new List<PartyMember>();
        //selectedPartyMember = playerChar;
        AddCompanion(playerChar);

        playerChar = party[0];  // main char
        var mainCharData = partyData.partyMembers[0];
        playerChar.LoadFromSaveData(mainCharData);
        DestroyCompanions();
        foreach (PartyData.CharSaveData partyMemberData in partyData.partyMembers.Skip(1))
        {
            var partyPos = new Vector3(partyMemberData.pos[0], partyMemberData.pos[1], partyMemberData.pos[2]);
            var partyMemberObj = Instantiate(Resources.Load<GameObject>("Prefabs/PartyChar"), partyPos, Quaternion.identity, gameObject.transform);
            partyMemberObj.name = partyMemberData.id;
            var partyMember = partyMemberObj.GetComponent<PartyMember>();
            partyMember.LoadFromSaveData(partyMemberData);
            AddCompanion(partyMember);
            Debug.Log(partyMember + " Load State: " + partyMember.StateMachine.CurrentPlayerState);
            if(partyMember.StateMachine.CurrentPlayerState == partyMember.ActiveState) selectedPartyMember = partyMember;
        }
    }

    public void DestroyCompanions()
    {
        foreach (var partyMember in party.Skip(1))
        {
            Destroy(partyMember.gameObject);
        }
        party = party.Take(1).ToList();
    }

    public PartyMember activePartyMember => (CombatManager.Instance.combatActive && CombatManager.Instance.activeCombatant != null) ? (CombatManager.Instance.activeCombatant.GetComponent<PartyMember>() != null ? CombatManager.Instance.activeCombatant.GetComponent<PartyMember>() :  null) : selectedPartyMember;

    public void MoveParty(List<Vector3> partyLocs, bool enable)
    {
        if (partyLocs.Count < party.Count) Debug.LogError("Not enough spawn points for the action");
        for (int i = 0; i < party.Count; i++)
        {
            Debug.Log("Moving party member " + i + " to " + partyLocs[i]);
            //party[i].gameObject.SetActive(enable);
            var feetOffset = partyLocs[i].y - party[i].GetComponent<Renderer>().bounds.min.y;
            party[i].GetComponent<NavMeshAgent>().Warp(partyLocs[i] + new Vector3(0, feetOffset, 0));
            party[i].gameObject.SetActive(enable);
        }
    }

    public List<Vector3> GetPartyLoc()
    {
        return party.Select(member => member.transform.position).ToList();
    }

    public void ActivateParty()
    {
        foreach(var player in party) { 
            //player.charObject.GetComponent<NavMeshAgent>().Warp(player.charObject.transform.position); 
            player.gameObject.SetActive(true); 
        }
        selectedPartyMember.SetActiveChar();
        UpdateParty();
        camScript.Instance.TrackObj(selectedPartyMember.gameObject);
    }

    public void DeactivateParty()
    {
        foreach (var player in party) { 
            if(player.gameObject.activeSelf) player.mover.StopMoving();  
            player.gameObject.SetActive(false); }
    }


    public bool CanAddCompanion(PartyMember character)
    {
        if (character == null) return false;
        return true;
    }

    public void RecruitCompanion(Companion recruitedCompanion)  // Used for generating a new companion object
    {  
        if (AddCompanion(recruitedCompanion))
        {
            SceneLoader.Instance.GetCurrentSceneManager(recruitedCompanion.gameObject).RemoveNPC(recruitedCompanion);
            SceneManager.MoveGameObjectToScene(recruitedCompanion.gameObject, SceneManager.GetSceneByName("PartyScene")); // TODO: hard coded?
            recruitedCompanion.Recruit();
            
        }
    }


    public bool AddCompanion(PartyMember companion, int playerSlot = 4)
    {
        Debug.Log("Add Companion " + companion);
        if (companion.mainChar && party.Count > 0) throw new Exception("There can only be one party leader");
        if (party.Count == maxParty) return false; // TODO: when this returned, create pop up [UI]
        playerSlot = Math.Min(playerSlot, party.Count);

        party.Insert(playerSlot, companion);
        //companion.SetFollower();
        UpdateParty();
        return true;
    }

    public PartyMember RemoveCompanion(PartyMember companion)
    {
        if (!party.Contains(companion)) throw new Exception("Attempt to remove companion not in list");
        if (companion.mainChar) throw new Exception("Cannot remove party leader");
        party.Remove(companion);
        if (companion == selectedPartyMember) selectedPartyMember = playerChar;
        UpdateParty();
        return companion;
    }

    public void UpdateParty()
    {
        //int leaderPriority = selectedPartyMember.mover.agent.avoidancePriority;
        //int nextPriority = 1;
        //foreach (PartyMember partyMember in party)
        //{
        //    if (partyMember ==  selectedPartyMember) continue;
        //    partyMember.mover.agent.avoidancePriority = leaderPriority + nextPriority;
        //    nextPriority++;
        //}
        updatePartyEvent?.Invoke();
    }

    public void SelectChar(PartyMember player)
    {
        selectedPartyMember.SetIdle();
        selectedPartyMember = player;
        selectedPartyMember.SetActiveChar();
        camScript.Instance.TrackObj(selectedPartyMember.gameObject);
        UpdateParty();
    }

    public void GoTo(Vector3 destination)
    {
        selectedPartyMember.MoveCommand(destination);
    }

    public void GoTo(Selectable target)
    {
        var closeDest = selectedPartyMember.mover.PointNearObject(target);
        selectedPartyMember.MoveCommand(closeDest);
    }

    public void SetPartyDestination(Vector3 destination)
    {
        var marchLeader = selectedPartyMember.mover;
        marchLeader.SetDestination(destination);
        foreach (var partyMember in party)
        {
            if (partyMember == selectedPartyMember || !partyMember.CanFollow()) continue;
            var nextInLine = partyMember.mover;
            nextInLine.Follow(selectedPartyMember);
        }
    }

    public int GetLevelByXP()
    {
        return Array.FindLastIndex(levelThresholds, levelxp => levelxp <= xp) + 1;
    }
}
