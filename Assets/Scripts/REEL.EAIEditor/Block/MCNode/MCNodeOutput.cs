using REEL.PROJECT;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class MCNodeOutput : MCNodeInputOutputBase
    {
        public Output output;

        private UnityEvent OnOutputConnected;

        //public HashSet<MCBezierLine> lines = new HashSet<MCBezierLine>();
        public List<MCBezierLine> lines = new List<MCBezierLine>();

        public MCBezierLine currentLine = null;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            if (OnOutputConnected is null)
            {
                OnOutputConnected = new UnityEvent();
            }

            socketPosition = SocketPosition.Right;
            socketType = SocketType.Output;

            Node.SubscribeOnNodeStateChanged(OnNodeStateChanged);
        }

        protected virtual void OnDisable()
        {
            Node.UnsubscribeOnNodeStateChanged(OnNodeStateChanged);
        }

        public void SetOutputData(PROJECT.Output output)
        {
            if (this.output is null)
            {
                this.output = new PROJECT.Output();
            }

            this.output.id = output.id;
            this.output.type = output.type;
            this.output.value = output.value;
            this.output.name = output.name;

            parameterType = output.type;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            // Test.
            shouldOverCheck = true;

            if (HasLine && KeyInputManager.Instance.isAltPressed)
            {
                for (int ix = lines.Count - 1; ix >= 0; --ix)
                {
                    MCBezierLine line = lines[ix];
                    MCUndoRedoManager.Instance.AddCommand(new MCDeleteLineCommand(line.LineID, line.left, line.right, line.color));
                }

                //foreach (var line in lines)
                //{
                //    //MCWorkspaceManager.Instance.RequestLineDelete(line.LineID);

                //    MCUndoRedoManager.Instance.AddCommand(new MCDeleteLineCommand(line.LineID, line.left, line.right, line.color));
                //}

                OnLineDelete?.Invoke();
                return;
            }

            if (NodeDrag != null)
            {
                NodeDrag.SetEnableDrag(false);
            }

            GameObject newLine = Utils.CreateNewLineGO();

            currentLine = newLine.GetComponent<MCBezierLine>();
            currentLine.SetLineColor(lineColor);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            //base.OnDrag(eventData);

            if (currentLine)
            {
                Vector2 globalMousePos;
                if (Utils.SetMousePositionToLocalPointInRectangle(eventData, out globalMousePos))
                {
                    Vector2 start = socketPosition == SocketPosition.Left ? globalMousePos : GetSocketPosition;
                    Vector2 end = socketPosition == SocketPosition.Left ? GetSocketPosition : globalMousePos;

                    LinePoint linePoint = new LinePoint();
                    linePoint.start = start;
                    linePoint.end = end;
                    currentLine.UpdateLinePoint(linePoint);
                }
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            // Test.
            shouldOverCheck = false;

            if (currentLine == null)
            {
                return;
            }

            List<RaycastResult> results = new List<RaycastResult>();
            MCEditorManager.Instance.UIRaycaster.Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                MCNodeInputOutputBase targetSocket = result.gameObject.GetComponent<MCNodeInputOutputBase>();
                MagneticBound magneticBound = result.gameObject.GetComponent<MagneticBound>();
                if (targetSocket != null || (magneticBound != null && magneticBound.inputoutputBase))
                {
                    targetSocket = targetSocket != null ? targetSocket : magneticBound.inputoutputBase;

                    //소켓의 enabled 이 꺼져있으면 연결되지 않는다.-kjh
                    if(targetSocket.enabled == false) continue;

                    if (!targetSocket.Node.NodeID.Equals(Node.NodeID)
                        && CheckTargetSocketType(targetSocket.socketType, targetSocket)
                        && !targetSocket.HasLine)
                    {
                        //currentLine.SetLinePoint(this, targetSocket);
                        //MCWorkspaceManager.Instance.AddLine(currentLine);
                        //LineSet();
                        //targetSocket.LineSet();

                        //lines.Add(currentLine);
                        //currentLine = null;


                        MCUndoRedoManager.Instance.AddCommand(
                            new MCAddLineCommand(currentLine, currentLine.LineID, this, targetSocket)
                        );

                        currentLine = null;

                        return;
                    }
                }
            }

            if (lines.Count == 0)
            {
                HasLine = false;
            }

            Destroy(currentLine.gameObject);
            currentLine = null;
        }

        public void SetOutputIndex(int index)
        {
            output.id = index;
        }

        //public void SetOutputType(Utils.ParameterType type)
        //{
        //    output.type = type;
        //}

        public override void SetLine(MCBezierLine line)
        {
            //base.SetLine(line);
            HasLine = true;
            if (IsLineSame(line) == false)
            {
                lines.Add(line);
                
                // Line이 삭제되면 MissingObject 상태가 되고 이 값이 lines에 남아있는 경우가 있음.
                // 이를 확인해 제거하기 위해 라인이 유효한지 검사 line == null로는 체크안됨.
                // MCTables.locatedLines 배열에 추가하는 작업이 완료된 다음에 검사해야해서 Invoke 사용함.
                // MCAddLineCommand 참고.
                Invoke("ValidateLines", 0.1f);
            }
        }

        public void ValidateLines()
        {
            MCTables tables = FindObjectOfType<MCTables>();
            for (int ix = lines.Count - 1; ix >= 0; --ix)
            {
                MCBezierLine found = tables.FindLineWithID(lines[ix].LineID);
                if (found is null)
                {
                    //Debug.Log("line delete");
                    lines.RemoveAt(ix);
                }
            }
        }

        public override void RemoveLine(int lineID = -1)
        {
            //base.RemoveLine(line);

            if (lineID.Equals(-1))
            {
                lines.Clear();
            }
            else
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
            }

            if (lines.Count == 0)
            {
                HasLine = false;
                ChangeSpriteToNomal();
                //output.type = REEL.PROJECT.DataType.NONE;
                output.value = string.Empty;
            }
        }

        public override void LineDeleted()
        {
            //base.LineDeleted();
            OnLineDelete?.Invoke();
            Node.NodeStateChanged();
            if (lines.Count == 0)
            {
                HasLine = false;
                ChangeSpriteToNomal();
            }

            //----------------------------------------
            // 출력 파라미터에 선 연결 해제됐을 때 발생하는 튜토리얼이벤트 -kjh
            //----------------------------------------
            TutorialManager.SendEvent(Tutorial.CustomEvent.ParameterUnlinked);
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

        private void OnNodeStateChanged()
        {
            // Get 노드에서 Dropdown 값이 바뀌면 이를 반영해 선 연결을 유지할 지 확인해야 함.
            if (Node is MCGetNode)
            {
                if (HasLine == true && line != null && line.left != null && line.right != null)
                {
                    MCNodeSocket right = line.right;
                    if (CheckTargetSocketType(right.socketType, right) == false)
                    {
                        MCDeleteLineCommand command = new MCDeleteLineCommand(line.LineID, this, right, line.color);
                        MCUndoRedoManager.Instance.AddCommand(command);
                    }
                }
            }
        }

        public override void LineSet()
        {
            base.LineSet();

            OnOutputConnected?.Invoke();
        }

        public void SubscribeOnOutputConnected(UnityAction OnConnected)
        {
            OnOutputConnected.AddListener(OnConnected);
        }

        public void UnSubscribeOnOutputConnected(UnityAction OnConnected)
        {
            OnOutputConnected.RemoveListener(OnConnected);
        }

        //public override void RemoveLine(MCBezierLine line = null)
        //{
        //    base.RemoveLine(line);

        //    output.type = REEL.PROJECT.DataType.NONE;
        //    output.value = string.Empty;
        //}

        public override bool CheckTargetSocketType(SocketType targetType, MCNodeSocket targetSocket = null)
        {
            #region MCGetNode인 경우
            if (Node is MCGetNode)
            {
                MCGetNode getNode = Node as MCGetNode;
                LeftMenuVariableItem variable;
                if (getNode.IsLocalVariable == true)
                {
                    variable = MCWorkspaceManager.Instance.GetLocalVariable(getNode.CurrentVariableIndex);
                }
                else
                {
                    variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
                }
                

                // Choice 블록 예외 처리.
                if (targetSocket.Node is MCChoiceNode)
                {
                    if (targetSocket.GetComponent<MCNodeInput>().parameterType == DataType.LIST)
                    {
                        if (variable.nodeType == NodeType.LIST && variable.dataType == DataType.STRING)
                        {
                            return true;
                        }

                        return false;
                    }

                    else if (targetSocket.GetComponent<MCNodeInput>().parameterType == DataType.STRING)
                    {
                        if (variable.nodeType != NodeType.LIST && variable.dataType == DataType.STRING)
                        {
                            return true;
                        }

                        return false;
                    }
                }

                if (targetSocket.GetComponent<MCNodeInput>().parameterType == DataType.LIST)
                {
                    if (variable.nodeType == NodeType.LIST)
                    {
                        return true;
                    }

                    return false;
                }

                else if (targetSocket.GetComponent<MCNodeInput>().parameterType == DataType.EXPRESSION)
                {
                    if (variable.nodeType == NodeType.EXPRESSION)
                    {
                        return true;
                    }

                    return false;
                }

                else
                {
                    if (variable.nodeType == NodeType.LIST)
                    {
                        return false;
                    }

                    //if (targetSocket.Node.nodeData.type == NodeType.FUNCTION_OUTPUT)
                    //{
                    //    Utils.LogRed($"{variable.dataType} / {targetSocket.GetComponent<MCNodeInput>().parameterType} / {variable.dataType == targetSocket.GetComponent<MCNodeInput>().parameterType}");
                    //}

                    if (variable.dataType == targetSocket.GetComponent<MCNodeInput>().parameterType)
                    {
                        return true;
                    }

                    if (variable.dataType == DataType.STRING && targetSocket.GetComponent<MCNodeInput>().parameterType == DataType.NUMBER)
                    {
                        return true;
                    }

                    return IsParametersConnectable<MCNodeInput>(ParameterPosition.Left, targetSocket, variable.value);
                }
            }
            #endregion

            #region MCGetElementNode인 경우.
            else if (Node is MCGetElementNode)
            {
                MCGetElementNode getElementNode = Node as MCGetElementNode;
                MCNodeInput nodeInput = getElementNode.GetNodeInputWithIndex(0);
                MCNode foundNode = MCWorkspaceManager.Instance.FindNodeWithID(nodeInput.input.source);
                if (foundNode is MCGetNode)
                {
                    MCGetNode getNode = foundNode as MCGetNode;
                    LeftMenuVariableItem variable;
                    if (getNode.IsLocalVariable == true)
                    {
                        variable = MCWorkspaceManager.Instance.GetLocalVariable(getNode.CurrentVariableIndex);
                    }
                    else
                    {
                        variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
                    }

                    MCNodeInput targetInput = targetSocket.GetComponent<MCNodeInput>();

                    // string->string/number->string, OK.
                    if (targetInput.parameterType == DataType.STRING)
                    {
                        if (variable.nodeType == NodeType.LIST)
                        {
                            if (variable.dataType == DataType.STRING || variable.dataType == DataType.NUMBER)
                            {
                                return true;
                            }
                        }

                        return false;
                    }

                    // number->number only OK.
                    if (targetInput.parameterType == DataType.NUMBER)
                    {
                        if (variable.nodeType == NodeType.LIST && variable.dataType == DataType.NUMBER)
                        {
                            return true;
                        }

                        return false;
                    }

                    // bool->bool only ok.
                    if (targetInput.parameterType == DataType.BOOL)
                    {
                        if (variable.nodeType == NodeType.LIST && variable.dataType == DataType.BOOL)
                        {
                            return true;
                        }

                        return false;
                    }

                    // facial->facial OK.
                    if (targetInput.parameterType == DataType.FACIAL)
                    {
                        if (variable.nodeType == NodeType.LIST && variable.dataType == DataType.FACIAL)
                        {
                            return true;
                        }

                        return false;
                    }

                    // motion->motion OK.
                    if (targetInput.parameterType == DataType.MOTION)
                    {
                        if (variable.nodeType == NodeType.LIST && variable.dataType == DataType.MOTION)
                        {
                            return true;
                        }

                        return false;
                    }

                    // mobility->mobilityOK.
                    if (targetInput.parameterType == DataType.MOBILITY)
                    {
                        if (variable.nodeType == NodeType.LIST && variable.dataType == DataType.MOBILITY)
                        {
                            return true;
                        }

                        return false;
                    }
                }

                else if (targetType == SocketType.Input)
                {
                    return IsParametersConnectable<MCNodeInput>(ParameterPosition.Left, targetSocket, output.value);
                }
            }
            #endregion

            else if (targetType == SocketType.Input)
            {
                return IsParametersConnectable<MCNodeInput>(ParameterPosition.Left, targetSocket, output.value);
            }

            return false;
        }

        protected override void OnDestroy()
        {
            //base.OnDestroy();
            //foreach (MCBezierLine line in lines)
            //{
            //    MCWorkspaceManager.Instance.RequestLineDelete(line.LineID);
            //}
        }

        internal void HighlightOn()
        {
            foreach (var line in lines)
            {
                line.HighlightOn();
            }
        }


        #region CheckTargetSocketType 코드 백업
        //public override bool CheckTargetSocketType(SocketType targetType, MCNodeSocket targetSocket = null)
        //{
        //    if (Node is MCGetNode)
        //    {
        //        MCGetNode getNode = Node as MCGetNode;
        //        LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);

        //        // EXPRESSION.
        //        if (parameterType == DataType.EXPRESSION
        //            && targetSocket.GetComponent<MCNodeInput>().parameterType == DataType.EXPRESSION)
        //        {
        //            return true;
        //        }

        //        if (variable.nodeType == NodeType.LIST)
        //        {
        //            if (targetSocket.GetComponent<MCNodeInput>().parameterType == DataType.LIST ||
        //                targetSocket.GetComponent<MCNodeInput>().parameterType == parameterType)
        //            {
        //                return true;
        //            }
        //        }
        //        else if (variable.nodeType == NodeType.VARIABLE)
        //        {
        //            if (parameterType == DataType.STRING &&
        //                targetSocket.GetComponent<MCNodeInput>().parameterType == DataType.NUMBER)
        //            {
        //                //Debug.LogWarning($"MCGetNode.Value Check: {variable.value}");

        //                int retValue = 0;
        //                if (int.TryParse(variable.value, out retValue))
        //                {
        //                    return true;
        //                }

        //                return false;
        //            }

        //            if (targetType == SocketType.Input && 
        //                parameterType == targetSocket.GetComponent<MCNodeInput>().parameterType)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    //else if (targetType == SocketType.Input
        //    //    && parameterType == targetSocket.GetComponent<MCNodeInput>().parameterType)
        //    //{
        //    //    return true;
        //    //}
        //    else if (targetType == SocketType.Input)
        //    {
        //        if (parameterType == targetSocket.GetComponent<MCNodeInput>().parameterType)
        //        {
        //            return true;
        //        }
        //        else if (parameterType == PROJECT.DataType.NUMBER &&
        //            targetSocket.GetComponent<MCNodeInput>().parameterType == PROJECT.DataType.STRING)
        //        {
        //            return true;
        //        }
        //        else if (parameterType == PROJECT.DataType.STRING &&
        //            targetSocket.GetComponent<MCNodeInput>().parameterType == PROJECT.DataType.NUMBER)
        //        {
        //            int retValue = 0;
        //            if (int.TryParse(output.value, out retValue))
        //            {
        //                return true;
        //            }

        //            return false;
        //        }
        //    }

        //    return false;
        //}
        #endregion
    }
}