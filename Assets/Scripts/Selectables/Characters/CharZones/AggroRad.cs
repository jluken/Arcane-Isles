using UnityEngine;

public class AggroRad : MonoBehaviour
{
    public Enemy enemy;

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger enter");
        //Debug.Log("Party Member: " + other.gameObject.GetComponent<PartyMember>() != null);
        //Debug.Log("Not dead: " + (other.gameObject.GetComponent<PartyMember>().StateMachine.CurrentPlayerState != other.gameObject.GetComponent<PartyMember>().DeadState));
        //Debug.Log("Companion: " + other.gameObject.GetComponent<Companion>() != null);
        if ((other.gameObject.GetComponent<PartyMember>() != null &&
            other.gameObject.GetComponent<PartyMember>().StateMachine.CurrentPlayerState != other.gameObject.GetComponent<PartyMember>().DeadState) &&
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
