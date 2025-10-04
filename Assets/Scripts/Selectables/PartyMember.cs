using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharStats))]
public class PartyMember : MonoBehaviour
{
    public virtual bool mainChar { get; } = false;

    public GameObject charObject;

    public void MoveChar(Vector3 position, bool active)
    {
        charObject.SetActive(active);
        var feetOffset = position.y - charObject.GetComponent<Renderer>().bounds.min.y;
        charObject.GetComponent<NavMeshAgent>().Warp(position + new Vector3(0, feetOffset, 0));
    }

    //public void SetLeader()  // TODO: any difference in components between leader and follower?
    //{
    //    if (!mainChar) SetNPCComponents(false);
    //    SetFollowerComponents(false);
    //    SetLeaderComponents(true);
    //}

    //public void SetFollower()
    //{
    //    if (!mainChar) SetNPCComponents(false);
    //    SetLeaderComponents(false);
    //    SetFollowerComponents(true);
    //}

    //public void DismissCompanion()
    //{
    //    SetFollowerComponents(false);
    //    SetLeaderComponents(false);
    //    SetNPCComponents(true);
    //}

    //private void SetLeaderComponents(bool enable)
    //{
    //    // Any components that only apply to leaders?

    //    //gameObject.GetComponent<FieldOfView>().enabled = enable;
    //    charObject.GetComponent<Follower>().enabled = enable;
    //    charObject.GetComponent<MoveToClick>().enabled = enable;
    //    charObject.GetComponent<NavMeshAgent>().enabled = enable;
    //}

    //private void SetFollowerComponents(bool enable)
    //{
    //    charObject.GetComponent<Follower>().enabled = enable;
    //    charObject.GetComponent<MoveToClick>().enabled = enable;
    //    charObject.GetComponent<NavMeshAgent>().enabled = enable;
    //}

    //private void SetNPCComponents(bool enable)
    //{
    //    charObject.GetComponent<NPC>().enabled = enable;
    //    charObject.GetComponent<NavMeshObstacle>().enabled = enable;
    //}
}
