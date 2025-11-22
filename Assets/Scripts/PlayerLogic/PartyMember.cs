using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharStats))]
public class PartyMember : MonoBehaviour  // TODO: Do I even need this class? Shouldn't everything be stored in the Follower/NPC?
{ 
    public virtual bool mainChar { get; } = false;

    public GameObject charObject;

    public PlayerInteract InteractRad;

    public PlayerStateMachine StateMachine;
    //public PartyLeaderState LeaderState;
    public PartyFollowerState FollowerState;
    public ActiveCombatState ActiveCombatState;
    //public WaitCombatState WaitCombatState;

    public void Awake()
    {
        StateMachine = new PlayerStateMachine();

        //LeaderState = new PartyLeaderState(this, StateMachine);
        FollowerState = new PartyFollowerState(this, StateMachine);
        ActiveCombatState = new ActiveCombatState(this, StateMachine);
        //WaitCombatState = new WaitCombatState(this, StateMachine);


        if (mainChar) StateMachine.Initialize(FollowerState); else StateMachine.Initialize(FollowerState);
        Debug.Log("Awoken");
        //StateMachine.Initialize(PartyLeaderState) ? mainChar : StateMachine.Initialize(PartyFollowerState);

            //TODO: add event listener to start combat and stop combat
            //TODO: will need to be able to report whether close enough to keep combat going

    }

    void Start()
    {
        InteractRad.npc = charObject.GetComponent<NPC>();
    }

    public void TeleportChar(Vector3 position, bool active)
    {
        charObject.SetActive(active);
        var feetOffset = position.y - charObject.GetComponent<Renderer>().bounds.min.y;
        charObject.GetComponent<NavMeshAgent>().Warp(position + new Vector3(0, feetOffset, 0));
    }

    //private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    //{
    //    StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    //}

    // Update is called once per frame
    void Update()
    {
        StateMachine.CurrentPlayerState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentPlayerState.PhysicsUpdate();
    }

    public void SetToCombat()
    {
        StateMachine.ChangeState(ActiveCombatState);
    }

    public void EndCombat()
    {
        if (mainChar) StateMachine.ChangeState(FollowerState); else StateMachine.ChangeState(FollowerState);
    }

    public void TravelToPoint(Vector3 point)
    {
        StateMachine.CurrentPlayerState.TravelToPoint(point);
    }

    public void TravelToItem(Selectable item)
    {
        StateMachine.CurrentPlayerState.TravelToItem(item);
        //var initPath = gameObject.GetComponent<MoveToClick>().PathToPoint(item.transform.position);
        //if (initPath != null)
        //{

        //}
    }

}
