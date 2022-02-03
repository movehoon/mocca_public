using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    [Serializable]
    public class LineConnectedEvent : UnityEvent<MCBezierLine>
    {
    }

    [Serializable]
    public class LineRemoveEvent : UnityEvent<int>
    {
    }

    public abstract class MCNodeSocket : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public enum SocketType
        {
            Input, Output, EPLeft, EPRight, None
        }

        protected enum SocketPosition
        {
            Left, Right, None
        }

        public SocketType socketType = SocketType.None;

        protected SocketPosition socketPosition = SocketPosition.None;

        protected RectTransform refRT;

        public MCBezierLine line;

        protected RectTransform graphPane;

        protected Color lineColor = Color.white;

        protected UnityEvent OnLineDelete;
        protected LineRemoveEvent OnRemoveLine;

        private LineConnectedEvent OnLineConnected;

        // 선 드래그 시 화면 벗어날 때 스크롤바 드래그 해주는 기능에 필요한 참조 값들.
        protected bool shouldOverCheck = false;
        private Transform topLeft = null;
        private Transform bottomRight = null;
        private Scrollbar horizontalScrollbar = null;
        private Scrollbar verticalScrollbar = null;
        private float dragSpeed = 0.3f;

        protected bool hasInitialized = false;

        //Alt+클릭으로 링크해제 금지 -kjh
        private bool DontUnlink
        {
            get
            {
                var n = gameObject.GetComponentInParent<MCNode>();
                if (n == null) return false;
                return n.DontUnlink;
            }
        }


        protected virtual void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            if (OnLineDelete is null)
            {
                OnLineDelete = new UnityEvent();
            }

            if (OnRemoveLine is null)
            {
                OnRemoveLine = new LineRemoveEvent();
            }

            if (OnLineConnected is null)
            {
                OnLineConnected = new LineConnectedEvent();
            }

            NodeDrag = GetComponentInParent<MCNodeDrag>();
            Node = GetComponentInParent<MCNode>();
            refRT = GetComponent<RectTransform>();

            GameObject magnetGO = new GameObject("MagneticBound");
            MagneticBound magneticBound = magnetGO.AddComponent<MagneticBound>();
            magnetGO.transform.SetParent(transform);
            magnetGO.transform.localScale = Vector3.one;
            switch (socketType)
            {
                case SocketType.EPLeft:
                    //magneticBound.SetPosition(new Vector2(-25f, 0f));
                    magneticBound.SetPosition(new Vector2(-15f, 0f));
                    break;
                case SocketType.Input:
                    //magneticBound.SetPosition(new Vector2(-25f, 0f));
                    magneticBound.SetPosition(new Vector2(-15f, 0f));
                    magneticBound.inputoutputBase = this as MCNodeInputOutputBase;
                    break;
                case SocketType.EPRight:
                    //magneticBound.SetPosition(new Vector2(25f, 0f));
                    magneticBound.SetPosition(new Vector2(15f, 0f));
                    break;
                case SocketType.Output:
                    //magneticBound.SetPosition(new Vector2(25f, 0f));
                    magneticBound.SetPosition(new Vector2(15f, 0f));
                    magneticBound.inputoutputBase = this as MCNodeInputOutputBase;
                    break;
            }
            magneticBound.Initialize();

            // 드래그를 위한 초기값 설정.
            InitializeWorkspaceDrag();

            hasInitialized = true;
        }

        protected virtual void InitializeWorkspaceDrag()
        {
            // 소켓에서 선을 연결할 때 작업창을 벗어났는지(좌우/상하) 판단하기 위해 사용하는 참조 값.
            DragPane pane = MCWorkspaceManager.Instance.GetDragPane;
            topLeft = pane.topLeftMarker.transform;
            bottomRight = pane.bottomRightMarker.transform;
            horizontalScrollbar = MCWorkspaceManager.Instance.workspaceScrollRect.horizontalScrollbar;
            verticalScrollbar = MCWorkspaceManager.Instance.workspaceScrollRect.verticalScrollbar;
        }

        public abstract bool CheckTargetSocketType(SocketType targetType, MCNodeSocket targetSocket = null);

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            // Test.
            shouldOverCheck = true;

            if (HasLine && KeyInputManager.Instance.isAltPressed)
            {
                //if(DontUnlink == true) return;

                //MCWorkspaceManager.Instance.RequestLineDelete(line.LineID);

                MCUndoRedoManager.Instance.AddCommand(new MCDeleteLineCommand(line.LineID, line.left, line.right, line.color));
                OnLineDelete?.Invoke();

                //----------------------------------------
                // 선연결 해제시 발생하는 튜토리얼이벤트 -kjh
                //----------------------------------------
                //string msg = string.Format($"{targetSocket.Node.nodeData.type},{this.Node.nodeData.type}");
                TutorialManager.SendEvent(Tutorial.CustomEvent.NodeUnlinked);

                return;
            }

            if (HasLine)
            {
                return;
            }

            if (NodeDrag != null)
            {
                NodeDrag.SetEnableDrag(false);
            }

            //GameObject newLine = Instantiate(MCEditorManager.Instance.linePrefab);
            //newLine.transform.SetParent(MCWorkspaceManager.Instance.LineParentTransform, false);
            //newLine.transform.localPosition = Vector3.zero;
            GameObject newLine = Utils.CreateNewLineGO();

            line = newLine.GetComponent<MCBezierLine>();
            line.SetLineColor(lineColor);
        }

        protected virtual void Update()
        {
            if (shouldOverCheck == false)
            {
                return;
            }

            ScrollWorkspace();
        }

        // 소켓에서 선 드래그할 때 작업창 범위 벗어나면 스크롤해주는 함수.
        protected virtual void ScrollWorkspace()
        {
            if (topLeft == null || bottomRight == null)
            {
                InitializeWorkspaceDrag();
            }

            Vector2 mousePosition = Input.mousePosition;
            if (bottomRight.position.x <= mousePosition.x)
            {
                //Utils.LogRed($"오른쪽 벗어남");
                horizontalScrollbar.value += Time.deltaTime * dragSpeed;
            }

            else if (topLeft.position.x >= mousePosition.x)
            {
                //Utils.LogRed($"왼쪽 벗어남");
                horizontalScrollbar.value -= Time.deltaTime * dragSpeed;
            }

            if (topLeft.position.y <= mousePosition.y)
            {
                //Utils.LogRed($"위쪽 벗어남");
                verticalScrollbar.value += Time.deltaTime * dragSpeed;
            }

            else if (bottomRight.position.y >= mousePosition.y)
            {
                //Utils.LogRed($"아래쪽 벗어남");
                verticalScrollbar.value -= Time.deltaTime * dragSpeed;
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (HasLine)
            {
                return;
            }

            if (line)
            {
                Vector2 globalMousePos;
                if (Utils.SetMousePositionToLocalPointInRectangle(eventData, out globalMousePos))
                {
                    Vector2 start = socketPosition == SocketPosition.Left ? globalMousePos : GetSocketPosition;
                    Vector2 end = socketPosition == SocketPosition.Left ? GetSocketPosition : globalMousePos;

                    LinePoint linePoint = new LinePoint();
                    linePoint.start = start;
                    linePoint.end = end;
                    line.UpdateLinePoint(linePoint);
                }
            }
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            // Test.
            shouldOverCheck = false;

            if (HasLine || line == null)
            {
                return;
            }

            if (CheckValidTargetSocket(eventData) == true)
            {
                return;
            }

            HasLine = false;
            Destroy(line.gameObject);
            line = null;
        }

        public virtual void SetLine(MCBezierLine line)
        {
            HasLine = true;
            this.line = line;
        }

        public virtual void RemoveLine(int lineID = -1)
        {
            HasLine = false;
            line = null;

            if (lineID != -1)
            {
                OnRemoveLine?.Invoke(lineID);
            }
        }

        protected bool CheckValidTargetSocket(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            MCEditorManager.Instance.UIRaycaster.Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                MCNodeSocket targetSocket = result.gameObject.GetComponent<MCNodeSocket>();
                MagneticBound magneticBound = result.gameObject.GetComponent<MagneticBound>();
                if (targetSocket != null || magneticBound != null)
                {
                    targetSocket = targetSocket != null ? targetSocket : magneticBound.nodeSocket;

                    //소켓의 enabled 이 꺼져있으면 연결되지 않는다.-kjh
                    if (targetSocket.enabled == false) continue;

                    if (!targetSocket.Node.NodeID.Equals(Node.NodeID)
                        && CheckTargetSocketType(targetSocket.socketType, targetSocket))
                    {

                        //if (socketPosition == SocketPosition.Left)
                        //{
                        //    line.SetLinePoint(targetSocket, this);
                        //}

                        //else if (socketPosition == SocketPosition.Right)
                        //{
                        //    line.SetLinePoint(this, targetSocket);
                        //}

                        //MCWorkspaceManager.Instance.AddLine(line);

                        //LineSet();
                        //targetSocket.LineSet();

                        MCAddLineCommand command = null;
                        if (socketPosition == SocketPosition.Left)
                        {
                            command = new MCAddLineCommand(line, line.LineID, targetSocket, this);
                        }

                        else if (socketPosition == SocketPosition.Right)
                        {
                            command = new MCAddLineCommand(line, line.LineID, this, targetSocket);
                        }

                        MCUndoRedoManager.Instance.AddCommand(command);



                        //----------------------------------------
                        // 선연결시 발생하는 튜토리얼이벤트 -kjh
                        //----------------------------------------
                        //string msg = string.Format($"{this.Node.nodeData.type},{targetSocket.Node.nodeData.type}");
                        //string msg2 = string.Format($"{targetSocket.Node.nodeData.type},{this.Node.nodeData.type}");
                        //string msg3 = string.Format($"{this.Node.name},{targetSocket.Node.name}");
                        //string msg4 = string.Format($"{targetSocket.Node.name},{this.Node.name}");

                        TutorialManager.SendEvent(Tutorial.CustomEvent.NodeLinked, "");
                        //TutorialManager.SendEvent(Tutorial.CustomEvent.NodeLinked, msg);
                        //TutorialManager.SendEvent(Tutorial.CustomEvent.NodeLinked, msg2);
                        //TutorialManager.SendEvent(Tutorial.CustomEvent.NodeLinked, msg3);
                        //TutorialManager.SendEvent(Tutorial.CustomEvent.NodeLinked, msg4);


                        //MCWorkspaceManager.Instance.AddLine(line);
                        //LineSet();
                        //targetSocket.LineSet();

                        return true;
                    }
                }

                //if (targetSocket != null 
                //    && !targetSocket.Node.NodeID.Equals(Node.NodeID)
                //    && CheckTargetSocketType(targetSocket.socketType, targetSocket))
                //{

                //    if (socketPosition == SocketPosition.Left)
                //    {
                //        line.SetLinePoint(targetSocket, this);
                //    }

                //    else if (socketPosition == SocketPosition.Right)
                //    {
                //        line.SetLinePoint(this, targetSocket);
                //    }

                //    MCWorkspaceManager.Instance.AddLine(line);

                //    LineSet();
                //    targetSocket.LineSet();

                //    return true;
                //}
            }

            return false;
        }

        public virtual Vector2 GetSocketPosition
        {
            get { return NodeDrag.GetComponent<RectTransform>().anchoredPosition + refRT.anchoredPosition; }
        }

        public void SubscribeOnLineConnected(UnityAction<MCBezierLine> listener)
        {
            if (OnLineConnected is null)
            {
                OnLineConnected = new LineConnectedEvent();
            }

            OnLineConnected.AddListener(listener);
        }

        public void UnSubscribeOnLineConnected(UnityAction<MCBezierLine> listener)
        {
            OnLineConnected.RemoveListener(listener);
        }

        public void SubscribeOnLineDelete(UnityAction listener)
        {
            if (OnLineDelete is null)
            {
                OnLineDelete = new UnityEvent();
            }
            OnLineDelete.AddListener(listener);
        }

        public void UnSubscribeOnLineDelete(UnityAction listener)
        {
            OnLineDelete.RemoveListener(listener);
        }

        public void SubscribeOnRemoveLine(UnityAction<int> listener)
        {
            if (OnRemoveLine == null)
            {
                OnRemoveLine = new LineRemoveEvent();
            }

            OnRemoveLine.AddListener(listener);
        }

        public void UnSubscribeOnRemoveLine(UnityAction<int> listener)
        {
            if (OnRemoveLine == null)
            {
                OnRemoveLine = new LineRemoveEvent();
            }

            OnRemoveLine.RemoveListener(listener);
        }

        public virtual void LineDeleted()
        {
            OnLineDelete?.Invoke();
            Node.NodeStateChanged();
        }

        public virtual void LineSet()
        {
            OnLineConnected?.Invoke(line);
            Node.NodeStateChanged();
        }

        protected virtual void OnDestroy()
        {
            // 소켓이 삭제될 때 필요한 로직 여기에 추가.
            //if (HasLine && line != null)
            //{
            //    MCWorkspaceManager.Instance.RequestLineDelete(line.LineID);
            //}
        }

        public MCNodeDrag NodeDrag { get; private set; }
        public MCNode Node { get; private set; }
        
        private bool hasLine = false;
        public bool HasLine
        {
            get
            {
                return hasLine;
            }
            set
            {
                //Utils.LogRed($"{Node?.GetType()} MCNodeOutput.HasLine.Set: {value}");
                hasLine = value;
            }
        }
    }
}