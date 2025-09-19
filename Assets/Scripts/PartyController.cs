using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PartyController : MonoBehaviour
{
    public static PartyController Instance { get; private set; }
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

    public GameObject leaderObject => selectedPartyMember.gameObject;


    public bool CanAddCompanion(PartyMember character)
    {
        if (character == null) return false;
        return true;
    }


    public bool AddCompanion(PartyMember companion, int playerSlot = 4)
    {
        if (companion.mainChar && party.Count > 0) throw new Exception("There can only be one party leader");
        if (party.Count == maxParty) return false; // TODO: when this returned, create pop up
        playerSlot = Math.Min(playerSlot, party.Count);

        party.Insert(playerSlot, companion);
        companion.SetFollower();
        UpdateParty();
        return true;
    }

    public PartyMember RemoveCompanion(PartyMember companion)
    {
        if (!party.Contains(companion)) throw new Exception("Attempt to remove companion not in list");
        if (companion.mainChar) throw new Exception("Cannot remove party leader");
        party.Remove(companion);
        companion.DeactivateCompanion();
        if (companion == selectedPartyMember) SelectChar(0);
        UpdateParty();
        return companion;
    }

    public void UpdateParty()
    {
        int leaderPriority = 50;
        selectedPartyMember.GetComponent<NavMeshAgent>().avoidancePriority = leaderPriority;
        int nextPriority = 1;
        foreach (PartyMember partyMember in party)
        {
            if (partyMember ==  selectedPartyMember) continue;
            partyMember.gameObject.GetComponent<NavMeshAgent>().avoidancePriority = leaderPriority + nextPriority;
            nextPriority++;
        }
        updatePartyEvent?.Invoke();
        DefaultUI.Instance.UpdateStats();
    }

    public void SelectChar(int playerSlot)
    {
        foreach (PartyMember partyMember in party) partyMember.SetFollower();
        selectedPartyMember = party[playerSlot];
        party[playerSlot].SetLeader();
        UpdateParty();
    }

    public void SelectChar(PartyMember player)
    {
        foreach (PartyMember partyMember in party) partyMember.SetFollower();
        selectedPartyMember = player;
        player.SetLeader();
        UpdateParty();
    }

    public void SetPartyDestination(Vector3 destination)  // TODO: for now this is just all in a single file line. Play with more complex configs later after it works
    {
        var marchLeader = selectedPartyMember.GetComponent<MoveToClick>();
        var startPoint = selectedPartyMember.transform.position - new Vector3(0, selectedPartyMember.transform.localScale.y, 0);
        //Debug.Log("start");
        //Debug.Log(startPoint);
        marchLeader.SetDestination(destination);
        //foreach (var corner in marchLeader.AgentPath().corners) { Debug.Log(corner); }
        Vector3[] leaderPathCorners = new Vector3[] { startPoint }.Concat(marchLeader.AgentPath().corners).ToArray();  // TODO: should this just be a list?
        //foreach (var leaderPathCorner in leaderPathCorners) { Debug.Log(leaderPathCorner); }
        Vector3 inFront = destination;
        for (int i = 0; i < party.Count; i++) {
            if (party[i] == selectedPartyMember || !party[i].GetComponent<Follower>().CanFollow()) continue;
            var nextInLine = party[i].GetComponent<MoveToClick>();
            var moveBackDist = 2f;
            for (int j = leaderPathCorners.Length - 1; j > 0; j--)
            {
                var stretch = Vector3.Distance(leaderPathCorners[j], leaderPathCorners[j - 1]);
                Debug.Log("stetch");
                Debug.Log(leaderPathCorners[j]);
                Debug.Log(leaderPathCorners[j - 1]);
                Debug.Log(stretch);
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
