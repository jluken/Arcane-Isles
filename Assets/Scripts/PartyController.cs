using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PartyController : MonoBehaviour
{
    public static PartyController Instance { get; private set; }  // TODO: possible keep "all" companions in this list, but only ones following are active?
    // TODO: allow moving ordering of players using draggable object code; handle in UI code?

    private int maxParty = 4;
    public List<PartyMember> party;

    private PartyMember selectedPartyMember;

    public delegate void UpdatePartyEvent(); // TODO: do I need multiple void delegates?
    public event UpdatePartyEvent updatePartyEvent;

    private void Awake()
    {
        Instance = this;
        updatePartyEvent = null;
    }

    public void Start()
    {
        party = new List<PartyMember>();
        selectedPartyMember = PlayerChar.Instance.gameObject.GetComponent<PartyMember>();
        AddCompanion(selectedPartyMember);
    }

    public void InstantiateFromData(PartyData partyData)
    {
        if (partyData == null) { return; }
        selectedPartyMember = party[0];  // main char
        var mainCharData = partyData.partyMembers[0];
        selectedPartyMember.gameObject.name = mainCharData.id;
        selectedPartyMember.GetComponent<CharStats>().LoadFromSaveData(mainCharData.charStatData);
        selectedPartyMember.GetComponent<EntityInventory>().LoadFromSaveData(mainCharData.inventory);
        //selectedPartyMember.charObject.transform.position = new Vector3(mainCharData.pos[0], mainCharData.pos[1], mainCharData.pos[2]);
        selectedPartyMember.charObject.GetComponent<NavMeshAgent>().Warp(new Vector3(mainCharData.pos[0], mainCharData.pos[1], mainCharData.pos[2]));
        DestroyCompanions();
        foreach (PartyData.CharSaveData partyMemberData in partyData.partyMembers.Skip(1))
        {
            var partyMember = Instantiate(Resources.Load<GameObject>("Prefabs/PartyChar"), gameObject.transform);
            partyMember.name = partyMemberData.id;
            var partyMemberComp = partyMember.GetComponent<PartyMember>();

            //partyMemberComp.charObject..transform.position = new Vector3(partyMemberData.pos[0], partyMemberData.pos[1], partyMemberData.pos[2]);
            partyMemberComp.charObject.GetComponent<NavMeshAgent>().Warp(new Vector3(partyMemberData.pos[0], partyMemberData.pos[1], partyMemberData.pos[2]));
            partyMemberComp.charObject.transform.rotation = Quaternion.identity * Quaternion.Euler(partyMemberData.rot[0], partyMemberData.rot[1], partyMemberData.rot[2]);

            var memberStats = partyMember.GetComponent<CharStats>();
            memberStats.LoadFromSaveData(partyMemberData.charStatData);
            var memberInv = partyMember.GetComponent<EntityInventory>();
            memberInv.LoadFromSaveData(partyMemberData.inventory);

            AddCompanion(partyMemberComp);
        }
    }

    public void DestroyCompanions()
    {
        foreach (var partyMember in party.Skip(1))
        {
            GameObject.Destroy(partyMember.charObject);
            GameObject.Destroy(partyMember.gameObject);
        }
        party = party.Take(1).ToList();
    }

    public PartyMember leader => selectedPartyMember;  // TODO: combine with CombatManager active NPC after refactor

    public NPC activeNPC => CombatManager.Instance.combatActive ? CombatManager.Instance.activeCombatant : leader.charObject.GetComponent<NPC>();  // TODO: put somewhere else

    public void MoveParty(List<Vector3> partyLocs, bool enable)
    {
        if (partyLocs.Count < party.Count) Debug.LogError("Not enough spawn points for the action");
        for (int i = 0; i < party.Count; i++)
        {
            Debug.Log("Moving party member " + i + " to " + partyLocs[i]);
            party[i].TeleportChar(partyLocs[i], enable);
        }
    }

    public List<Vector3> GetPartyLoc()
    {
        return party.Select(member => member.charObject.transform.position).ToList();
    }

    public void ActivateParty()
    {
        foreach(var player in party) { 
            //player.charObject.GetComponent<NavMeshAgent>().Warp(player.charObject.transform.position); 
            player.charObject.SetActive(true); 
        }
        camScript.Instance.CenterCamera(selectedPartyMember.charObject.transform.position);
    }

    public void DeactivateParty()
    {
        foreach (var player in party) { player.charObject.SetActive(false); }
    }


    public bool CanAddCompanion(PartyMember character)
    {
        if (character == null) return false;
        return true;
    }

    public void CreateCompanion(NPC npc, GameObject recruitedNPCObj)  // Used for generating a new companion object
    {
        Debug.Log("Create Companion");
        var companion = Instantiate(Resources.Load<GameObject>("Prefabs/PartyChar"), gameObject.transform);
        var partyMember = companion.GetComponent<PartyMember>();
        Debug.Log("Old NPC pos " + recruitedNPCObj.transform.position);
        recruitedNPCObj.SetActive(false);
        partyMember.charObject.SetActive(true);
        partyMember.charObject.GetComponent<NavMeshAgent>().Warp(recruitedNPCObj.transform.position);
        Debug.Log("New follower pos " + partyMember.charObject.transform.position);
        partyMember.charObject.transform.rotation = recruitedNPCObj.transform.rotation;
        companion.GetComponent<CharStats>().LoadFromSaveData(PartyData.LoadCharStatData(npc.charStats));
        companion.GetComponent<EntityInventory>().LoadFromSaveData(new EntityInventorySaveData(npc.inventory));
        AddCompanion(partyMember);
    }


    public bool AddCompanion(PartyMember companion, int playerSlot = 4)
    {
        Debug.Log("Add Companion");
        if (companion.mainChar && party.Count > 0) throw new Exception("There can only be one party leader");
        if (party.Count == maxParty) return false; // TODO: when this returned, create pop up
        playerSlot = Math.Min(playerSlot, party.Count);

        party.Insert(playerSlot, companion);
        //companion.SetFollower();
        UpdateParty();
        return true;
    }

    //public PartyMember RemoveCompanion(PartyMember companion)
    //{
    //    if (!party.Contains(companion)) throw new Exception("Attempt to remove companion not in list");
    //    if (companion.mainChar) throw new Exception("Cannot remove party leader");
    //    party.Remove(companion);
    //    companion.DismissCompanion();
    //    if (companion == selectedPartyMember) SelectChar(0);
    //    UpdateParty();
    //    return companion;
    //}

    public void UpdateParty()
    {
        int leaderPriority = 50;
        Debug.Log("Debug Update Party");
        Debug.Log(selectedPartyMember);
        Debug.Log(selectedPartyMember.charObject);
        Debug.Log(selectedPartyMember.charObject.GetComponent<NavMeshAgent>());
        selectedPartyMember.charObject.GetComponent<NavMeshAgent>().avoidancePriority = leaderPriority;
        int nextPriority = 1;
        foreach (PartyMember partyMember in party)
        {
            if (partyMember ==  selectedPartyMember) continue;
            partyMember.charObject.GetComponent<NavMeshAgent>().avoidancePriority = leaderPriority + nextPriority;
            nextPriority++;
        }
        updatePartyEvent?.Invoke();
        //DefaultUI.Instance.UpdateStats();
    }

    //public void SelectChar(int playerSlot)
    //{
    //    foreach (PartyMember partyMember in party) partyMember.SetFollower();
    //    selectedPartyMember = party[playerSlot];
    //    party[playerSlot].SetLeader();
    //    UpdateParty();
    //}

    public void SelectChar(PartyMember player)
    {
        //foreach (PartyMember partyMember in party) partyMember.SetFollower();
        Debug.Log("Debug Select Char");
        Debug.Log(player);
        selectedPartyMember = player;
        Debug.Log(selectedPartyMember);
        //player.SetLeader();
        UpdateParty();
    }

    public void GoTo(Vector3 destination)
    {
        if (CombatManager.Instance.combatActive) {
            Selectable target;
            if (SelectionController.Instance.selectedItem != null) target = SelectionController.Instance.selectedItem;
            else target = CombatManager.Instance.activeCombatant.GetComponent<MoveToClick>().SetTempMarker(destination);
            CombatManager.Instance.UseCombatAbility(target, CombatManager.CombatActionType.Run);
        }
        else SetPartyDestination(destination);
    }

    private void SetPartyDestination(Vector3 destination)  // TODO: for now this is just all in a single file line. Play with more complex configs later after it works
    {
        // TODO: Just make this a method that send the leader to the point and then calls some functions on the canFollow followers
        var marchLeader = selectedPartyMember.charObject.GetComponent<MoveToClick>();
        var startPoint = selectedPartyMember.charObject.transform.position - new Vector3(0, selectedPartyMember.transform.localScale.y, 0);
        marchLeader.SetDestination(destination);
        //foreach (var corner in marchLeader.AgentPath().corners) { Debug.Log(corner); }
        Vector3[] leaderPathCorners = new Vector3[] { startPoint }.Concat(marchLeader.AgentPath().corners).ToArray();  // TODO: should this just be a list?
        //foreach (var leaderPathCorner in leaderPathCorners) { Debug.Log(leaderPathCorner); }
        Vector3 inFront = destination;
        for (int i = 0; i < party.Count; i++) {
            if (party[i] == selectedPartyMember || !party[i].charObject.GetComponent<Follower>().CanFollow()) continue;
            var nextInLine = party[i].charObject.GetComponent<MoveToClick>();
            var moveBackDist = 2f;
            for (int j = leaderPathCorners.Length - 1; j > 0; j--)
            {
                var stretch = Vector3.Distance(leaderPathCorners[j], leaderPathCorners[j - 1]);
                if (stretch > moveBackDist)
                {
                    var direction = (leaderPathCorners[j - 1] - leaderPathCorners[j]).normalized;
                    var nextSpot = leaderPathCorners[j] + direction * moveBackDist;
                    nextInLine.SetDestination(nextSpot);
                    //leaderPathCorners = nextInLine.AgentPath().corners;
                    leaderPathCorners = leaderPathCorners.Take(j).Concat(new Vector3[] { nextSpot }).ToArray();
                    moveBackDist = 0;
                    break;
                }
                else moveBackDist -= stretch;
            }
            if (moveBackDist > 0)
            {
                var direction = (leaderPathCorners[0] - leaderPathCorners[1]).normalized;
                var nextSpot = leaderPathCorners[1] + direction * moveBackDist;
                nextInLine.SetDestination(nextSpot);
            }
            leaderPathCorners = nextInLine.AgentPath().corners;
        }
    }
}
