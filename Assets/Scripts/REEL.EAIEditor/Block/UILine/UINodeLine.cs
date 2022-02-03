using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UINodeLine : MaskableGraphic, IMeshModifier
{
	public static GameObject Create( GameObject startNode, GameObject endNode
									, Vector3 startPoint, Vector3 endPoint
									, GameObject parentObject
									, Color color
									, LineModeType lineMode = LineModeType.Bezier
									, float lineWidth = 2.0f)
	{
		GameObject gameObject = new GameObject("NodeLine", typeof(UINodeLine) );

		gameObject.transform.SetParent( parentObject.transform , false );
		gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.zero; ;

		UINodeLine nodeLine = gameObject.GetComponent<UINodeLine>();

		nodeLine.NodeCenter1 = startNode.transform;
		nodeLine.NodeCenter2 = endNode.transform;

		nodeLine.NodeDelta1 = startPoint - startNode.transform.localPosition;
		nodeLine.NodeDelta2 = endPoint - endNode.transform.localPosition;

		nodeLine.color = color;
		nodeLine.LineWidth = lineWidth;

		nodeLine.LineMode = lineMode;

		return gameObject;
	}



	public enum LineModeType
	{
		Line,
		Catmul,
		Bezier,
	};


	[Header("Node 1")]
	public Transform	NodeCenter1;
	public Vector3		NodeDelta1;

	[Header("Node 2")]
	public Transform	NodeCenter2;
	public Vector3		NodeDelta2;

	[Header("Line")]
	public LineModeType LineMode = LineModeType.Line;
	public float		LineWidth = 1;


	[Header("Spline Middle Point")]
	public int			NumSubPoint = 30;
	public float		DistanceRate = 0.5f;
	public float		DistanceMin = 5;
	public float		DistanceMax = 100;



	[Header("Bezier Middle Point")]
	public float BezierWeightX = 0.2f;
	public float BezierWeightY = 0.2f;



	Vector3 t1 = Vector3.zero;
	Vector3 t2 = Vector3.zero;



	Vector3 StartPoint
	{
		get
		{
			return NodeCenter1.localPosition + NodeDelta1; 
		}
	}
	Vector3 EndPoint 
	{
		get
		{
			return NodeCenter2.localPosition + NodeDelta2;
		}
	}

	void Update()
    {
		if( t1 != NodeCenter1.position )
		{
			t1 = NodeCenter1.position;
			SetVerticesDirty();
		}

		if (t2 != NodeCenter2.position)
		{
			t2 = NodeCenter2.position;
			SetVerticesDirty();
		}
	}


	public void ModifyMesh(VertexHelper verts)
	{
		EditMesh(verts);
	}

	public void ModifyMesh(Mesh mesh)
	{
		using (var vh = new VertexHelper(mesh))
		{
			EditMesh(vh);
			vh.FillMesh(mesh);
		}
	}

	void EditMesh(VertexHelper vh)
	{
		vh.Clear();

		switch(LineMode)
		{
			case LineModeType.Line:
				{
					Vector3[] pointList = null;
					pointList = MakeBezierPointList();
					UILineUtil.AddLine(vh, pointList , LineWidth, color);
				}
				break;

			case LineModeType.Catmul:
				{
					Vector3[] pointList = MakeBezierPointList();
					UILineUtil.AddCatmullromSpline(vh, pointList, NumSubPoint, LineWidth, color);
				}
				break;

			case LineModeType.Bezier:
				{
					Vector3[] pointList = null;
					pointList = MakeBezierPointList();

					Vector3[] p = new Vector3[]
						{
							pointList[1],
							pointList[2],
							pointList[3],
							pointList[4]
						};
					UILineUtil.AddBezier(vh, p, NumSubPoint, LineWidth, color);
				}
				break;
		}
	}


	float GetDistance()
	{
		float dist = Vector3.Distance( StartPoint , EndPoint );
		float d = dist * DistanceRate;

		if (d < DistanceMin) d = DistanceMin;
		else if(d > DistanceMax) d = DistanceMax;

		if (d > dist) d = dist;

		return d;
	}


	private Vector3[] MakeCatmullromPointList()
	{
		Vector3 p1 = NodeCenter1.localPosition;
		Vector3 p4 = NodeCenter2.localPosition;

		Vector3 n1 = NodeDelta1.normalized;
		Vector3 n2 = NodeDelta2.normalized;

		float d = GetDistance();

		Vector3[] pointList =
		{
			NodeCenter1.localPosition,

			StartPoint,
			StartPoint + n1 * d,

			EndPoint + n2 * d,
			EndPoint,

			NodeCenter2.localPosition
		};

		return pointList;
	}



	private Vector3[] MakeBezierPointList()
	{
		Vector3 p1 = NodeCenter1.localPosition;
		Vector3 p4 = NodeCenter2.localPosition;

		Vector3 n1 = NodeDelta1;
        Vector3 n2 = NodeDelta2;

        n1.y = 0;
        n2.y = 0;

        n1 = n1.normalized;
        n2 = n2.normalized;

        Vector3 d1 = EndPoint - StartPoint;
		Vector3 d2 = StartPoint - EndPoint;


		d1.x *= BezierWeightX;
		d2.x *= BezierWeightX;

		d1.y *= BezierWeightY;
		d2.y *= BezierWeightY;

		float d = GetDistance();

		Vector3[] pointList =
		{
			NodeCenter1.localPosition,

			StartPoint,
			StartPoint + n1 * d + d1,

			EndPoint + n2 * d + d2,
			EndPoint,

			NodeCenter2.localPosition
		};

		return pointList;
	}

}
