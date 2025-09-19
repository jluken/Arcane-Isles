using System;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class MoveToClick : MonoBehaviour
{
    NavMeshAgent agent;

    private string[] interactTags = { "object", "npc" };
    //private GameObject target;
    //private SelectionController selectionController;
    public float activateDist = 1.0f;
    private GameObject destMarkerPrefab;
    private GameObject destMarker;

    public float movingThreshold = 0.1f;
    private bool startedMoving;

    private int stopCount;
    private int maxStopCount = 20;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startedMoving = false;
        //selectionController = SelectionController.Instance;
        stopCount = 0;
        destMarkerPrefab = Resources.Load<GameObject>("Prefabs/Destination");
    }

    private void FixedUpdate()
    {
        //Debug.Log("fixed update");
        if (!agent.pathPending)
        {
            if (!startedMoving && agent.velocity.sqrMagnitude > movingThreshold) { startedMoving = true; stopCount = 0; Debug.Log("Start moving"); }
            if (agent.hasPath && agent.velocity.sqrMagnitude <= movingThreshold) //stuck
            {
                //Debug.Log("stopped");
                stopCount++;
                if (stopCount >= maxStopCount)
                {
                    // end pathing
                    //Debug.Log("stuck end");
                    StopMoving();
                }
            }
            else if (!agent.hasPath && startedMoving && agent.velocity.sqrMagnitude <= movingThreshold) // finished
            {
                //Debug.Log("done");
                StopMoving();
            }
        }
    }

    void LateUpdate()
    {
        //if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject(-1))  // TODO: as soon as controller updates the target (activated by controller?) - need consolidation of responsibilities
        //{
        //    RaycastHit hit;
        //    // Ignore barriers (walls, unselectable furniture, etc. Layer 8 = )
        //    LayerMask layerMask = LayerMask.GetMask("Barrier");
        //    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, ~layerMask, QueryTriggerInteraction.Ignore))
        //    {
        //        startedMoving = false;
        //        if (controller.targetSelectableObj != null)
        //        {
        //            agent.destination = controller.targetSelectableObj.transform.TransformPoint(controller.targetSelectableObj.GetComponent<Selectable>().standPoint);
        //        }
        //        else
        //        {
        //            SetDestination(hit.point);
        //        }
        //        //destMarker.SetActive(true);
                
        //        //destMarker.transform.position = new Vector3(agent.destination.x, agent.destination.y, agent.destination.z);
        //        //destMarker.transform.
        //    }
        //}

    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject == controller.targetSelectableObj && !other.gameObject.GetComponent<Selectable>().IsActive())
    //    {
    //        RaycastHit hit;
    //        var rayDirection = other.gameObject.transform.position - transform.position;
    //        if (Physics.Raycast(transform.position, rayDirection, out hit))
    //        {
    //            if (hit.transform == other.gameObject.transform)
    //            {  // Line of sight between player and target
    //                controller.targetSelectableObj.GetComponent<Selectable>().Interact();
    //                StopMoving();
    //            }

    //        }
    //    }
    //}

    public void SetDestination(Vector3 dest)
    {
        //agent.SetDestination(dest);
        //yield return new WaitForEndOfFrame();
        Debug.Log("dest");
        Debug.Log(dest);
        NavMeshPath path = new NavMeshPath();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(dest, out hit, 5.0f, NavMesh.AllAreas) && agent.CalculatePath(hit.position, path))
        {
            Debug.Log("calculated");
            agent.SetPath(path);
        }
        else
        {
            Debug.Log("calcfail");
        }
        Destroy(destMarker);
        destMarker = Instantiate(destMarkerPrefab, agent.destination, destMarkerPrefab.transform.rotation);
    }

    public void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
        startedMoving = false;
        Destroy(destMarker);
        stopCount = 0;
    }

    public NavMeshPath AgentPath()
    {
        return agent.path;
    }

    // TODO: Create child dummy navmeshagent for that cannot move, but is used for "hovering" to display how far it will go, and maximum (UI/combat stage)
}
