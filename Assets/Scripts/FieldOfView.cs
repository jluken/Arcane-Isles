using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public int meshResolution;
    public float fieldOfViewDistance;
    public int edgeResolveIterations;
    public float edgeDistThreshold;

    public float visionBuffer = 0.1f;

    public LayerMask obstacleMask;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    public MeshFilter topMeshFilter;
    Mesh topMesh;
    public MeshFilter midMeshFilter;
    Mesh midMesh;

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View mesh";
        viewMeshFilter.mesh = viewMesh;
        topMesh = new Mesh();
        topMesh.name = "Top mesh";
        topMeshFilter.mesh = topMesh;
        midMesh = new Mesh();
        midMesh.name = "Mid mesh";
        midMeshFilter.mesh = midMesh;
    }

    void drawFieldOfView()
    {
        float stepAngleSize = 360 / meshResolution;
        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= meshResolution; i++)
        {
            float angle = stepAngleSize * i;
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle) * fieldOfViewDistance, Color.red);
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeThreshold = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDistThreshold;
                if(oldViewCast.hit != newViewCast.hit || edgeThreshold)
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        Debug.DrawLine(transform.position, edge.pointA, Color.blue);
                        viewPoints.Add(edge.pointA + newViewCast.dir * visionBuffer);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        Debug.DrawLine(transform.position, edge.pointB, Color.green);
                        viewPoints.Add(edge.pointB + newViewCast.dir * visionBuffer);
                    }
                }
            }

            viewPoints.Add(newViewCast.point + newViewCast.dir * visionBuffer);
            oldViewCast = newViewCast;
            Debug.DrawLine(transform.position, newViewCast.point, Color.red);
        }
        //viewPoints.Add(ViewCast(0f).point);  // start from beginning for circle
        //Debug.DrawLine(transform.position, ViewCast(0f).point, Color.black);

        //Debug.Log(transform.localScale.y);


        var orthDir = new Vector3(-1.0f, 1.08f, 1.0f) * 3;
        orthDir = new Vector3(0.0f, 0.0f, 0.0f); // Just ignore for now, field of view will be separate from visibility mesh
        //var topOrthDir = new Vector3(-0.33f, 1.08f, 0.33f) * 3;


        int vertexCount = viewPoints.Count + 1; // 1 for origin
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        Vector3[] topVertices = new Vector3[vertexCount];

        vertices[0] = transform.InverseTransformPoint(transform.position + orthDir);
        //topVertices[0] = transform.InverseTransformPoint(transform.position + topOrthDir);
        //var rightmost = 1;
        //var leftmost = 1;  
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i] + orthDir);
            //topVertices[i + 1] = transform.InverseTransformPoint(viewPoints[i] + topOrthDir);
            //if (viewPoints[i].x + viewPoints[i].z > viewPoints[rightmost - 1].x + viewPoints[rightmost - 1].z)
            //{
            //    rightmost = i + 1;
            //}
            //if (viewPoints[i].x + viewPoints[i].z < viewPoints[leftmost - 1].x + viewPoints[leftmost - 1].z)
            //{
            //    leftmost = i + 1;
            //}
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
        //topMesh.Clear();
        //topMesh.vertices = topVertices;
        //topMesh.triangles = triangles;
        //topMesh.RecalculateNormals();

        //Vector3[] midVertices = new Vector3[4];
        //midVertices[0] = vertices[leftmost];
        //midVertices[1] = vertices[rightmost];
        //midVertices[2] = topVertices[leftmost];
        //midVertices[3] = topVertices[rightmost];
        //int[] midTriangles = new int[6];
        //midTriangles[0] = 0;
        //midTriangles[1] = 1;
        //midTriangles[2] = 3;
        //midTriangles[3] = 0;
        //midTriangles[4] = 2;
        //midTriangles[5] = 3;
        //midMesh.Clear();
        //midMesh.vertices = midVertices;
        //midMesh.triangles = midTriangles;
        //midMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero, maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++) {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeThreshold = Mathf.Abs(minViewCast.dist - newViewCast.dist) > edgeDistThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeThreshold)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, DirFromAngle(minAngle), maxPoint, DirFromAngle(maxAngle));
    }

    ViewCastInfo ViewCast(float angle)
    {
        Vector3 dir = DirFromAngle(angle);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, fieldOfViewDistance, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, angle, dir);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * fieldOfViewDistance, fieldOfViewDistance, angle, dir);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;
        public Vector3 dir;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle, Vector3 _dir)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
            dir = _dir;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 dirA;
        public Vector3 pointB;
        public Vector3 dirB;

        public EdgeInfo(Vector3 _pointA, Vector3 _dirA, Vector3 _pointB, Vector3 _dirB)
        {
            pointA = _pointA;
            dirA = _dirA;
            pointB = _pointB;
            dirB = _dirB;
        }
    }

    public Vector3 DirFromAngle(float degreeAngle)
    {
        return new Vector3(Mathf.Sin(degreeAngle * Mathf.Deg2Rad),0,Mathf.Cos(degreeAngle * Mathf.Deg2Rad));    
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    void Update()
    {
        drawFieldOfView(); 
    }
}
