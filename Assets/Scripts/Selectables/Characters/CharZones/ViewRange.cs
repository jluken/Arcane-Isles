using UnityEngine;

public class ViewRange : MonoBehaviour
{
    public Character character;
    public CapsuleCollider viewCollider => GetComponent<CapsuleCollider>();

    private void OnTriggerStay(Collider other)
    {
        if (character.IsActive && other.GetComponent<VisTarget>() != null)
        {
            if(Utils.LineOfSight(character.gameObject, other.gameObject))
            {
                other.GetComponent<VisTarget>().TriggerTarget();
            }
            else
            {
                other.GetComponent<VisTarget>().SetUnseen();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<VisTarget>() != null) other.GetComponent<VisTarget>().SetUnseen();
    }
}
