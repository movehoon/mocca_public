#define USINGTMPPRO

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using System;

namespace REEL.D2EEditor
{
    [System.Serializable]
    public struct LinePoint
    {
        public Vector2 start;
        public Vector2 end;

        public LinePoint(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }

        public override string ToString()
        {
            return start.ToString() + ", " + end.ToString();
        }
    }

    public class MCBezierLine : MaskableGraphic, IMeshModifier
    {

		[Header("Texture")]
		[SerializeField]
		Texture m_Texture;

		public override Texture mainTexture
		{
			get
			{
				return m_Texture == null ? s_WhiteTexture : m_Texture;
			}
		}

		public Texture texture
		{
			get
			{
				return m_Texture;
			}
			set
			{
				if(m_Texture == value)
					return;

				m_Texture = value;
				SetVerticesDirty();
				SetMaterialDirty();
			}
		}


		[Header("debug")]
		public bool debugLine = false;			// true = 직선으로 표시
		public bool debugNoReverse = false;     // true = 반전 연결(베지어2개 사용) 사용하지 않음


		[Header("data")]
		[SerializeField] private LinePoint linePoint;

        // For Bezier LIne.
        private Vector2 delta1 = new Vector2(30f, 0f);
        private Vector2 delta2 = new Vector2(-30f, 0f);

        [SerializeField] private float lineWidth = 5f;

        [Header("Spline Middle Point")]
        [SerializeField] private int numSubPoint = 30;
        [SerializeField] private float distanceRate = 0.5f;
        [SerializeField] private float distanceMin = 5f;
        [SerializeField] private float distanceMax = 100f;
        [SerializeField] private float distanceMaxReverse = 70.0f;

		[Header("Bezier Middle Point")]
        [SerializeField] private float bezierWeightX = 0.2f;
        [SerializeField] private float bezierWeightY = 0.2f;


		[Header("Highlight Option")]
		public float highlightDrawTime = 0.0f;	// 마우스 오버 되었을때 값을 true 로 바꿔주면 강조 효과 표시됨 // 마우스 오버 없으면 false로 처리
		const float HIGHLIGHT_SIZE = 5.0f;		// 강조표시 선 두께 값

		const float REVERSE_DIST = 10.0f;	//반전 연결 가로 거리 판별 이 값 보다 가깝거나 작으면 베지어 곡선 2개를 사용한다.
		const float ADJUST_SCALE = 0.8f;    //작업 영역 스케일축소시 두께를 좀더 두껍게 보정 해주는 값

		[Header("Data")]

		public List<Vector2> pointList = new List<Vector2>();

        public MCNodeSocket left;
        public MCNodeSocket right;

        private bool isSet = false;
        private RectTransform refRT;

		private float scale = 1.0f;
		private float lastScale = 1.0f;

		UIVertex[] vertexBuffer = null;
		List<UIVertex> vertexBuffer2 = null;
		List<int> indexBuffer = new List<int>();

		float updateDelay = 0.5f;
		bool isDirty = true;


		enum MiddlePointType
		{
			Normal,			//일반 선(또는 베지어) 연결
			BezierPart1,	//반전연결시 첫번째 베지어 곡선
			BezierPart2,    //반전연결시 두번째 베지어 곡선
		}

#if USINGTMPPRO
		public override void Cull(Rect clipRect, bool validRect)
		{
			//base.Cull(clipRect, validRect);
            //Utils.LogRed($"MCBezierLine.Cull");
        }

        //public override void OnCullingChanged()
        //{
        //    base.OnCullingChanged();
        //    Utils.LogRed($"MCBezierLine.OnCullingChanged");
        //}
#endif

        protected override void OnEnable()
        {
            base.OnEnable();

            refRT = GetComponent<RectTransform>();

            Vector3 canvasScale = MCWorkspaceManager.Instance.CanvasRectTransform.localScale;
            bezierWeightX *= canvasScale.x;
            bezierWeightY *= canvasScale.y;
            lineWidth *= canvasScale.y;

            LineID = Utils.NewGUID;

            MCWorkspaceManager.Instance.PaneObject.SubscribeOnPointerDownWithData(OnPointerDownFromWorkSpace);
            MCWorkspaceManager.Instance.PaneObject.SubscribeOnPointerMoveEvent(OnPointerMove);

            lastScale = transform.lossyScale.x;
        }

