using UnityEngine;

public class AggroRad : MonoBehaviour
{

    public Enemy enemy;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PartyMember>() != null) { 
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
