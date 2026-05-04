using UnityEngine;

public class AggroRad : MonoBehaviour
{
    public Enemy enemy;

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.GetComponent<PartyMember>() != null &&
            other.gameObject.GetComponent<PartyMember>().StateMachine.CurrentPlayerState != other.gameObject.GetComponent<Companion>().DeadState) &&
            !(other.gameObject.GetComponent<Companion>() != null && 
            other.gameObject.GetComponent<Companion>().StateMachine.CurrentPlayerState == other.gameObject.GetComponent<Companion>().UnrecruitedState)
            ) {
            if (Utils.LineOfSight(gameObject, other.gameObject))
            {
                enemy.isAggroed = true;
            }
        }
    }

}
