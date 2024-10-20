using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(LineRenderer))]

public class FillPolygon : MonoBehaviour
{
    // Based onh ttp://answers.unity3d.com/questions/835675/how-to-fill-polygon-collider-with-a-solid-color.html

    public enum Interior { None, Filled }
    public enum Outline { None, Open, Closed }

    public Interior interior = Interior.Filled;
    public Outline outline = Outline.Closed;

    public PolygonCollider2D polygonCollider2D;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public LineRenderer lineRenderer;
    public bool isWorldSpaceUV;
    public bool isOutlineClosed = true;

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        CreateMesh();
        CreateLine();
    }

    void CreateMesh()
    {
        if (polygonCollider2D == null) polygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        if ((meshFilter == null || meshRenderer == null) && interior == Interior.None) return;
        if (meshFilter == null)
        {
            Debug.LogError(this + " has null meshFilter");
            return;
        }
        if (meshRenderer == null)
        {
            Debug.LogError(this + " has null meshRenderer");
            return;
        }

        if (interior == Interior.None)
        {
            meshRenderer.enabled = false;
            return;
        }
        else
        {
            meshRenderer.enabled = true;
        }


        //Render thing
        int pointCount = 0;
        pointCount = polygonCollider2D.GetTotalPointCount();
        Mesh mesh = new Mesh();
        Vector2[] points = polygonCollider2D.points;
        Vector3[] vertices = new Vector3[pointCount];
        Vector2[] uv = new Vector2[pointCount];
        for (int j = 0; j < pointCount; j++)
        {
            Vector2 actual = points[j];
            vertices[j] = new Vector3(actual.x, actual.y, 0);
            if (isWorldSpaceUV)
            {
                uv[j] = actual;

            }
            else
            {
                uv[j] = new Vector2(actual.x / polygonCollider2D.bounds.size.x, actual.y / polygonCollider2D.bounds.size.y);
            }
        }
        Triangulator tr = new Triangulator(points);
        int[] triangles = tr.Triangulate();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        meshFilter.mesh = mesh;
    }

    void CreateLine()
    {
        if (polygonCollider2D == null) polygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null && outline == Outline.None) return;
        if (lineRenderer == null)
        {
            Debug.LogError(this + " has null lineRenderer");
            return;
        }
        if (outline == Outline.None)
        {
            lineRenderer.enabled = false;
            return;
        }
        else
        {
            lineRenderer.enabled = true;
        }

        //Render thing
        int pointCount = 0;
        pointCount = polygonCollider2D.GetTotalPointCount();
        if (pointCount < 1) return;

        if (outline == Outline.Closed) pointCount++;
        Vector2[] points = polygonCollider2D.points;

        lineRenderer.positionCount = pointCount;

        for (int j = 0; j < polygonCollider2D.GetTotalPointCount(); j++)
        {
            Vector2 actual = points[j] + new Vector2(transform.position.x, transform.position.y);
            lineRenderer.SetPosition(j, new Vector3(actual.x, actual.y, 0));
        }

        if (outline == Outline.Closed)
        {
            Vector2 actual = points[0];
            lineRenderer.SetPosition(pointCount - 1, new Vector3(actual.x, actual.y, 0));
        }
    }


}