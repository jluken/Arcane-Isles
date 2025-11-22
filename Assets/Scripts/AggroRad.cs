using UnityEngine;

public class AggroRad : MonoBehaviour
{

    public Enemy enemy;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Follower>() != null) { 
            RaycastHit hit;
            var rayDirection = other.gameObject.transform.position - transform.position;  // TODO: line of sight used a lot; should refactor and store somewhere
            if (Physics.Raycast(transform.position, rayDirection, out hit))
            {
                if (hit.transform == other.gameObject.transform)
                {  // Line of sight between enemy and target
                    enemy.isAggroed = true;
                }

            }
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
        
    //}
}
