using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineUtil
{
	static public void AddLine(VertexHelper vh, Vector3 p0, Vector3 p1, float width , Color color)
	{
		Vector3 crossVector = p1 - p0;
		Vector3 normal = Vector3.Cross(crossVector, new Vector3(0, 0, 1));
		normal.Normalize();

		UIVertex[] verts = new UIVertex[4];

		verts[0] = UIVertex.simpleVert;
		verts[1] = UIVertex.simpleVert;
		verts[2] = UIVertex.simpleVert;
		verts[3] = UIVertex.simpleVert;

		Vector3 n = normal * width * 0.5f;

		verts[0].color = color;
		verts[1].color = color;
		verts[2].color = color;
		verts[3].color = color;

		verts[0].position = p0 + n;
		verts[1].position = p0 - n;

		verts[2].position = p1 - n;
		verts[3].position = p1 + n;

		vh.AddUIVertexQuad(verts);
	}

	static public void AddLine(VertexHelper vh, Vector3[] pointList , float width, Color color)
	{
		for (int f = 0; f < pointList.Length - 1; f++)
		{
			UILineUtil.AddLine(vh, pointList[f], pointList[f+1], width, color);
		}
	}


	static public void AddBezier(VertexHelper vh, Vector3[] points, int numPoints, float width, Color color)
	{
		if (points.Length < 4)
			return;

		var pointList = new List<Vector3>();

		for (int j = 0; j < numPoints+1; j++)
		{
			pointList.Add(GetBezierPoint(points[0], points[1], points[2], points[3], 1f / numPoints * j) );
		}

		AddLine(vh, pointList.ToArray(), width, color);
	}


	static public void AddCatmullromSpline(VertexHelper vh, Vector3[] points,int numPoints, float width, Color color)
	{
		if (points.Length < 4)	
			return;

		var pointList = new List<Vector3>();

		for (int i = 0; i < points.Length - 3; i++)
		{
			for (int j = 0; j < numPoints; j++)
			{
				pointList.Add(GetCatmullromPoint(points[i], points[i + 1], points[i + 2], points[i + 3], 1f / numPoints * j));
			}
		}

		pointList.Add(points[points.Length - 2]);

		AddLine(vh, pointList.ToArray(), width, color);
	}



	public static Vector3 GetBezierPoint(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint,  float t)
	{
		float x = (1 - t) * (1 - t) * (1 - t) * startPoint.x +
					3 * t * (1 - t) * (1 - t) * controlPoint1.x +
					3 * t * t * (1 - t) * controlPoint2.x +
					t * t * t * endPoint.x;
		float y = (1 - t) * (1 - t) * (1 - t) * startPoint.y +
					3 * t * (1 - t) * (1 - t) * controlPoint1.y +
					3 * t * t * (1 - t) * controlPoint2.y +
					t * t * t * endPoint.y;

		return new Vector3(x,y,0);
	}


	public static Vector3 GetCatmullromPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		float t2 = t * t;
		float t3 = t2 * t;

		float x = 0.5f * ((2.0f * p1.x) +
					(-p0.x + p2.x) * t +
					(2.0f * p0.x - 5.0f * p1.x + 4 * p2.x - p3.x) * t2 +
					(-p0.x + 3.0f * p1.x - 3.0f * p2.x + p3.x) * t3);

		float y = 0.5f * ((2.0f * p1.y) +
					(-p0.y + p2.y) * t +
					(2.0f * p0.y - 5.0f * p1.y + 4 * p2.y - p3.y) * t2 +
					(-p0.y + 3.0f * p1.y - 3.0f * p2.y + p3.y) * t3);

		return new Vector3(x, y, 0);
	}




}
