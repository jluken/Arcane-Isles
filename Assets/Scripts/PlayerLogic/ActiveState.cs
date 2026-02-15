using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActiveState : NPCState
{
    public ActiveState(NPC npc, NPCStateMachine playerStateMachine, List<SelectionData> actions) : base(npc, playerStateMachine, actions)
    {
    }

    public override bool isActive => true;

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        npc.mover.agent.avoidancePriority = 50;
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void SetIdle()
    {
        npcStateMachine.ChangeState(npc.IdleState);
    }

    public override void MoveTo(Vector3 destination)
    {
        //npc.mover.SetDestination(destination);
        PartyController.Instance.SetPartyDestination(destination);
    }

    //private void SetPartyDestination(Vector3 destination)  // TODO: for now this is just all in a single file line. Play with more complex configs later after it works
    //{
    //    // TODO: Determine party destination spots and then let the members themselves listen for event and decide for itself where to go (based on state - will need to store "order" in party member)
    //    var marchLeader = npc.mover;
    //    var startPoint = npc.transform.position - new Vector3(0, npc.transform.localScale.y, 0);
    //   // marchLeader.SetDestination(destination);
    //    //foreach (var corner in marchLeader.AgentPath().corners) { Debug.Log(corner); }
    //    Vector3[] leaderPathCorners = new Vector3[] { startPoint }.Concat(marchLeader.AgentPath().corners).ToArray();  // TODO: should this just be a list?
    //    //foreach (var leaderPathCorner in leaderPathCorners) { Debug.Log(leaderPathCorner); }
    //    Vector3 inFront = destination;
    //    for (int i = 0; i < party.Count; i++)
    //    {
    //        if (party[i] == npc || !party[i].CanFollow()) continue;
    //        var nextInLine = party[i].mover;
    //        var moveBackDist = 2f;
    //        for (int j = leaderPathCorners.Length - 1; j > 0; j--)
    //        {
    //            var stretch = Vector3.Distance(leaderPathCorners[j], leaderPathCorners[j - 1]);
    //            if (stretch > moveBackDist)
    //            {
    //                var direction = (leaderPathCorners[j - 1] - leaderPathCorners[j]).normalized;
    //                var nextSpot = leaderPathCorners[j] + direction * moveBackDist;
    //                nextInLine.SetDestination(nextSpot);
    //                //leaderPathCorners = nextInLine.AgentPath().corners;
    //                leaderPathCorners = leaderPathCorners.Take(j).Concat(new Vector3[] { nextSpot }).ToArray();
    //                moveBackDist = 0;
    //                break;
    //            }
    //            else moveBackDist -= stretch;
    //        }
    //        if (moveBackDist > 0)
    //        {
    //            var direction = (leaderPathCorners[0] - leaderPathCorners[1]).normalized;
    //            var nextSpot = leaderPathCorners[1] + direction * moveBackDist;
    //            nextInLine.SetDestination(nextSpot);
    //        }
    //        leaderPathCorners = nextInLine.AgentPath().corners;
    //    }
    //}
}
