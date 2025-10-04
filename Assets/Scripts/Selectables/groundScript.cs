using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class groundScript : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Barrier");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, ~layerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject(-1) && hit.transform.gameObject == gameObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetTarget(hit.point);
            }
        }
    }

    public void SetTarget(Vector3 dest)
    {
        SelectionController.Instance.NewSelection();
        PartyController.Instance.SetPartyDestination(dest);
    }
}
