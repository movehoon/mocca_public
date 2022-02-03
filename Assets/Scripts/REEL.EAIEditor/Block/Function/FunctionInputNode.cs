using UnityEngine;
using System.Collections.Generic;
using REEL.PROJECT;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    public class FunctionInputNode : MCNode
    {
        public RectTransform epRight;

        public GameObject outputSocketPrefab;
        public GameObject inputfieldPrefab;
        public GameObject dropdownPrefab;

        public List<PROJECT.Input> functionInputs = new List<PROJECT.Input>();

        public float socketOriginYPos = -19f;
        public float offsetBWExecSocketAndFirstSocket = 49f;
        public float blockOriginYSize = 90f;
        public float ySizeOffset = 22.5f;

        private LeftMenuFunctionItem targetFunctionItem;

        private List<GameObject> socketAndInputList = new List<GameObject>();

        protected override void OnEnable()
        {
            base.OnEnable();

            MCNodeNext nodeNext = epRight.GetComponent<MCNodeNext>();
            nodeNext.SubscribeOnLineConnected(OnStartNodeConnected);
            nodeNext.SubscribeOnLineDelete(OnStartNodeDisconnected);

            targetFunctionItem = MCWorkspaceManager.Instance.GetFunctionItemWithName(Utils.CurrentTabName);
            //int functionID = (FindObjectOfType<TabManager>().CurrentTab as MCFunctionTab).FunctionID;
            //targetFunctionItem = MCWorkspaceManager.Instance.GetFunctionItemWithID(functionID);

            MCWorkspaceManager.Instance.SubscribeFunctionUpdate(OnFunctionUpdate);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MCNodeNext nodeNext = epRight.GetComponent<MCNodeNext>();
            nodeNext.UnSubscribeOnLineConnected(OnStartNodeConnected);
            nodeNext.UnSubscribeOnLineDelete(OnStartNodeDisconnected);

            MCWorkspaceManager.Instance.UnsubscribeFunctionUpdate(OnFunctionUpdate);

            targetFunctionItem = null;
        }

        public void SetInputInfo(PROJECT.Input[] inputs, bool shouldReset = false)
        {
            if (shouldReset is true)
            {
                for (int ix = 0; ix < functionInputs.Count; ++ix)
                {
                    functionInputs[ix] = null;
                }

                functionInputs.Clear();
            }

            foreach (PROJECT.Input input in inputs)
            {
                functionInputs.Add(input);
            }
        }

        private void OnStartNodeConnected(MCBezierLine line)
        {
            targetFunctionItem.FunctionData.startID = line.right.Node.NodeID;
        }

        private void OnStartNodeDisconnected()
        {
            targetFunctionItem.FunctionData.startID = -2;
        }

        protected Vector2 SetTotalSizeOfBlock()
        {
            // 블록 크기 설정.
            RectTransform refRect = GetComponent<RectTransform>();
            Vector2 size = refRect.sizeDelta;
            //size.y = blockOriginYSize + functionInputs.Count* ySizeOffset;
            size.y = blockOriginYSize + Mathf.Max(0, functionInputs.Count - 1) * ySizeOffset;
            refRect.sizeDelta = size;

            RectTransform targetRect = selectedGameObject.GetComponent<RectTransform>();
            Vector2 selectionSize = targetRect.sizeDelta;
            selectionSize = size + new Vector2(2f, 2f);
            targetRect.sizeDelta = selectionSize;

            return size;
        }

        protected Vector2 SetPositionOfFirstSocket(Vector2 blockSize)
        {
            // 첫번째 소켓의 y위치 계산.
            float yOffset = (blockSize.y - blockOriginYSize) * 0.5f + socketOriginYPos;
            Vector2 newPos = epRight.anchoredPosition;
            newPos.y = yOffset;

            return newPos;
        }

        protected void SetPositionOfFirstExecuteSocket(Vector2 firstSocketPos)
        {
            // 실행 소켓에 첫번째 y위치 설정.
            epRight.anchoredPosition = firstSocketPos;
            firstSocketPos.x = epRight.anchoredPosition.x;
            epRight.anchoredPosition = firstSocketPos;
        }

        protected void InitInputs(float yOffset /* 블록 크기의 Y위치 값 */)
        {
            // 입력 프리팹 생성 및 위치 설정(입력 개수 대로 생성).
            //List<FunctionInputGO> inputGOs = new List<FunctionInputGO>();
            outputs = new MCNodeOutput[functionInputs.Count];
            for (int ix = 0; ix < functionInputs.Count; ++ix)
            {
                var functionInput = functionInputs[ix];

                // 입력 소켓 생성.
                RectTransform socket = Instantiate(outputSocketPrefab, transform).GetComponent<RectTransform>();
                Vector2 position = epRight.anchoredPosition;
                //position.y = yOffset - ySizeOffset * (ix + 1);
                position.y = yOffset - ySizeOffset * ix;
                socket.anchoredPosition = position;

                socketAndInputList.Add(socket.gameObject);

                MCNodeOutput nodeOutput = socket.GetComponent<MCNodeOutput>();
                nodeOutput.parameterType = functionInput.type;

                // 라인 색상 설정.
                nodeOutput.SetLineColor(Utils.GetParameterColor(functionInput.type));

                // 추가할 입력 프리팹 결정 (InputField or Dropdown).
                //GameObject inputPrefab = GetInputPrefab(functionInput.type);

                // 입력 필드 생성.
                //RectTransform input = Instantiate(inputPrefab, transform).GetComponent<RectTransform>();
                //position.x = input.anchoredPosition.x;
                //input.anchoredPosition = position;

                //socketAndInputList.Add(input.gameObject);

                // 입력 필드 색성 설정.
                //input.GetComponent<Image>().color = Utils.GetParameterColor(functionInput.type);

                // 드롭다운인 경우.
                //if (IsDropdownType(functionInput.type) == true)
                //{
                //    var dropdown = input.GetComponent<TMP_Dropdown>();
                    

                //    dropdown.onValueChanged.AddListener((value) =>
                //    {
                //        if (targetFunctionItem != null)
                //        {
                //            if (targetFunctionItem != null && targetFunctionItem.Inputs.Length > functionInput.id)
                //            {
                //                targetFunctionItem.Inputs[functionInput.id].default_value = functionInput.default_value;
                //            }
                //        }
                //    });

                //    string defaultValue = functionInput.default_value;
                //    //Utils.LogRed($"defaultValue: {defaultValue}");
                    
                //    // 드롭다운인 경우 타입 설정(Facial, Motion, Mobility).
                //    SetDropdownType(functionInput.type, input, () =>
                //    {
                //        dropdown.value = Constants.GetDropdownValueIndex(defaultValue, functionInput.type);
                //    });
                //}

                // MCNode 입력에 추가.
                outputs[ix] = nodeOutput;

                // output 노드 id 설정.
                outputs[ix].output = new Output();
                outputs[ix].output.id = ix;
                outputs[ix].output.type = nodeOutput.parameterType;
            }
        }

        protected bool IsDropdownType(DataType dataType)
        {
            return dataType == DataType.FACIAL
                || dataType == DataType.MOBILITY
                || dataType == DataType.MOTION;
        }

        protected void SetDropdownType(DataType dataType, RectTransform input, System.Action function = null)
        {
            switch (dataType)
            {
                case DataType.FACIAL:
                case DataType.MOBILITY:
                case DataType.MOTION:
                    //input.GetComponent<MoccaDropdownHelper>().SetType(dataType);
                    input.GetComponent<MoccaTMPDropdownHelper>().SetType(dataType, function);
                    break;
                default: break;
            }
        }

        protected GameObject GetInputPrefab(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.FACIAL:
                case DataType.MOTION:
                case DataType.MOBILITY:
                    return dropdownPrefab;

                default: return inputfieldPrefab;
            }
        }

        public void InitializeBlock()
        {
            // 블록 크기 설정.
            Vector2 size = SetTotalSizeOfBlock();

            // 첫번째 소켓의 y위치 계산.
            Vector2 socketPos = SetPositionOfFirstSocket(size);
            float yOffset = socketPos.y;

            // 실행 소켓에 첫번째 y위치 설정.
            SetPositionOfFirstExecuteSocket(socketPos);

            // 입력 프리팹 생성 및 위치 설정(입력 개수 대로 생성).
            InitInputs(yOffset - offsetBWExecSocketAndFirstSocket);
        }

        private void OnFunctionUpdate(FunctionData functionData)
        {
            // 함수 탭이 열린 상태에서, 함수가 삭제됐는지 확인.
            if (functionData == null)
            {
                return;
            }

            if (Utils.IsTheSameFunction(
                targetFunctionItem.FunctionID, 
                functionData.functionID,
                targetFunctionItem.FunctionData.name,
                functionData.name) == false)
            {
                return;
            }

            CompileRestLogic(functionData);
            ResetSocketAndInputs();

            SetInputInfo(functionData.inputs, true);
            InitializeBlock();

            // 입력 노드 Input과 연결된 소켓 선 생성&연결.
            CreateInputLine(functionData);

            Utils.GetTables().DelayCheckSocketState();

            //Utils.LogRed("[FunctionInputNode.OnFunctionUpdate]");
        }

        private void CompileRestLogic(FunctionData functionData)
        {
            //LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithName(functionData.name);
            LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithID(functionData.functionID);
            if (function == null || function.LogicNodes == null || function.LogicNodes.Count == 0)
            {
                return;
            }

            for (int ix = 0; ix < function.LogicNodes.Count; ++ix)
            {
                function.LogicNodes[ix].MakeNode();
            }
        }

        private void ResetSocketAndInputs()
        {
            MCTables tables = Utils.GetTables();

            // 함수 명세가 변경되면 일단 Socket/Input/Dropdown 모두 삭제한 다음,다시 생성.
            if (socketAndInputList != null && socketAndInputList.Count > 0)
            {
                foreach (GameObject go in socketAndInputList)
                {
                    if (go.GetComponent<MCNodeSocket>() != null)
                    {
                        MCNodeSocket socket = go.GetComponent<MCNodeSocket>();
                        if (socket.HasLine == true)
                        {
                            if (tables.locatedLines.Count > 0)
                            {
                                for (int ix = tables.locatedLines.Count - 1; ix >= 0; --ix)
                                {
                                    var line = tables.locatedLines[ix];
                                    if (line is null)
                                    {
                                        continue;
                                    }

                                    if (socket is MCNodeOutput)
                                    {
                                        MCNodeOutput nodeOutput = socket as MCNodeOutput;

                                        foreach (MCBezierLine socketLine in nodeOutput.lines)
                                        {
                                            if (line.LineID.Equals(socketLine.LineID))
                                            {
                                                socketLine.UnsubscribeAllDragNodes();
                                                socketLine.dontChangeSocketInfo = true;
                                                Destroy(socketLine.gameObject);
                                                tables.RemoveLine(socketLine);
                                            }
                                        }

                                        nodeOutput.lines.Clear();
                                    }
                                }
                            }
                        }
                    }

                    Destroy(go);
                }

                socketAndInputList.Clear();
            }
        }

        private LeftMenuFunctionItem GetCurrentFunction(FunctionData functionData)
        {
            //LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithName(functionData.name);
            LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithID(functionData.functionID);

            if (function == null)
            {
                function = MCWorkspaceManager.Instance.GetFunctionItemWithName(functionData.name);
            }

            return function;
        }

        private void CreateInputLine(FunctionData functionData)
        {
            LeftMenuFunctionItem function = GetCurrentFunction(functionData);
            if (function.LogicNodes == null)
            {
                return;
            }

            foreach (MCNode node in function.LogicNodes)
            {
                foreach (PROJECT.Input input in node.nodeData.inputs)
                {
                    // 노드의 입력이 Function Input 노드를 가리키는 경우.
                    if (input.source.Equals(NodeID))
                    {
                        MCNodeOutput left = GetNodeOutputWithIndex(input.subid);
                        MCNodeInput right = node.GetNodeInputWithIndex(input.id);
                        if (left == null || right == null)
                        {
                            if (left == null && right != null)
                            {
                                right.RemoveLine();
                                right.LineDeleted();
                                MCWorkspaceManager.Instance.RemoveLine(right.line);
                                continue;
                            }
                            if (left != null && right == null)
                            {
                                left.RemoveLine();
                                left.LineDeleted();
                                MCWorkspaceManager.Instance.RemoveLine(left.line);
                                continue;
                            }
                            if (left == null && right == null)
                            {
                                continue;
                            }
                        }

                        if (left.CheckTargetSocketType(right.socketType, right) == false)
                        {
                            var deleteLine = left.line == null ? right.line : left.line;
                            if (deleteLine != null)
                            {
                                deleteLine.UnsubscribeAllDragNodes();
                                Destroy(deleteLine.gameObject);
                                Utils.GetTables().RemoveLine(deleteLine);
                            }

                            left.RemoveLine();
                            left.LineDeleted();

                            right.RemoveLine();
                            right.LineDeleted();

                            continue;
                        }

                        if (left.line != null)
                        {
                            MCWorkspaceManager.Instance.RequestLineDelete(left.line.LineID);
                        }

                        else if (right.line != null)
                        {
                            MCWorkspaceManager.Instance.RequestLineDelete(right.line.LineID);
                        }

                        MCBezierLine line = Utils.CreateNewLine();
                        //line.SetLineColor(Utils.GetParameterColor(left.parameterType));
                        line.SetLineColor(Utils.GetParameterColor(right.parameterType));

                        line.SetLinePoint(left, right);
                        MCWorkspaceManager.Instance.AddLine(line);

                        left.LineSet();
                        left.SetOutputData(left.output);

                        right.LineSet();
                        right.SetInputData(right.input);
                    }
                }
            }

            Utils.GetTables().DelayCheckSocketState();

            #region 기존 코드 주석처리
            //if (node.nodeData.inputs is null)
            //{
            //    continue;
            //}

            //foreach (PROJECT.Input input in node.nodeData.inputs)
            //{
            //    // 노드의 입력이 Function Input 노드를 가리키는 경우.
            //    if (input.source.Equals(NodeID))
            //    {
            //        MCNodeOutput left = GetNodeOutputWithIndex(input.subid);
            //        MCNodeInput right = node.GetNodeInputWithIndex(input.id);
            //        if (left == null || right == null)
            //        {
            //            if (left == null && right != null)
            //            {
            //                right.RemoveLine();
            //                right.LineDeleted();
            //                MCWorkspaceManager.Instance.RemoveLine(right.line);
            //                continue;
            //            }
            //            if (left != null && right == null)
            //            {
            //                left.RemoveLine();
            //                left.LineDeleted();
            //                MCWorkspaceManager.Instance.RemoveLine(left.line);
            //                continue;
            //            }
            //            if (left == null && right == null)
            //            {
            //                continue;
            //            }
            //        }

            //        if (left.CheckTargetSocketType(right.socketType, right) == false)
            //        {
            //            left.RemoveLine();
            //            left.LineDeleted();

            //            right.RemoveLine();
            //            right.LineDeleted();
            //            continue;
            //        }

            //        MCBezierLine line = Utils.CreateNewLine();
            //        line.SetLineColor(Utils.GetParameterColor(left.parameterType));

            //        line.SetLinePoint(left, right);
            //        MCWorkspaceManager.Instance.AddLine(line);

            //        left.LineSet();
            //        left.SetOutputData(left.output);

            //        right.LineSet();
            //        right.SetInputData(right.input);
            //    }
            //}
            #endregion
        }

        private MCNode GetNodeWithID(int nodeID)
        {
            LeftMenuFunctionItem functionItem = CurrentFunctionItem;
            foreach (var node in functionItem.LogicNodes)
            {
                if (node.NodeID.Equals(nodeID))
                {
                    return node;
                }
            }

            return null;
        }

        private LeftMenuFunctionItem CurrentFunctionItem
        {
            get
            {
                int index = MCWorkspaceManager.Instance.GetFunctionIndexWithName(
                    Utils.CurrentTabName
                );
                return MCWorkspaceManager.Instance.GetFunction(index);
            }
        }
    }
}