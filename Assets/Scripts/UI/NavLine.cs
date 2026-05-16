using System;
using UnityEngine;
using UnityEngine.AI;

public class NavLine : MonoBehaviour
{
    public LineRenderer validLine;
    public LineRenderer errLine;
    public GameObject destMarker;

    public static NavLine Instance { get; private set; }
    
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        validLine.startWidth = 0.05f;
        validLine.endWidth = 0.05f;
        validLine.positionCount = 0;
        errLine.startWidth = 0.05f;
        errLine.endWidth = 0.05f;
        errLine.positionCount = 0;
    }

    public void DrawPath(NavMeshPath validPath, NavMeshPath fullPath)  // TODO: refactor indexing
    {
        var fullLength = MoveToClick.PathDist(fullPath);
        var anyValid = MoveToClick.PathDist(validPath) > 0;
        var anyErr = Math.Abs(MoveToClick.PathDist(validPath) - fullLength) > 0.00001;
        var bothLines = anyErr && anyValid;

        validLine.enabled = anyValid;
        validLine.positionCount = validPath.corners.Length;
        errLine.enabled = anyErr;

        errLine.positionCount = bothLines ? (fullPath.corners.Length - validPath.corners.Length) + 2 : anyErr ? fullPath.corners.Length : 0;

        var totalPoints = bothLines ? fullPath.corners.Length + 1 : fullPath.corners.Length;

        for (int i = 0; i < totalPoints; i++)
        {
            var onValid = i < validPath.corners.Length;
            var corner = onValid ? validPath.corners[i] : bothLines ? fullPath.corners[i - 1] : fullPath.corners[i];
            if(anyValid && onValid) validLine.SetPosition(i, corner);
            var errIdx = i - (validPath.corners.Length - 1);
            if(anyErr && (i == validPath.corners.Length - 1 || !onValid)) errLine.SetPosition(errIdx, corner);
        }
    }

    public void DisableLine()
    {
        validLine.enabled = false;
        errLine.enabled = false;
    }

    public void DisableMarker()
    {
        destMarker.SetActive(false);
    }

    public void SetMarker(Vector3 position)
    {
        destMarker.SetActive(true);
        destMarker.transform.position = position; // + new Vector3(0,1,0); // for when using projector
    }
}