		private void Update()
		{

			//스케일 변경 됐을때 두께 조절 처리
			if (lastScale != transform.lossyScale.x)
			{
				isDirty = true;
				lastScale = transform.lossyScale.x;
				scale = 1.0f;
				if(lastScale < 1.0f)
					scale = Mathf.Max(1.0f, 1.0f / lastScale * ADJUST_SCALE);
				//updateDelay = 0.1f;	// 최적화를 위해 0.1초 후에 적용처리
			}


			if(updateDelay != 0.0f )
			{
				//updateDelay -= Time.deltaTime;
				//if(updateDelay < 0) // 0.1초 지나면 mesh 업데이트
				//{
				//	isDirty = true;
				//	updateDelay = 0.0f;
				//}
			}

			if (highlightDrawTime > 0.0f)
			{
				isDirty = true;
				highlightDrawTime -= Time.deltaTime;
			}

			if( isDirty == true )
			{
				UpdateLinePoint(linePoint);
			}

			if (highlightDrawTime <= 0.0f) highlightDrawTime = 0.0f;
		}


		public bool dontChangeSocketInfo = false;
		protected override void OnDestroy()
        {
            base.OnDestroy();

            MCTables tables = FindObjectOfType<MCTables>();

            if (left != null && left.NodeDrag != null)
            {
                left.NodeDrag.UnsubscribeOnChanged(UpdateLine);
                if (dontChangeSocketInfo == false)
                {
                    if (tables != null)
                    {
                        bool shouldRemove = true;
                        foreach (MCBezierLine line in tables.locatedLines)
                        {
                            if (line.left.GetInstanceID().Equals(left.GetInstanceID()))
                            {
                                shouldRemove = false;
                                break;
                            }
                        }

                        if (shouldRemove == true)
                        {
                            left.RemoveLine(LineID);
                            left.LineDeleted();
                            //Utils.LogBlue("hello left");
                        }
                    }
                }
                
                //left.HasLine = false;
                //Utils.LogRed("[MCBezierLine.OnDestroy] 1");
            }

            if (right != null && right.NodeDrag != null)
            {
                right.NodeDrag.UnsubscribeOnChanged(UpdateLine);
                if (dontChangeSocketInfo == false)
                {
                    if (tables != null)
                    {
                        bool shouldRemove = true;
                        foreach (MCBezierLine line in tables.locatedLines)
                        {
                            if (line.right.GetInstanceID().Equals(right.GetInstanceID()))
                            {
                                shouldRemove = false;
                                break;
                            }
                        }

                        if (shouldRemove == true)
                        {
                            right.RemoveLine(LineID);
                            right.LineDeleted();
                            //Utils.LogBlue($"[hello right] right.Node.name: {right.Node.name} LineID: {LineID} / dontChangeSocketInfo: {dontChangeSocketInfo} / right.Node.NodeID: {right.Node.NodeID}");
                        }
                    }

                    //right.RemoveLine(LineID);
                    //Utils.LogBlue($"[hello right] right.Node.name: {right.Node.name} LineID: {LineID} / dontChangeSocketInfo: {dontChangeSocketInfo} / right.Node.NodeID: {right.Node.NodeID}");
                }
                //right.HasLine = false;
                //Utils.LogRed("[MCBezierLine.OnDestroy] 2");
            }

            MCWorkspaceManager.Instance.UnsubscribeLineUpdate(UpdateLine);
            Utils.GetGraphPane().UnSubscribeOnPointerDownWithData(OnPointerDownFromWorkSpace);
            Utils.GetGraphPane().UnSubscribeOnPointerMoveEvent(OnPointerMove);

            //Utils.LogRed("[MCBezierLine.OnDestroy] 3");
        }

        public void UnsubscribeAllDragNodes()
        {
            if (left != null && left.NodeDrag != null)
            {
                left.NodeDrag.UnsubscribeOnChanged(UpdateLine);
                //Utils.LogRed("[MCBezierLine.UnsubscribeAllDragNodes] 1");
            }

            if (right != null && right.NodeDrag != null)
            {
                right.NodeDrag.UnsubscribeOnChanged(UpdateLine);
                //Utils.LogRed("[MCBezierLine.UnsubscribeAllDragNodes] 2");
            }

            Utils.GetGraphPane().UnSubscribeOnPointerDownWithData(OnPointerDownFromWorkSpace);
            Utils.GetGraphPane().UnSubscribeOnPointerMoveEvent(OnPointerMove);
        }

