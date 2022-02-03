using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
	public class MCNodeStart : MCNodeSocket
	{
        //private HashSet<MCBezierLine> lines = new HashSet<MCBezierLine>();
        private List<MCBezierLine> lines = new List<MCBezierLine>();

        //Alt+클릭으로 링크해제 금지 -kjh
        private bool DontUnlink
        {
            get
            {
                var n = gameObject.GetComponentInParent<MCNode>();
                if(n == null) return false;
                return n.DontUnlink;
            }
        }


        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();
            socketPosition = SocketPosition.Left;
            socketType = SocketType.EPLeft;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            // Test.
            shouldOverCheck = true;

            if (HasLine && KeyInputManager.Instance.isAltPressed)
            {
				if(DontUnlink == true) return;

				//Utils.LogRed("[MCNodeStart.OnPointerDown]");
				//foreach (var line in lines)
				//{
				//    //MCWorkspaceManager.Instance.RequestLineDelete(line.LineID);

				//    MCUndoRedoManager.Instance.AddCommand(new MCDeleteLineCommand(line.LineID, line.left, line.right, line.color));
				//}
				for(int ix = lines.Count - 1; ix >= 0; --ix)
                {
                    MCBezierLine line = lines[ix];
                    MCUndoRedoManager.Instance.AddCommand(new MCDeleteLineCommand(line.LineID, line.left, line.right, line.color));
                }

                OnLineDelete?.Invoke();

                //----------------------------------------
                // 선연결 해제시 발생하는 튜토리얼이벤트 -kjh
                //----------------------------------------
                //string msg = string.Format($"{targetSocket.Node.nodeData.type},{this.Node.nodeData.type}");
                TutorialManager.SendEvent(Tutorial.CustomEvent.NodeUnlinked);

                return;
            }

            if (NodeDrag != null)
            {
                NodeDrag.SetEnableDrag(false);
            }

            GameObject newLine = Utils.CreateNewLineGO();

            line = newLine.GetComponent<MCBezierLine>();
            line.SetLineColor(lineColor);
        }

        public override void OnDrag(PointerEventData eventData)
        {
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

        public override void OnPointerUp(PointerEventData eventData)
        {
            // Test.
            shouldOverCheck = false;

            if (line == null)
            {
                return;
            }

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
                    if(targetSocket.enabled == false) continue;

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

                        //lines.Add(line);
                        //line = null;

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

                        //MCWorkspaceManager.Instance.AddLine(line);
                        //LineSet();
                        //targetSocket.LineSet();

                        //----------------------------------------
                        // 선연결시 발생하는 튜토리얼이벤트 -kjh
                        //----------------------------------------
                        //string msg = string.Format($"{targetSocket.Node.nodeData.type},{this.Node.nodeData.type}");
                        //string msg2 = string.Format($"{this.Node.nodeData.type},{targetSocket.Node.nodeData.type}");
                        TutorialManager.SendEvent(Tutorial.CustomEvent.NodeLinked, "");
                        //TutorialManager.SendEvent(Tutorial.CustomEvent.NodeLinked, msg);
                        //TutorialManager.SendEvent(Tutorial.CustomEvent.NodeLinked, msg2);

                        lines.Add(line);
                        line = null;

                        return;
                    }
                }   
            }

            if (lines.Count == 0)
            {
                ResetLineInfo();
            }
            
            Destroy(line.gameObject);
            line = null;
        }

        public override void SetLine(MCBezierLine line)
        {
            this.line = line;
            HasLine = true;
            if (!IsLineSame(line))
            {
                lines.Add(line);
            }
        }

        public override void RemoveLine(int lineID = -1)
        {
            foreach (var line in lines)
            {
                if (line.LineID.Equals(lineID))
                {
                    lines.Remove(line);
                    OnRemoveLine?.Invoke(lineID);
                    break;
                }
            }

            if (lines.Count == 0)
            {
                ResetLineInfo();
            }
        }

        public override void LineDeleted()
        {
            //OnLineDelete?.Invoke();
            base.LineDeleted();
            if (lines.Count == 0)
            {
                ResetLineInfo();
            }
        }

        private void ResetLineInfo()
        {
            HasLine = false;

            MCNode.visitedNode.Clear();
            Node.OnProcessInformationReset();
        }

        private bool IsLineSame(MCBezierLine targetLine)
        {
            foreach (var line in lines)
            {
                if (line.LineID.Equals(targetLine.LineID))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool CheckTargetSocketType(SocketType targetType, MCNodeSocket targetSocket = null)
        {
            if (targetSocket == null)
            {
                return false;
            }

            if (targetType == SocketType.EPRight && !targetSocket.HasLine)
            {
                return true;
            }

            return false;
        }
    }
}