using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class groundScript : MonoBehaviour  // TODO: maybe undo making the ground a "selectable". No highlighting + different behavior for travel. Can still use Ability Action and SelectionController
{

    public bool accessible { get; private set; }  // TODO: will need to toggle on/off when area is "discovered"

    public void Start()
    {
        accessible = true;
    }

    //public override List<SelectionData> CombatActions()
    //{
    //    //Returns the actions accessible during combat
    //    var acts = new List<SelectionData>() { combatMovement };
    //    return acts;
    //}

    

    //// Update is called once per frame
    //void Update() // TODO: move this to Selection Controller?
    //{
    //    RaycastHit hit;
    //    LayerMask layerMask = LayerMask.GetMask("Barrier");
    //    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, ~layerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject(-1) && hit.transform.gameObject == gameObject)
    //    {
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            SetTarget(hit.point);
    //        }
    //    }
    //}

    //public void SetTarget()
    //{
    //    SelectionController.Instance.Deselect();
    //    PartyController.Instance.GoTo(SelectionController.Instance.lastHitPoint);
    //}
}