        private bool hasRequestedToLineDelete = false;
        private void UpdateLine()
        {
			if (!isSet)
			{
				return;
			}


			// 소켓이 삭제된 경우에는 라인도 삭제 처리.
			if (left == null || right == null)
            {
                if (hasRequestedToLineDelete == false)
                {
                    hasRequestedToLineDelete = true;

                    bool result = MCWorkspaceManager.Instance.RequestLineDelete(LineID);
                    if (result is false)
                    {
                        Destroy(this);
                        //Debug.Log("DELETE false");
                        //return;
                    }

                    Debug.Log($"[DELETE] LineID: {LineID} / left is null:{left == null} / right is null: {right == null}");
					Utils.GetTables().DelayCheckLineIsValid();

					return;
                }

                if (hasRequestedToLineDelete == true)
                {
                    return;
                }
            }

			linePoint.start = left.GetSocketPosition;
            linePoint.end = right.GetSocketPosition;

			if (refRT == null)
            {
				refRT = GetComponent<RectTransform>();

				if (refRT == null)
                {
					//Utils.LogRed("[MCBezierLine.UpdateLine()]refRT is null");
					return;
                }
			}

			if (refRT != null)
            {
				refRT.anchoredPosition = Vector2.zero;

				UpdateLinePoint(linePoint);
			}

            //refRT.anchoredPosition = Vector2.zero;

            //UpdateLinePoint(linePoint);
        }

        public void UpdateLinePoint(LinePoint linePoint)
        {
			isDirty = true;
			this.linePoint = linePoint;
            SetVerticesDirty();
        }

        public void ForceToDraw()
        {
			isDirty = true;
			SetVerticesDirty();
        }

        public void SetLineColor(Color color)
        {
			isDirty = true;
            this.color = color;
        }

        public void SetLinePoint(MCNodeSocket left, MCNodeSocket right)
        {
            this.left = left;
            this.right = right;

            isSet = true;

            //left.HasLine = true;
            left.SetLine(this);
            left.NodeDrag.SubscribeOnChanged(UpdateLine);

            //right.HasLine = true;
            right.SetLine(this);
            right.NodeDrag.SubscribeOnChanged(UpdateLine);

            MCWorkspaceManager.Instance.SubscribeLineUpdate(UpdateLine);
            UpdateLine();
        }

		VertexHelper m_vertexHelper = null;

		public void ModifyMesh(Mesh mesh)
		{
			if (m_vertexHelper == null)
			{
				m_vertexHelper = new VertexHelper(mesh);
			}

			EditMesh2(m_vertexHelper);
			//UpdateMesh(m_vertexHelper);
			m_vertexHelper.FillMesh(mesh);
		}

		public void ModifyMesh(VertexHelper vertexHelper)
		{
		}


		bool CheckReverse()
		{
			Vector2 p1 = linePoint.start;
			Vector2 p4 = linePoint.end;

			return p4.x-p1.x < REVERSE_DIST;
		}

		/* 최적화 테스트 코드

		private void CheckVertexSize()
		{
			if (vertexBuffer == null ||
				vertexBuffer.Length != (numSubPoint + 1) * 4)
			{
				vertexBuffer = new UIVertex[(numSubPoint + 1) * 4];
				indexBuffer = new List<int>();

				for (int f = 0; f < vertexBuffer.Length; f++)
				{
					UIVertex vertex = UIVertex.simpleVert;
					vertex.color = color;

					vertexBuffer[f] = vertex;
				}

				for(int f=0;f< (numSubPoint + 1) * 6;f++)
				{
					indexBuffer.Add(0);
				}

			}

			//if (vertexBuffer == null ||
			//	vertexBuffer.Count!= (numSubPoint + 1) * 4)
			//{
			//	vertexBuffer = new  List<UIVertex>();
			//	indexBuffer = new int[(numSubPoint + 1) * 6];

			//	for (int f = 0; f < vertexBuffer.Count; f++)
			//	{
			//		vertexBuffer[f] = UIVertex.simpleVert;

			//		UIVertex
			//	}
			//}

		}


		bool UpdateMesh(VertexHelper vertexHelper)
		{
			if (isDirty == false)
				return false;

			isDirty = false;

			vertexHelper.Clear();
			Vector2[] p0 = MakeBezierPointList(MiddlePointType.Normal);

			CheckVertexSize();

			MakeVertexBufferBezier(vertexHelper,p0);

			return true;
		}


		private void MakeVertexBufferBezier(VertexHelper vertexHelper,Vector2[] points)
		{
			int vbIndex = 0;
			int ibIndex = 0;

			Vector3 p0 = points[0];
			Vector3 p1;

			UIVertex vertex = UIVertex.simpleVert;
			vertex.color = color;

			for (int ix = 0; ix <= numSubPoint; ++ix)
			{
				p1 = Utils.CalculateCubicBezierPoint(1f / numSubPoint * ix, points[0], points[1], points[2], points[3]);

				Vector3 crossVector = p1 - p0;
				Vector3 normal = Vector3.Cross(crossVector, new Vector3(0f, 0f, 1f));
				normal.Normalize();

				Vector3 n = normal * lineWidth * scale * 0.5f;

				vertex.position = p0 + n;
				vertexBuffer[vbIndex+0] = vertex;

				vertex.position = p0 - n;
				vertexBuffer[vbIndex+1] = vertex;

				vertex.position = p1 - n;
				vertexBuffer[vbIndex+2] = vertex;

				vertex.position = p1 + n;
				vertexBuffer[vbIndex+3] = vertex;

				indexBuffer[ibIndex+0] = vbIndex;
				indexBuffer[ibIndex+1] = vbIndex+1;
				indexBuffer[ibIndex+2] = vbIndex+2;
				indexBuffer[ibIndex+3] = vbIndex;
				indexBuffer[ibIndex+4] = vbIndex+2;
				indexBuffer[ibIndex+5] = vbIndex+3;

				vbIndex += 4;
				ibIndex += 6;
				p0 = p1;
			}

			vertexHelper.AddUIVertexStream(vertexBuffer.ToList(), indexBuffer );
		}
		*/

