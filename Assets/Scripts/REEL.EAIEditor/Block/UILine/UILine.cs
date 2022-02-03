using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct UILinePoint
{
	public Vector2 m_point;

	public UILinePoint(Vector3 p)
	{
		m_point = p;
	}
}

public class UILine : MaskableGraphic, IMeshModifier
{
	[SerializeField]
	List<UILinePoint> m_points = new List<UILinePoint>();

	public float m_width = 1.0f;

	public int Length
	{
		get { return m_points.Count;  }
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

		for (int f=0;f< m_points.Count-1;f++)
		{
			UILineUtil.AddLine(vh , m_points[f].m_point,m_points[f+1].m_point , m_width , color);
		}
	}

	public void Clear()
	{
		m_points.Clear();
		SetVerticesDirty();
	}

	public void AddPoint(Vector2 position)
	{
		m_points.Add(new UILinePoint(position));
		SetVerticesDirty();
	}

	public void RemoveRange(int index , int count)
	{
		m_points.RemoveRange(index, count);
		SetVerticesDirty();
	}


}
