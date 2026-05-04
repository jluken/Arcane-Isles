using UnityEngine;

public class CharInteract : MonoBehaviour
{
    public Character character;
    public CapsuleCollider interactCollider => GetComponent<CapsuleCollider>();

    private void OnTriggerStay(Collider other)
    {
        if (character.IsActive && SelectionController.Instance.IsSelected(other.gameObject))
        {
            //Debug.Log(npc.name + " Found selected");
            if(Utils.LineOfSight(character.gameObject, other.gameObject))
            {
                character.GetComponent<MoveToClick>().StopMoving();
                other.GetComponent<Selectable>().Interact(character);
            }
        }
    }
}
