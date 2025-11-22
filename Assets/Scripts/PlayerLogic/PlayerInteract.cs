using UnityEngine;

public class PlayerInteract : MonoBehaviour  // TODO: rename
{
    public NPC npc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerStay(Collider other) //TODO: OntriggerEnter? Should this be on the player side instead?
    {
        //Debug.Log(npc.name + " Searching");
        //if (npc.name.Contains("Bug"))
        //{
        //    Debug.Log(npc.name + " Searching");
        //    Debug.Log(npc.name + " Active: " + PartyController.Instance.activeNPC);
        //    Debug.Log(npc.name + " Collision: " + other.gameObject);
        //}
        if (npc == PartyController.Instance.activeNPC && SelectionController.Instance.IsSelected(other.gameObject))
        {
            Debug.Log(npc.name + " Found selected");
            RaycastHit hit;
            var rayDirection = other.gameObject.transform.position - npc.transform.position;
            if (Physics.Raycast(npc.transform.position, rayDirection, out hit))
            {
                if (hit.transform == other.gameObject.transform)
                {  // Line of sight between player and target
                    npc.GetComponent<MoveToClick>().StopMoving();
                    if(npc.GetComponent<Follower>() != null)  // TODO: how currently handling checks to see if a party member is the one moving
                    {
                        other.GetComponent<Selectable>().Interact(npc.GetComponent<Follower>().parentPartyMember);
                    }
                    
                }

            }
        }
    }
}
