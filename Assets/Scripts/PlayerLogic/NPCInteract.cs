using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    public NPC npc;
    public CapsuleCollider interactCollider => GetComponent<CapsuleCollider>();

    private void OnTriggerStay(Collider other)
    {
        if (npc.IsActive && SelectionController.Instance.IsSelected(other.gameObject))
        {
            //Debug.Log(npc.name + " Found selected");
            if(Utils.LineOfSight(npc.gameObject, other.gameObject))
            {
                npc.GetComponent<MoveToClick>().StopMoving();
                other.GetComponent<Selectable>().Interact(npc);
            }
        }
    }
}
