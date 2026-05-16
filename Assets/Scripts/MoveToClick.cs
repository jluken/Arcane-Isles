using NUnit.Framework.Internal;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class MoveToClick : MonoBehaviour
{
    public NavMeshAgent agent => GetComponent<NavMeshAgent>();
    public NavMeshObstacle obstacle => GetComponent<NavMeshObstacle>();

    public float activateDist = 1.0f;
    private bool useMarker => PartyController.Instance.activePartyMember != null && agent == PartyController.Instance.activePartyMember.mover.agent;
    public bool controlled = false;
    public bool pathLocked { private set; get; }

    public float movingThreshold = 0.1f;
    private bool startedMoving;

    private int stopCount;
    private int maxStopCount = 20;

    private Character followTarget;
    private float followDist = 1.5f;

    private Vector3 lastPoint;

    public bool planted { private set; get; }

    void Start()
    {
        Debug.Log("Start MTC");
        startedMoving = false;
        pathLocked = false;
        followTarget = null;
        stopCount = 0;
        planted = false;

        if (controlled) SelectionController.Instance.deselectEvent += StopMoving;
    }

    private void Update()
    {
        if(followTarget != null)
        {
            var distToTarget = Vector3.Distance(agent.transform.position, followTarget.transform.position);
            if (!followTarget.mover.IsMoving() && distToTarget <= followDist)
            {
                followTarget = null;
                StopMoving();
            }
            else if(distToTarget > followDist) SetDestination(followTarget);
        }

        if(IsMoving() && useMarker && CombatManager.Instance.combatActive) DrawPath(AgentPath());

        if (!agent.pathPending)
        {
            if (!startedMoving && agent.velocity.sqrMagnitude > movingThreshold) { startedMoving = true; stopCount = 0;}
            if (agent.hasPath && agent.velocity.sqrMagnitude <= movingThreshold) //stuck
            {
                stopCount++;
                if (stopCount >= maxStopCount)
                {
                    // end pathing
                    StopMoving();
                }
            }
            else if (!agent.hasPath && startedMoving && agent.velocity.sqrMagnitude <= movingThreshold) // finished //TODO: also stop if never got to start moving after some amount of time
            {
                StopMoving();
            }
        }
        if (startedMoving && CombatManager.Instance.combatActive)
        {
            CombatManager.Instance.LogTravel(agent, Vector3.Distance(Utils.GroundPoint(agent.gameObject), lastPoint));
            lastPoint = Utils.GroundPoint(agent.gameObject);
        }
    }

    public NavMeshPath PathToPoint(Vector3 dest)
    {
        NavMeshPath path = new NavMeshPath();
        NavMeshHit hit;
        if(NavMesh.SamplePosition(dest, out hit, 5.0f, NavMesh.AllAreas) && agent.CalculatePath(hit.position, path)) return path;
        else return null;
    }

    public NavMeshPath PathToObj(Selectable target)
    {
        return PathToPoint(PointNearObject(target));
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

    public void Follow(Character leader)
    {
        followTarget = leader;
    }

    public void PlantFeet() {
        planted = true;
        agent.avoidancePriority = 0;
        agent.enabled = false;
        obstacle.enabled = true;
        obstacle.carving = true;
    }

    public void DefaultAvoidance() {
        planted = false;
        obstacle.enabled = false;
        agent.enabled = true;
        agent.avoidancePriority = 50;
    }

    public Vector3 PointNearObject(Selectable target)
    {
        var agentWidth = Math.Max(agent.transform.localScale.x, agent.transform.localScale.z);
        var interactRad = agent.GetComponent<Character>().reach;
        var rawPath = PathToPoint(target.transform.position);
        const float interactBuffer = 0.05f;

        int backtrack = 1;
        var ghostPoint = new Vector3(rawPath.corners[^backtrack].x, target.transform.position.y, rawPath.corners[^backtrack].z);
        while (target.GetComponent<Collider>().ClosestPoint(ghostPoint) == ghostPoint && backtrack < rawPath.corners.Length)
        {
            backtrack++;
            ghostPoint = new Vector3(rawPath.corners[^backtrack].x, target.transform.position.y, rawPath.corners[^backtrack].z);
        }
        var pointOfContact = target.GetComponent<Collider>().ClosestPoint(ghostPoint);
        var pathToEdge = PathToPoint(pointOfContact);

        return PointAlongPath(pathToEdge, (PathDist(pathToEdge) - (interactRad * agentWidth/2)) + interactBuffer); ;
    }

    public void SetDestination(Selectable target)
    {
        SetDestination(PointNearObject(target));
    }

    public void SetDestination(Vector3 dest)
    {
        if (pathLocked) return;
        var path = PathToPoint(dest);
        if (path != null) agent.SetPath(path);
        lastPoint = Utils.GroundPoint(agent.gameObject);
        if(useMarker) NavLine.Instance.DisableMarker();
        if (useMarker && PathDist(path) > 0) NavLine.Instance.SetMarker(dest);
    }

    public void DrawTo(Selectable target)
    {
        if (target == null) Debug.LogError("Cannot draw path to null target");
        DrawTo(PointNearObject(target));
    }

    public void DrawTo(Vector3 dest)
    {
        var path = PathToPoint(dest);
        if (path != null) DrawPath(path);
    }

    private void DrawPath(NavMeshPath path)
    {
        var apDist = CombatManager.Instance.GetMaxPath();
        var splitPoint = PointAlongPath(path, apDist);
        var validPath = PathToPoint(splitPoint);
        if (useMarker) NavLine.Instance.DrawPath(validPath, path);
    }

    public void StopMoving()
    {
        pathLocked = false;
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        lastPoint = Utils.GroundPoint(agent.gameObject);
        startedMoving = false;
        if (useMarker)
        {
            NavLine.Instance.DisableMarker();
            NavLine.Instance.DisableLine();
        }
        stopCount = 0;
        CombatManager.Instance.FinishAction();  // Necessary for action to register as finished before next frame
    }

    public void OnDisable()
    {
        SelectionController.Instance.deselectEvent -= StopMoving;
    }

    public void OnEnable()
    {
        if (controlled && gameObject != null) SelectionController.Instance.deselectEvent -= StopMoving;
    }

    public bool IsMoving()
    {
        return startedMoving || agent.hasPath;
    }

    public NavMeshPath AgentPath()
    {
        return agent.path;
    }
}
