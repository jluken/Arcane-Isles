using UnityEngine;
using UnityEngine.EventSystems;

public class CharModelDisplay : MonoBehaviour, IDragHandler
{
    public float rotSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        float xInput = eventData.delta.x * Time.deltaTime * rotSpeed;

        UICharModel.Instance.CharacterModel.transform.Rotate(Vector3.up, -xInput, Space.World);
    }
}
