using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : IdleState
{
    private Character npc;
    private bool waiting = false;
    private float waitTime = 0f;
    public WanderState(Character npc, CharStateMachine enemyStateMachine, List<SelectionData> actions) : base(npc, enemyStateMachine, actions)
    {
        //mover = enemy.mover;
        this.npc = npc;
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        Debug.Log("Start Patrol");
        if (npc.GetComponent<Enemy>() != null) npc.GetComponent<Enemy>().isAggroed = false;  // TODO: maybe make aggro logic generic to NPC
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (!npc.mover.IsMoving() && npc.wanderZone != null)
        {
            if (!waiting) {
                waiting = true;
                waitTime = GameData.Instance.gameTime + Random.Range(1.0f, 5.0f); // TODO: maybe make this customizable
            }
            else if (GameData.Instance.gameTime > waitTime) {
                var randomCirclePoint = Random.insideUnitCircle * npc.wanderZone.radius;
                var newDest = new Vector3(npc.wanderZone.transform.position.x + randomCirclePoint.x, 0f, npc.wanderZone.transform.position.z + randomCirclePoint.y);

                npc.mover.SetDestination(newDest);
                waiting = false;
            }
        }


        if (npc.GetComponent<Enemy>() != null && npc.GetComponent<Enemy>().isAggroed)
        {
            CombatManager.Instance.InitiateCombat();
        }
    }

    //public IEnumerator Wait()
    //{
    //    var waitTime = Random.Range(1.0f, 5.0f);
    //    yield return new WaitForSeconds(waitTime);  
    //    waiting = false;
    //}

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
