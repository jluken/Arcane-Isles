using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class MoveToClick : MonoBehaviour
{
    NavMeshAgent agent;

    private string[] interactTags = { "object", "npc" };
    //private GameObject target;
    public Controller controller;
    public float activateDist = 1.0f;
    public GameObject destMarkerPrefab;
    private GameObject destMarker;

    public float movingThreshold = 0.1f;
    private bool startedMoving;

    private int stopCount;
    private int maxStopCount = 20;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stopCount = 0;
    }

    private void FixedUpdate()
    {
        //Debug.Log("fixed update");
        if (!agent.pathPending)
        {
            if (!startedMoving && agent.velocity.sqrMagnitude > movingThreshold) { startedMoving = true; stopCount = 0; Debug.Log("Start moving"); }
            //if (destMarker != null) Debug.Log("no path pending");
            //if (destMarker != null) Debug.Log("velocity: ");
            //if (destMarker != null) Debug.Log(agent.velocity.sqrMagnitude);
            if (agent.hasPath && startedMoving && agent.velocity.sqrMagnitude <= movingThreshold) //stuck
            {
                Debug.Log("stopped");
                stopCount++;
                if (stopCount >= maxStopCount) // TODO: what if start out stuck?
                {
                    // end pathing
                    Debug.Log("stuck end");
                    StopMoving();
                }
            }
            else if (!agent.hasPath && startedMoving && agent.velocity.sqrMagnitude <= movingThreshold) // finished
            {
                Debug.Log("done");
                StopMoving();
            }
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject(-1))  // TODO: as soon as controller updates the target (activated by controller?) - need consolidation of responsibilities
        {
            RaycastHit hit;
            // Ignore barriers (walls, unselectable furniture, etc. Layer 8 = )
            LayerMask layerMask = LayerMask.GetMask("Barrier");
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, ~layerMask, QueryTriggerInteraction.Ignore))
            {
                startedMoving = false;
                if (controller.targetSelectableObj != null)
                {
                    agent.destination = controller.targetSelectableObj.transform.TransformPoint(controller.targetSelectableObj.GetComponent<Selectable>().standPoint);
                }
                else
                {
                    SetDestination(hit.point);
                }
                //destMarker.SetActive(true);
                
                //destMarker.transform.position = new Vector3(agent.destination.x, agent.destination.y, agent.destination.z);
                //destMarker.transform.
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == controller.targetSelectableObj && !other.gameObject.GetComponent<Selectable>().IsActive())
        {
            RaycastHit hit;
            var rayDirection = other.gameObject.transform.position - transform.position;
            if (Physics.Raycast(transform.position, rayDirection, out hit))
            {
                if (hit.transform == other.gameObject.transform)
                {  // Line of sight between player and target
                    controller.targetSelectableObj.GetComponent<Selectable>().Activate();
                    StopMoving();
                }

            }
        }
    }

    public void SetDestination(Vector3 dest)
    {
        agent.destination = dest;
        Destroy(destMarker);
        destMarker = Instantiate(destMarkerPrefab, agent.destination, destMarkerPrefab.transform.rotation);
    }

    private void StopMoving()
    {
        //agent.destination = agent.transform.position;
        agent.isStopped = true;
        agent.ResetPath();
        startedMoving = false;
        Destroy(destMarker);
        stopCount = 0;
    }
}
