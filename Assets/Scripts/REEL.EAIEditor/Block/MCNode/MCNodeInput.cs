using TMPro;

using REEL.PROJECT;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    public class MCNodeInput : MCNodeInputOutputBase
    {
        [SerializeField]
        public Image altImage;

        public PROJECT.Input input;

        private UnityEvent OnListConnected;
        private UnityEvent OnListDisConnected;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            if (OnListConnected is null)
            {
                OnListConnected = new UnityEvent();
            }

            if (OnListDisConnected is null)
            {
                OnListDisConnected = new UnityEvent();
            }

            socketPosition = SocketPosition.Left;
            socketType = SocketType.Input;

            input.source = -1;
            input.subid = -1;

            if (altImage)
            {
                altImage.color = lineColor;

                altImage.GetComponent<TMP_InputField>()?.onEndEdit.AddListener(OnSubmitInputfield);
                //altImage.GetComponent<TMP_InputField>()?.onDeselect.AddListener(OnSubmitInputfield);
            }

            if (parameterType != PROJECT.DataType.EXPRESSION &&
                parameterType != PROJECT.DataType.LIST)
            {
                input.type = parameterType;
            }

            Node.SubscribeOnNodeStateChanged(OnNodeStateChanged);
        }

        protected virtual void OnDisable()
        {
            Node.UnsubscribeOnNodeStateChanged(OnNodeStateChanged);
        }

        public void SetInputData(PROJECT.Input input, bool isFunctionUpdate = false)
        {
            if (this.input is null)
            {
                this.input = new PROJECT.Input();
            }
            
            this.input.id = input.id;
            this.input.type = input.type;
            parameterType = input.type;

            if (isFunctionUpdate is false)
            {
                this.input.source = input.source;
                this.input.subid = input.subid;
            }
            
            this.input.default_value = input.default_value;
            this.input.name = input.name;
        }

        // for inputfield.
        public void SetAlterData(string value)
        {
            input.default_value = value;

            // Set Project Dirty.
            MCWorkspaceManager.Instance.IsDirty = true;
        }

        public void OnSubmitInputfield(string value)
        {
            //----------------------------------------
            // 인풋 필드 입력시 발생하는 튜토리얼이벤트 -kjh
            //----------------------------------------
            TutorialManager.SendEvent(Tutorial.CustomEvent.InputFieldSubmitted, value);     //Contains
            TutorialManager.SendEvent(Tutorial.CustomEvent.InputFieldSubmittedEqual, value);
        }

        // for dropdown.
        public void SetAlterData(int value)
        {
            input.default_value = GetDropdownDataWithIndex(value);

            // FunctionItem의 값도 같이 업데이트.
            if (Node != null)
            {
                MCFunctionNode functionNode = Node as MCFunctionNode;
                if (functionNode != null)
                {
                    var functionItem = MCWorkspaceManager.Instance.GetFunctionItemWithID(functionNode.FunctionID);
                    if (functionItem != null && functionItem.Inputs.Length > input.id)
                    {
                        functionItem.Inputs[input.id].default_value = input.default_value;
                    }
                }
            }

            // Set Project Dirty.
            MCWorkspaceManager.Instance.IsDirty = true;

            //----------------------------------------
            // 인풋 필드 입력시 발생하는 튜토리얼이벤트 -kjh
            //----------------------------------------
            string facialNameInKorean = GetDropdownDataWithIndexForkor(value);
            TutorialManager.SendEvent(Tutorial.CustomEvent.DropdownSelected , facialNameInKorean);
        }


        private string GetDropdownDataWithIndex(int index)
        {
            MoccaTMPDropdownHelper helper = altImage.GetComponent<MoccaTMPDropdownHelper>();
            if (helper != null)
            {
                if (helper.type == MoccaTMPDropdownHelper.Type.Facial)
                {
                    //return Constants.facialList[index];
                    return Constants.facialListData[index].nameEnglish;
                }

                if (helper.type == MoccaTMPDropdownHelper.Type.Motion)
                {
                    //return Constants.motionList[index];
                    return Constants.motionListData[index].nameEnglish;
                }

                if (helper.type == MoccaTMPDropdownHelper.Type.Mobility)
                {
                    //return Constants.mobilityList[index];
                    return Constants.mobilityListData[index].nameEnglish;
                }

                if (helper.type == MoccaTMPDropdownHelper.Type.ExpressionVariableList)
                {
                    if (Constants.GetExpressionVariableListTMPOptionData.Count > 0)
                    {
                        return Constants.GetExpressionVariableListTMPOptionData[index].text;
                    }
                }
            }

            return string.Empty;
        }

        private string GetDropdownDataWithIndexForkor(int index)
        {
            MoccaTMPDropdownHelper helper = altImage.GetComponent<MoccaTMPDropdownHelper>();
            if(helper != null)
            {
                if(helper.type == MoccaTMPDropdownHelper.Type.Facial)
                {
                    //return Constants.facialList[index];
                    return Constants.facialListData[index].nameKorean;
                }

                if(helper.type == MoccaTMPDropdownHelper.Type.Motion)
                {
                    //return Constants.motionList[index];
                    return Constants.motionListData[index].nameKorean;
                }

                if(helper.type == MoccaTMPDropdownHelper.Type.Mobility)
                {
                    //return Constants.mobilityList[index];
                    return Constants.mobilityListData[index].nameKorean;
                }

                if(helper.type == MoccaTMPDropdownHelper.Type.ExpressionVariableList)
                {
                    if(Constants.GetExpressionVariableListTMPOptionData.Count > 0)
                    {
                        return Constants.GetExpressionVariableListTMPOptionData[index].text;
                    }
                }
            }

            return string.Empty;
        }


        public void SetInputIndex(int index)
        {
            input.id = index;
        }

        public override void SetLine(MCBezierLine line)
        {
            base.SetLine(line);

            input.source = line.left.Node.NodeID;
            input.subid = (line.left as MCNodeOutput).output.id;
            if (Node.nodeData.inputs != null 
                && Node.nodeData.inputs.Length > 0
                && input.id < Node.nodeData.inputs.Length - 1
                && Node.nodeData.inputs[input.id] != null)
            {
                Node.nodeData.inputs[input.id].source = input.source;
                Node.nodeData.inputs[input.id].subid = input.subid;
            }

            //Utils.LogRed($"[SetLine] lineID: {line.LineID} / Node.name: {Node.name} / input.source: {input.source} / input.subid: {input.subid}");
        }

        public override void RemoveLine(int lineID = -1)
        {
            base.RemoveLine();

            //Utils.LogRed($"[RemoveLine] lineID: {lineID} / Node.name: {Node.name}");

            input.source = -1;
            input.subid = -1;

            if (lineID != -1)
            {
                OnRemoveLine?.Invoke(lineID);
            }
        }

        public override bool CheckTargetSocketType(SocketType targetType, MCNodeSocket targetSocket = null)
        {
            if (targetSocket.Node is MCGetNode)
            {
                MCGetNode getNode = targetSocket.Node as MCGetNode;
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
                if (Node is MCChoiceNode)
                {
                    if (parameterType == DataType.LIST)
                    {
                        if (variable.nodeType == NodeType.LIST && variable.dataType == DataType.STRING)
                        {
                            return true;
                        }

                        return false;
                    }

                    else if (parameterType == DataType.STRING)
                    {
                        if (variable.nodeType != NodeType.LIST && variable.dataType == DataType.STRING)
                        {
                            //Utils.LogRed("Hello");
                            return true;
                        }

                        return false;
                    }
                }

                if (parameterType == DataType.LIST)
                {
                    if (variable.nodeType == NodeType.LIST)
                    {
                        return true;
                    }

                    return false;
                }

                else if (parameterType == DataType.EXPRESSION)
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

                    return IsParametersConnectable<MCNodeOutput>(ParameterPosition.Right, targetSocket, variable.value);
                }
            }

            else if (targetType == SocketType.Output)
            {
                return IsParametersConnectable<MCNodeOutput>(ParameterPosition.Right, targetSocket, targetSocket.GetComponent<MCNodeOutput>().output.value);
            }

            return false;
        }

        #region CheckTargetSocketType 기존 코드 백업
        //public override bool CheckTargetSocketType(SocketType targetType, MCNodeSocket targetSocket = null)
        //{
        //    if (parameterType == PROJECT.DataType.LIST)//count, getelem, insert, remove, removeall 일 때.
        //    {
        //        if (targetSocket.Node is MCGetNode)
        //        {
        //            MCGetNode getNode = targetSocket.Node as MCGetNode;
        //            LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
        //            if (variable.nodeType == PROJECT.NodeType.LIST)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (targetSocket.Node is MCGetNode)
        //        {
        //            MCGetNode getNode = targetSocket.Node as MCGetNode;
        //            LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);

        //            if (targetSocket.GetComponent<MCNodeOutput>().parameterType == DataType.STRING &&
        //                parameterType == DataType.NUMBER)
        //            {
        //                int retValue = 0;
        //                if (int.TryParse(variable.value, out retValue))
        //                {
        //                    return true;
        //                }

        //                return false;
        //            }
        //            else if (targetType == SocketType.Output
        //                && parameterType == targetSocket.GetComponent<MCNodeOutput>().parameterType)
        //            {
        //                return true;
        //            }

        //            //else if (targetType == SocketType.Output
        //            //&& parameterType == targetSocket.GetComponent<MCNodeOutput>().parameterType
        //            //&& variable.nodeType != PROJECT.NodeType.LIST)
        //            //{
        //            //    return true;
        //            //}
        //        }
        //        //else if (targetSocket.Node is MCSetNode)
        //        //{
        //        //    MCSetNode setNode = targetSocket.Node as MCSetNode;
        //        //    LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetVariable(setNode.CurrentVariableIndex);
        //        //    if (targetType == SocketType.Output
        //        //        && parameterType == targetSocket.GetComponent<MCNodeOutput>().parameterType
        //        //        && variable.nodeType != PROJECT.NodeType.LIST)
        //        //        return true;
        //        //}
        //        //else if (targetType == SocketType.Output
        //        //    && parameterType == targetSocket.GetComponent<MCNodeOutput>().parameterType)
        //        //{
        //        //    return true;
        //        //}
        //        else if (targetType == SocketType.Output)
        //        {
        //            if (parameterType == targetSocket.GetComponent<MCNodeOutput>().parameterType)
        //            {
        //                return true;
        //            }
        //            else if (parameterType == PROJECT.DataType.NUMBER &&
        //                targetSocket.GetComponent<MCNodeOutput>().parameterType == PROJECT.DataType.STRING)
        //            {
        //                int retValue = 0;
        //                if (int.TryParse(targetSocket.GetComponent<MCNodeOutput>().output.value, out retValue))
        //                {
        //                    return true;
        //                }

        //                return false;
        //            }
        //            else if (parameterType == PROJECT.DataType.STRING &&
        //                targetSocket.GetComponent<MCNodeOutput>().parameterType == PROJECT.DataType.NUMBER)
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}
        #endregion

        public override void LineDeleted()
        {
            base.LineDeleted();

            if (altImage
                //&& parameterType != PROJECT.DataType.EXPRESSION
                && parameterType != PROJECT.DataType.LIST)
            {
                altImage.gameObject.SetActive(true);
            }

            OnListDisConnected?.Invoke();

            //----------------------------------------
            // 입력 파라미터에 선 연결 해제됐을 때 발생하는 튜토리얼이벤트 -kjh
            //----------------------------------------
            TutorialManager.SendEvent(Tutorial.CustomEvent.ParameterUnlinked);
        }

        public override void LineSet()
        {
            base.LineSet();

            if (altImage)
            {
                altImage.gameObject.SetActive(false);
                ChangeSpriteToSet();
            }

            OnListConnected?.Invoke();
        }

        private void OnNodeStateChanged()
        {
            // Get/Set 노드에서 Dropdown 값이 바뀌면 이를 반영해 선 연결을 유지할 지 확인해야 함.
            if (Node is MCSetNode)
            {
                if (HasLine == true && line != null && line.left != null && line.right != null)
                {
                    MCNodeSocket left = line.left;
                    if (CheckTargetSocketType(left.socketType, left) == false)
                    {
                        MCDeleteLineCommand command = new MCDeleteLineCommand(
                            line.LineID, left, this, line.color
                        );

                        MCUndoRedoManager.Instance.AddCommand(command);
                    }
                }
            }
        }

        public void SubscribeOnListDisConnected(UnityAction OnConnected)
        {
            if (OnListDisConnected is null)
            {
                OnListDisConnected = new UnityEvent();
            }

            OnListDisConnected.AddListener(OnConnected);
        }

        public void UnSubscribeOnListDisConnected(UnityAction OnConnected)
        {
            OnListDisConnected.RemoveListener(OnConnected);
        }

        public void SubscribeOnListConnected(UnityAction OnConnected)
        {
            if (OnListConnected is null)
            {
                OnListConnected = new UnityEvent();
            }

            OnListConnected.AddListener(OnConnected);
        }

        public void UnSubscribeOnListConnected(UnityAction OnConnected)
        {
            OnListConnected.RemoveListener(OnConnected);
        }
    }
}
