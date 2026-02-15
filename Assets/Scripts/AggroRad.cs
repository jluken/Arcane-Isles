using UnityEngine;

public class AggroRad : MonoBehaviour
{

    public Enemy enemy;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PartyMember>() != null && 
            other.gameObject.GetComponent<PartyMember>().StateMachine.CurrentPlayerState != other.gameObject.GetComponent<PartyMember>().DeadState) { // TODO: handle better
            Debug.Log("Current State: " + other.gameObject.GetComponent<PartyMember>().StateMachine.CurrentPlayerState);
            if (Utils.LineOfSight(gameObject, other.gameObject))
            {
                enemy.isAggroed = true;
            }
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
        
    //}
}
