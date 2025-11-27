using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class MoveToClick : MonoBehaviour
{
    public NavMeshAgent agent => GetComponent<NavMeshAgent>();

    private string[] interactTags = { "object", "npc" };
    //private GameObject target;
    //private SelectionController selectionController;
    public float activateDist = 1.0f;
    private GameObject destMarkerPrefab;
    private GameObject destMarker;
    public bool useMarker = false;
    public bool controlled = false;
    public bool pathLocked { private set; get; }

    public float movingThreshold = 0.1f;
    private bool startedMoving;

    private int stopCount;
    private int maxStopCount = 20;

    public void Awake()
    {
        Debug.Log("Mover Awake");
        //agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        startedMoving = false;
        pathLocked = false;
        //selectionController = SelectionController.Instance;
        stopCount = 0;
        destMarkerPrefab = Resources.Load<GameObject>("Prefabs/Destination");

        if (controlled) SelectionController.Instance.deselectEvent += StopMoving;
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

    public NavMeshPath PathToPoint(Vector3 dest)
    {
        NavMeshPath path = new NavMeshPath();
        NavMeshHit hit;
        if(NavMesh.SamplePosition(dest, out hit, 5.0f, NavMesh.AllAreas) && agent.CalculatePath(hit.position, path)) return path;
        else return null;
    }

    public static float PathDist(NavMeshPath path)
    {
        var corners = path.corners;
        var fullDistance = 0f;
        for (int i = 1; i < corners.Length; i++)
        {
            fullDistance += Vector3.Distance(corners[i - 1], corners[i]);
        }
        return fullDistance;
    }

    public static Vector3 PointAlongPath(NavMeshPath calcPath, float dist)
    {
        float remaining = dist;
        var startNode = calcPath.corners[0];
        int nextIdx = 1;
        while (remaining > 0 && nextIdx < calcPath.corners.Length) {
            var nextNode = calcPath.corners[nextIdx];
            float p2p = Vector3.Distance(startNode, nextNode);
            if (p2p > remaining)
            {
                var direction = (nextNode - startNode).normalized;
                return startNode + direction * remaining;
            }
            remaining -= p2p;
            startNode = nextNode;
            nextIdx++;
        }
        return startNode;
    }

    public Selectable SetTempMarker(Vector3 dest)
    {
        destMarker = Instantiate(destMarkerPrefab, dest, destMarkerPrefab.transform.rotation);
        return destMarker.GetComponent<Selectable>();
    }

    public void SetDestination(Vector3 dest, bool locked = false)
    {
        Debug.Log("Check lock: " + pathLocked);
        if (pathLocked) return;
        var path = PathToPoint(dest);
        if (path != null) agent.SetPath(path);
        locked = pathLocked;
        //else
        //{
        //    Debug.Log("calcfail");
        //    Debug.Log(gameObject);
        //    Debug.Log(gameObject.transform.position);
        //    Debug.Log(agent.isOnNavMesh);
        //}
        Destroy(destMarker);
        if (useMarker) destMarker = Instantiate(destMarkerPrefab, agent.destination, destMarkerPrefab.transform.rotation);  // TODO: don't set marker if going to selectable and not in combat (just have selectable glow)
    }

    public void StopMoving()
    {
        Debug.Log("stop");
        pathLocked = false;
        agent.isStopped = true;
        agent.ResetPath();
        startedMoving = false;
        Destroy(destMarker);
        stopCount = 0;
    }

    public bool IsMoving()
    {
        return startedMoving || agent.hasPath;
    }

    public NavMeshPath AgentPath()
    {
        return agent.path;
    }

    // TODO: Create child dummy navmeshagent for that cannot move, but is used for "hovering" to display how far it will go, and maximum (UI/combat stage)
}
