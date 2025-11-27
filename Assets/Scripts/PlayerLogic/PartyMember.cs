using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

//[RequireComponent(typeof(PartyMember))]
public class PartyMember : NPC
{
    public virtual bool mainChar { get; } = false;

    public SelectionData setPlayer;
    public SelectionData stay;
    public SelectionData follow;

    private bool stayPut;

    public override void Start()
    {
        stayPut = false;

        setPlayer = new SelectionData(this)
        {
            actionName = "Select",
            immediateAction = SetPlayer
        };
        stay = new SelectionData(this)
        {
            actionName = "Stay",
            immediateAction = Stay
        };
        follow = new SelectionData(this)
        {
            actionName = "Follow",
            immediateAction = Follow
        };

        base.Start();
    }

    public override List<SelectionData> Actions()
    {
        var acts = new List<SelectionData>() { setPlayer, talk, CanFollow() ? stay : follow};
        return acts;
    }

    public override List<SelectionData> CombatActions()
    {
        var acts = new List<SelectionData>() { startAttack, inspectSelection };
        return acts;
    }

    public void SetPlayer()
    {
        PartyController.Instance.SelectChar(this);
    }

    public void Stay()
    {
        stayPut = true;
    }

    public void Follow()
    {
        stayPut = false;
        // TODO: Immediately catch up with the leader (when switch to more complex follow logic)
    }

    public bool CanFollow()
    {
        return !stayPut;
    }

    public override void SetIdle()
    {
        Follow();
        base.SetIdle();
    }

    //public override void SetToCombat()
    //{
    //    parentPartyMember.SetToCombat();
    //}

    //public override void EndCombat()
    //{
    //    parentPartyMember.EndCombat();
    //}

}