		void EditMesh2(VertexHelper vertexHelper)
        {
			if ( isDirty == false) return;
			isDirty = false;

			vertexHelper.Clear();

			if (debugLine == true)
			{
				Vector2[] p0 = MakeBezierPointList(MiddlePointType.Normal);
				Vector2[] p1 = MakeBezierPointList(MiddlePointType.BezierPart1);
				Vector2[] p2 = MakeBezierPointList(MiddlePointType.BezierPart2);

				if (CheckReverse() && debugNoReverse == false)
				{
					pointList = Utils.AddLineList(vertexHelper, p1.ToList(), lineWidth * scale, color);
					pointList.AddRange(Utils.AddLineList(vertexHelper, p2.ToList(), lineWidth * scale, color));
				}
				else
				{
					pointList = Utils.AddLineList(vertexHelper, p0.ToList(), lineWidth * scale, color);
				}
			}
			else
			{
				if (CheckReverse() && debugNoReverse == false)
				{
					Vector2[] p1 = MakeBezierPointList(MiddlePointType.BezierPart1);
					Vector2[] p2 = MakeBezierPointList(MiddlePointType.BezierPart2);

					Color c = color;
					c.a = 1.0f;

					if(highlightDrawTime > 0.0f)
					{
						Utils.AddBezier(vertexHelper, p1, numSubPoint, (lineWidth + HIGHLIGHT_SIZE) * scale, c, true);
						Utils.AddBezier(vertexHelper, p2, numSubPoint, (lineWidth + HIGHLIGHT_SIZE) * scale, c, true);
					}

					pointList = Utils.AddBezier(vertexHelper, p1, numSubPoint, lineWidth * scale, color);
					pointList.AddRange(Utils.AddBezier(vertexHelper, p2, numSubPoint, lineWidth * scale, color));
				}
				else
				{
					Vector2[] p0 = MakeBezierPointList(MiddlePointType.Normal);

					Color c = color;
					c.a = 1.0f;

					if(highlightDrawTime > 0.0f)
					{
						Utils.AddBezier(vertexHelper, p0, numSubPoint, (lineWidth + HIGHLIGHT_SIZE) * scale, c,true);
					}

					pointList = Utils.AddBezier(vertexHelper, p0, numSubPoint, lineWidth * scale, color);
				}
			}
		}


		private Vector2[] MakeBezierPointList(MiddlePointType type)
		{
			Vector2 p1 = linePoint.start;
			Vector2 p4 = linePoint.end;

			Vector2 n1 = delta1;
			Vector2 n2 = delta2;

			n1.y = n2.y = 0f;

			n1.Normalize();
			n2.Normalize();

			Vector2 d1 = p4 - p1;
			Vector2 d2 = p1 - p4;

			Vector2 dy = p4 - p1;
			dy.x = 0f;


			d1.x *= bezierWeightX;
			d2.x *= bezierWeightX;

			d1.y *= bezierWeightY;
			d2.y *= bezierWeightY;

			float d = 0.0f;

			switch (type)
			{
				case MiddlePointType.Normal:
					{
						d = GetDistance(p1, p4);
						Vector2[] point =
						{
							p1,
							p1 + n1 * d+ d1,
							p4 + n2 * d+ d2,
							p4
						};
						return point;
					}

				case MiddlePointType.BezierPart1:
					{
						d = GetDistance(p1, p4, true);
						Vector2[] point =
						{
							p1,
							p1 + n1 * d,
							(p1 + n1 * d) + dy*0.5f,
							(p1 + p4 ) * 0.5f,
						};
						return point;
					}

				case MiddlePointType.BezierPart2:
					{
						d = GetDistance(p1, p4, true);
						Vector2[] point =
						{
							(p1 + p4 ) * 0.5f,
							(p4 + n2 * d) - dy*0.5f,
							p4 + n2 * d,
							p4
						};
						return point;
					}

			}

			return null;
		}


