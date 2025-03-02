using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Controller : MonoBehaviour
{
    public GameObject targetSelectableObj;
    public GameObject hoverObj;

    private GameObject selectMenu;
    public MoveToClick mover;  //TODO: handle this with player controller for current active player


    // Start is called before the first frame update
    void Start()
    {
        selectMenu = GameObject.Find("SelectMenu");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, -1, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject(-1)) // TODO: barriers should be ignored
        {
            hoverObj = hit.transform.gameObject;
            Selectable hoverSelectable = hoverObj.GetComponent<Selectable>();  // TODO: separate out click functionality into different components? (all this would need to do make sure only one selected object at a time and deselect others; ground should make similar call to deselect all as normal select object does (event rather than controller?))
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Clickable: " + hoverSelectable);
                Debug.Log("Hit " + hoverObj.name);
                Debug.Log("target: " + targetSelectableObj);
                if (targetSelectableObj != null && targetSelectableObj != hoverObj)  // Old selected no longer selected
                {
                    targetSelectableObj.GetComponent<Selectable>().Deselect();
                }

                if (hoverSelectable != null)
                {
                    hoverSelectable.SetTarget();
                }
                else
                {
                    targetSelectableObj = null;
                    mover.SetDestination(hit.point);
                }

                //if (hoverSelectable != null)  // TODO: move logic to selectable code?
                //{
                //    targetSelectableObj = hoverObj;
                //}
                //else
                //{
                //    targetSelectableObj = null;
                //}
            }
            else if (Input.GetMouseButtonDown(1) && hoverSelectable != null) //right click on selectable
            {
                Debug.Log("select menu: " + selectMenu);
                selectMenu.GetComponent<SelectMenu>().ActivateMenu(Input.mousePosition, hoverSelectable.Actions());
            }

            if (hoverObj.GetComponent<Selectable>() != null) {
                hoverSelectable.Display();
            }
        }
    }

    //public void SetTarget(GameObject selectTarget)  // TODO: enforce that must be selectable
    //{
    //    Debug.Log("Set Target " + selectTarget);
    //    if (targetSelectableObj != null) {
    //        Selectable hoverSelectable = targetSelectableObj.GetComponent<Selectable>();
    //        mover.SetDestination(selectTarget.transform.TransformPoint(hoverSelectable.standPoint));  // TODOL what to do with standpoint?
    //        hoverSelectable.Select();
    //    }
    //    else
    //    {
    //        mover.SetDestination(hit.point);
    //    }
        
        
    //}
}
