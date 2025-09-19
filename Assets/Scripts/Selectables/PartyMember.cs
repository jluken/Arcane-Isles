using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharStats))]
public class PartyMember : MonoBehaviour
{
    public virtual bool mainChar { get; } = false;

    public void SetLeader()  // TODO: any difference in components between leader and follower?
    {
        if (!mainChar) SetNPCComponents(false);
        SetFollowerComponents(false);
        SetLeaderComponents(true);
    }

    public void SetFollower()
    {
        if (!mainChar) SetNPCComponents(false);
        SetLeaderComponents(false);
        SetFollowerComponents(true);
    }

    public void DeactivateCompanion()
    {
        SetFollowerComponents(false);
        SetLeaderComponents(false);
        SetNPCComponents(true);
    }

    private void SetLeaderComponents(bool enable)
    {
        // Any components that only apply to leaders?

        //gameObject.GetComponent<FieldOfView>().enabled = enable;
        gameObject.GetComponent<Follower>().enabled = enable;
        gameObject.GetComponent<MoveToClick>().enabled = enable;
        gameObject.GetComponent<NavMeshAgent>().enabled = enable;
    }

    private void SetFollowerComponents(bool enable)
    {
        gameObject.GetComponent<Follower>().enabled = enable;
        gameObject.GetComponent<MoveToClick>().enabled = enable;
        gameObject.GetComponent<NavMeshAgent>().enabled = enable;
    }

    private void SetNPCComponents(bool enable)
    {
        gameObject.GetComponent<NPC>().enabled = enable;
        gameObject.GetComponent<NavMeshObstacle>().enabled = enable;
    }
}
