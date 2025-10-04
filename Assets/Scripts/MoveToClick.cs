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
            if (!startedMoving && agent.velocity.sqrMagnitude > movingThreshold) { startedMoving = true; stopCount = 0; }
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
        //Debug.Log("dest");
        //Debug.Log(dest);
        NavMeshPath path = new NavMeshPath();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(dest, out hit, 5.0f, NavMesh.AllAreas) && agent.CalculatePath(hit.position, path))
        {
            //Debug.Log("calculated");
            agent.SetPath(path);
        }
        else
        {
            Debug.Log("calcfail");
            Debug.Log(gameObject);
            Debug.Log(gameObject.transform.position);
            Debug.Log(agent.isOnNavMesh);
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
