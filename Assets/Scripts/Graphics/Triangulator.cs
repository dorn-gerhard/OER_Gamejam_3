// Author: runevision
// URL: http://wiki.unity3d.com/index.php?title=Triangulator
// Version: http://wiki.unity3d.com/index.php?title=Triangulator&oldid=14208
//
// Then modified a bit. -NC
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Triangulator
{
	private List<Vector2> m_points;
	public Triangulator(Vector2[] points)
	{
		m_points = new List<Vector2>(points);
	}
	public Triangulator(Vector3[] points)
	{
		m_points = points.Select(vertex => new Vector2(vertex.x, vertex.y)).ToList();
	}
	public static bool Triangulate(Vector3[] vertices, int[] indices, int indexOffset = 0, int vertexOffset = 0, int numVertices = 0)
	{
		if (numVertices == 0)
			numVertices = vertices.Length;
		if (numVertices < 3)
			return false;
		var workingIndices = new int[numVertices];
		if (Area(vertices, vertexOffset, numVertices) > 0)
		{
			for (int v = 0; v < numVertices; v++)
				workingIndices[v] = v;
		}
		else
		{
			for (int v = 0; v < numVertices; v++)
				workingIndices[v] = (numVertices - 1) - v;
		}
		int nv = numVertices;
		int count = 2 * nv;
		int currentIndex = indexOffset;
		for (int m = 0, v = nv - 1; nv > 2;)
		{
			if (count-- <= 0)
				return false;
			int u = v;
			if (nv <= u)
				u = 0;
			v = u + 1;
			if (nv <= v)
				v = 0;
			int w = v + 1;
			if (nv <= w)
				w = 0;
			if (Snip(vertices, u, v, w, nv, workingIndices))
			{
				indices[currentIndex++] = workingIndices[u];
				indices[currentIndex++] = workingIndices[v];
				indices[currentIndex++] = workingIndices[w];
				m++;
				for (int s = v, t = v + 1; t < nv; s++, t++)
					workingIndices[s] = workingIndices[t];
				nv--;
				count = 2 * nv;
			}
		}
		return true;
	}
	public static float Area(Vector3[] vertices, int vertexOffset = 0, int numVertices = 0)
	{
		if (numVertices == 0)
			numVertices = vertices.Length;
		float area = 0.0f;
		for (int p = vertexOffset + numVertices - 1, q = 0; q < numVertices; p = q++)
			area += vertices[p].x * vertices[q].y - vertices[q].x * vertices[p].y;
		return area * 0.5f;
	}
	private static bool Snip(Vector3[] vertices, int u, int v, int w, int n, int[] workingIndices)
	{
		Vector2 A = vertices[workingIndices[u]];
		Vector2 B = vertices[workingIndices[v]];
		Vector2 C = vertices[workingIndices[w]];
		if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
			return false;
		for (int p = 0; p < n; p++)
		{
			if ((p == u) || (p == v) || (p == w))
				continue;
			Vector2 P = vertices[workingIndices[p]];
			if (InsideTriangle(A, B, C, P))
				return false;
		}
		return true;
	}
	public int[] Triangulate()
	{
		var indices = new List<int>();
		int n = m_points.Count;
		if (n < 3)
			return indices.ToArray();
		var V = new int[n];
		if (Area() > 0)
		{
			for (int v = 0; v < n; v++)
				V[v] = v;
		}
		else
		{
			for (int v = 0; v < n; v++)
				V[v] = (n - 1) - v;
		}
		int nv = n;
		int count = 2 * nv;
		for (int m = 0, v = nv - 1; nv > 2;)
		{
			if (count-- <= 0)
				return indices.ToArray();
			int u = v;
			if (nv <= u)
				u = 0;
			v = u + 1;
			if (nv <= v)
				v = 0;
			int w = v + 1;
			if (nv <= w)
				w = 0;
			if (Snip(u, v, w, nv, V))
			{
				int a = V[u];
				int b = V[v];
				int c = V[w];
				indices.Add(a);
				indices.Add(b);
				indices.Add(c);
				m++;
				for (int s = v, t = v + 1; t < nv; s++, t++)
					V[s] = V[t];
				nv--;
				count = 2 * nv;
			}
		}
		//		indices.Reverse();
		return indices.ToArray();
	}
	private float Area()
	{
		int n = m_points.Count;
		float A = 0.0f;
		for (int p = n - 1, q = 0; q < n; p = q++)
		{
			Vector2 pval = m_points[p];
			Vector2 qval = m_points[q];
			A += pval.x * qval.y - qval.x * pval.y;
		}
		return A * 0.5f;
	}
	private bool Snip(int u, int v, int w, int n, int[] V)
	{
		Vector2 A = m_points[V[u]];
		Vector2 B = m_points[V[v]];
		Vector2 C = m_points[V[w]];
		if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
			return false;
		for (int p = 0; p < n; p++)
		{
			if ((p == u) || (p == v) || (p == w))
				continue;
			Vector2 P = m_points[V[p]];
			if (InsideTriangle(A, B, C, P))
				return false;
		}
		return true;
	}
	private static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
	{
		float ax = C.x - B.x;
		float ay = C.y - B.y;
		float bx = A.x - C.x;
		float by = A.y - C.y;
		float cx = B.x - A.x;
		float cy = B.y - A.y;
		float apx = P.x - A.x;
		float apy = P.y - A.y;
		float bpx = P.x - B.x;
		float bpy = P.y - B.y;
		float cpx = P.x - C.x;
		float cpy = P.y - C.y;
		float aCROSSbp = ax * bpy - ay * bpx;
		float cCROSSap = cx * apy - cy * apx;
		float bCROSScp = bx * cpy - by * cpx;
		return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
	}
}