        float GetDistance(Vector2 StartPoint, Vector2 EndPoint,bool reverse = false)
        {
            float dist = Vector2.Distance(StartPoint, EndPoint);
            float d = dist * distanceRate;

            d = Mathf.Clamp(d, distanceMin, reverse ? distanceMaxReverse : distanceMax );
            if (d > dist) d = dist;

            return d;
        }

        public int LineID { get; private set; }


		//segment vs circle 충돌 판별 함수
		public static bool IntersectLineCircle(Vector2 location, float radius, Vector2 lineFrom, Vector2 lineTo)
		{
			if (lineFrom == lineTo) return false;

			float ab2, acab, h2;
			Vector2 ac = location - lineFrom;
			Vector2 ab = lineTo - lineFrom;
			ab2 = Vector2.Dot(ab, ab);
			acab = Vector2.Dot(ac, ab);
			float t = acab / ab2;

			if (t < 0)	t = 0;
			else if (t > 1)	t = 1;

			Vector2 h = (ab * t) + lineFrom - location;
			h2 = Vector2.Dot(h, h);

			return h2 <= (radius * radius);
		}

		public static MCBezierLine GetHitLine(Vector2 point , float radius = 5.0f)
		{
			var lines = FindObjectsOfType<MCBezierLine>();

			if (lines == null || lines.Length == 0) return null;

			var line = lines.FirstOrDefault(x => x.HitTest(point));

			return line;
		}

		private bool ContainsLinePoint(Vector2 point, LinePoint linePoint,int extend)
		{
			//size를 10확장 한다.
			Rect aabb = Rect.MinMaxRect(Mathf.Min(linePoint.start.x, linePoint.end.x) - extend,
							Mathf.Min(linePoint.start.y, linePoint.end.y) - extend,
							Mathf.Max(linePoint.start.x, linePoint.end.x) + extend,
							Mathf.Max(linePoint.start.y, linePoint.end.y) + extend);

			return aabb.Contains(point);
		}

		public bool HitTest(Vector2 point, float radius = 5.0f)
		{
			int ext = 10;

			if( CheckReverse() )
			{
				ext = (int)distanceMaxReverse;
			}

			if( ContainsLinePoint(point, linePoint,ext) == false ) return false;

			for (int f = 0; f < pointList.Count - 1; f++)
			{
				if (IntersectLineCircle(point, radius, pointList[f], pointList[f + 1]) == true )
				{
					return true;
				}
			}

			return false;
		}


		//강조 효과 ON
		public void HighlightOn(float time = 0.3f)
		{
			highlightDrawTime = time;
		}


		//강조 효과 OFF
		public void HighlightOff()
		{
			if(highlightDrawTime != 0.0f )
			{
				highlightDrawTime = 0.01f;
			}
		}


		public bool HitTest(PointerEventData data , float radius = 5.0f)
		{
			if (transform == null || transform.parent == null)
			{
				return false;
			}

			Vector2 outlocalPos = Vector3.zero;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), data.position, null, out outlocalPos) == false)
				return false;

			return HitTest(outlocalPos);
		}


		// 작업창 클릭 이벤트 받는 함수(이벤트 함수).
		private void OnPointerDownFromWorkSpace(PointerEventData data)
        {
			if( HitTest(data) == true )
			{
				//충돌 체크 되었다.
                // 라인 삭제.
                if (KeyInputManager.Instance.isAltPressed)
                {
					//튜토리얼에서 링크 해제 가능 할때만  해제 한다.
					if(left && left.Node && right && right.Node)
					{
						if(left.Node.DontUnlink == true && right.Node.DontUnlink == true) return;
					}

					//MCWorkspaceManager.Instance.RequestLineDelete(LineID);
					MCUndoRedoManager.Instance.AddCommand(new MCDeleteLineCommand(LineID, left, right, color));
                }
			}
		}

        // 마우스가 작업창 안에 있을 때 마우스 위치 전달해주는 함수.
        private void OnPointerMove(PointerEventData data)
        {
            if (HitTest(data) == true)
            {
                HighlightOn();
            }

            //Utils.LogRed($"data.position: {data.position} / mouse Position: {UnityEngine.Input.mousePosition}");
        }
    }
}