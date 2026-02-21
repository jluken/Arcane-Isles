using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PartyController : MonoBehaviour
{
    public static PartyController Instance { get; private set; }  // TODO: possible keep "all" companions in this list, but only ones following are active?
    // TODO: allow moving ordering of players using draggable object code; handle in UI code?

    public static int maxParty = 4;
    public List<PartyMember> party;
    public PartyMember playerChar;

    private PartyMember selectedPartyMember;

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
        selectedPartyMember = party[0];  // main char
        selectedPartyMember.SetActiveNPC();
        var mainCharData = partyData.partyMembers[0];
        //selectedPartyMember.gameObject.name = mainCharData.id;
        selectedPartyMember.charStats.LoadFromSaveData(mainCharData.charStatData);  // TODO: genericize this for not just party but also scene
        selectedPartyMember.inventory.LoadFromSaveData(mainCharData.inventory);
        selectedPartyMember.mover.agent.Warp(new Vector3(mainCharData.pos[0], mainCharData.pos[1], mainCharData.pos[2]));
        DestroyCompanions();
        foreach (PartyData.CharSaveData partyMemberData in partyData.partyMembers.Skip(1))
        {
            var partyMemberObj = Instantiate(Resources.Load<GameObject>("Prefabs/PartyChar"), gameObject.transform);
            Debug.Log("party member obj: " + partyMemberObj);
            partyMemberObj.name = partyMemberData.id;
            var partyMember = partyMemberObj.GetComponent<PartyMember>();

            //partyMemberComp.charObject..transform.position = new Vector3(partyMemberData.pos[0], partyMemberData.pos[1], partyMemberData.pos[2]);
            partyMember.mover.agent.Warp(new Vector3(partyMemberData.pos[0], partyMemberData.pos[1], partyMemberData.pos[2]));
            partyMember.transform.rotation = Quaternion.identity * Quaternion.Euler(partyMemberData.rot[0], partyMemberData.rot[1], partyMemberData.rot[2]);

            var memberStats = partyMember.charStats;
            memberStats.LoadFromSaveData(partyMemberData.charStatData);
            var memberInv = partyMember.inventory;
            memberInv.LoadFromSaveData(partyMemberData.inventory);

            AddCompanion(partyMember);
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

    public PartyMember leader => selectedPartyMember;

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
        selectedPartyMember.SetActiveNPC();
        PartyController.Instance.UpdateParty();
        Debug.Log("Center camera on char " + selectedPartyMember);
        camScript.Instance.TrackObj(selectedPartyMember.gameObject);
    }

    public void DeactivateParty()
    {
        foreach (var player in party) { player.gameObject.SetActive(false); }
    }


    public bool CanAddCompanion(PartyMember character)
    {
        if (character == null) return false;
        return true;
    }

    public void RecruitCompanion(Companion recruitedCompanion)  // Used for generating a new companion object
    {  
        //var companion = Instantiate(Resources.Load<GameObject>("Prefabs/PartyChar"), gameObject.transform);
        //var partyMember = companion.GetComponent<PartyMember>();
        //Debug.Log("Old NPC pos " + recruitedNPCObj.transform.position);
        //recruitedNPCObj.SetActive(false);
        //partyMember.gameObject.SetActive(true);
        //partyMember.mover.agent.Warp(recruitedNPCObj.transform.position);
        //Debug.Log("New follower pos " + partyMember.transform.position);
        //partyMember.transform.rotation = recruitedNPCObj.transform.rotation;
        //partyMember.charStats.LoadFromSaveData(PartyData.LoadCharStatData(npc.charStats));
        //partyMember.inventory.LoadFromSaveData(new EntityInventorySaveData(npc.inventory));
        if (AddCompanion(recruitedCompanion))
        {
            SceneLoader.Instance.SceneObjectManagers[recruitedCompanion.gameObject.scene.name].RemoveNPC(recruitedCompanion);
            SceneManager.MoveGameObjectToScene(recruitedCompanion.gameObject, SceneManager.GetSceneByName("PartyScene")); // TODO: hard coded?
            recruitedCompanion.SetSkills(); // TODO: how much of this function should be happening in the Companion class?
            recruitedCompanion.SetIdle();
            recruitedCompanion.recruited = true;
        }
    }


    public bool AddCompanion(PartyMember companion, int playerSlot = 4)
    {
        Debug.Log("Add Companion " + companion);
        if (companion.mainChar && party.Count > 0) throw new Exception("There can only be one party leader");
        if (party.Count == maxParty) return false; // TODO: when this returned, create pop up
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
        //Debug.Log("Debug Update Party");
        //Debug.Log(selectedPartyMember);
        //Debug.Log(selectedPartyMember.mover);
        //Debug.Log(selectedPartyMember.mover.agent);
        int leaderPriority = selectedPartyMember.mover.agent.avoidancePriority;
        int nextPriority = 1;
        foreach (PartyMember partyMember in party)
        {
            if (partyMember ==  selectedPartyMember) continue;
            partyMember.mover.agent.avoidancePriority = leaderPriority + nextPriority;
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
        //Debug.Log("Debug Select Char");
        //Debug.Log(player);
        selectedPartyMember.SetIdle();
        selectedPartyMember = player;
        selectedPartyMember.SetActiveNPC();
        camScript.Instance.TrackObj(selectedPartyMember.gameObject);
        //Debug.Log(selectedPartyMember);
        //player.SetLeader();
        UpdateParty();
    }

    public void GoTo(Vector3 destination)
    {
        selectedPartyMember.MoveCommand(destination);
        //if (CombatManager.Instance.combatActive) {
        //    Selectable target;
        //    if (SelectionController.Instance.selectedItem != null) target = SelectionController.Instance.selectedItem;
        //    else target = CombatManager.Instance.activeCombatant.GetComponent<MoveToClick>().SetTempMarker(destination); // TODO: make into an agnostic event
        //    CombatManager.Instance.UseCombatAbility(target, CombatManager.CombatActionType.Run);
        //}
        //else SetPartyDestination(destination);
    }

    public void SetPartyDestination(Vector3 destination)  // TODO: for now this is just all in a single file line. Play with more complex configs later after it works
    {
        // TODO: Determine party destination spots and then let the members themselves listen for event and decide for itself where to go (based on state - will need to store "order" in party member)
        // TODO: How much of the movement calls should happen directly vs in individual states?
        var marchLeader = selectedPartyMember.mover;
        var startPoint = selectedPartyMember.transform.position - new Vector3(0, selectedPartyMember.transform.localScale.y, 0);
        marchLeader.SetDestination(destination);
        //foreach (var corner in marchLeader.AgentPath().corners) { Debug.Log(corner); }
        Vector3[] leaderPathCorners = new Vector3[] { startPoint }.Concat(marchLeader.AgentPath().corners).ToArray();  // TODO: should this just be a list?
        //foreach (var leaderPathCorner in leaderPathCorners) { Debug.Log(leaderPathCorner); }
        Vector3 inFront = destination;
        for (int i = 0; i < party.Count; i++)
        {
            if (party[i] == selectedPartyMember || !party[i].CanFollow()) continue;
            var nextInLine = party[i].mover;
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

    public int GetLevelByXP()
    {
        return Array.FindLastIndex(levelThresholds, levelxp => levelxp <= xp) + 1;
    }
}
