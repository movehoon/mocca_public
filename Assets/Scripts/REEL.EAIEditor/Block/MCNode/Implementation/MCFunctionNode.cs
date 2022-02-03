using System;
using System.Collections;
using System.Collections.Generic;
using REEL.PROJECT;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace REEL.D2EEditor
{
    public class FunctionInputGO
    {
        public GameObject socket;
        public GameObject input;
    }

    public class MCFunctionNode : MCNode
    {
        [Header("Execution Sockets")]
        public RectTransform epLeft;
        public RectTransform epRight;

        [Header("Input/Output Socket Prefabs")]
        public GameObject inputSocketPrefab;
        public GameObject outputSocketPrefab;
        public GameObject inputfieldPrefab;
        public GameObject dropdownPrefab;

        private List<GameObject> inputSocketAndInputList = new List<GameObject>();
        private List<GameObject> outputSockets = new List<GameObject>();

        public float socketOriginYPos = -19f;
        public float offsetBWexecSocketAndFirstSocket = 49f;
        public float blockOriginYSize = 90f;
        public float ySizeOffset = 22.5f;

        private string functionname;
        public string FunctionName
        {
            get { return functionname; }
            set
            {
                functionname = value;
                functionNameText.text = functionname;
            }
        }
        public int FunctionIndex { get { return MCWorkspaceManager.Instance.GetFunctionIndexWithName(FunctionName); } }
        public int StartID { get; set; }
        public int FunctionID { get; set; }

        public TMP_Text functionNameText;

        protected Vector2 SetTotalSizeOfBlock()
        {
            // 블록 크기 설정.
            Vector2 size = GetComponent<RectTransform>().sizeDelta;
            //size.y = blockOriginYSize + Mathf.Max(nodeData.inputs.Length, nodeData.outputs.Length) * ySizeOffset;
            size.y = blockOriginYSize + Mathf.Max(0, Mathf.Max(nodeData.inputs.Length - 1, nodeData.outputs.Length - 1)) * ySizeOffset;
            GetComponent<RectTransform>().sizeDelta = size;

            Vector2 selectionSize = selectedGameObject.GetComponent<RectTransform>().sizeDelta;
            selectionSize = size + new Vector2(2f, 2f);
            selectedGameObject.GetComponent<RectTransform>().sizeDelta = selectionSize;

            return size;
        }

        protected Vector2 SetPositionOfFirstSocket(Vector2 blockSize)
        {
            // 첫번째 소켓의 y위치 계산.
            float yOffset = (blockSize.y - blockOriginYSize) * 0.5f + socketOriginYPos;
            Vector2 newPos = epLeft.anchoredPosition;
            newPos.y = yOffset;

            return newPos;
        }

        protected void SetPositionOfFirstExecuteSocket(Vector2 firstSocketPos)
        {
            // 실행 소켓에 첫번째 y위치 설정.
            epLeft.anchoredPosition = firstSocketPos;
            firstSocketPos.x = epRight.anchoredPosition.x;
            epRight.anchoredPosition = firstSocketPos;
        }

        protected void InitInputs(float yOffset /* 블록 크기의 Y위치 값 */, bool isFunctionUpdate = false)
        {
            // 입력 프리팹 생성 및 위치 설정(입력 개수 대로 생성).
            //List<FunctionInputGO> inputGOs = new List<FunctionInputGO>();
            inputs = new MCNodeInput[nodeData.inputs.Length];
            for (int ix = 0; ix < nodeData.inputs.Length; ++ix)
            {
                // 입력 소켓 생성.
                RectTransform socket = Instantiate(inputSocketPrefab, transform).GetComponent<RectTransform>();
                Vector2 position = epLeft.anchoredPosition;
                //position.y = yOffset - ySizeOffset * (ix + 1);
                position.y = yOffset - ySizeOffset * ix;
                socket.anchoredPosition = position;

                inputSocketAndInputList.Add(socket.gameObject);

                MCNodeInput nodeInput = socket.GetComponent<MCNodeInput>();
                nodeInput.parameterType = nodeData.inputs[ix].type;
                nodeInput.SetInputData(nodeData.inputs[ix], isFunctionUpdate);

                // 라인 색상 설정.
                nodeInput.SetLineColor(Utils.GetParameterColor(nodeData.inputs[ix].type));

                // 추가할 입력 프리팹 결정 (InputField or Dropdown).
                GameObject inputPrefab = GetInputPrefab(nodeData.inputs[ix].type);

                // 입력 필드 생성.
                RectTransform input = Instantiate(inputPrefab, transform).GetComponent<RectTransform>();
                position.x = input.anchoredPosition.x;
                input.anchoredPosition = position;

                inputSocketAndInputList.Add(input.gameObject);

                // 입력 필드 색성 설정.
                input.GetComponent<Image>().color = Utils.GetParameterColor(nodeData.inputs[ix].type);

                // MCNodeInput의 altImage 설정.
                nodeInput.altImage = input.GetComponent<Image>();

                // SetAlterData 리스너 연결.
                // InputField or Dropdown에 설정된 값을 기본 값으로 읽어올 수 있도록.
                if (IsInputDropdownType(nodeData.inputs[ix].type) == true)
                {
                    TMP_Dropdown inputDropdown = input.GetComponent<TMP_Dropdown>();
                    inputDropdown.onValueChanged.AddListener(nodeInput.SetAlterData);
                    // 드롭다운인 경우 타입 설정(Facial, Motion, Mobility).

                    // Expression 타입인 경우 처리.
                    if (nodeData.inputs[ix].type == DataType.EXPRESSION)
                    {
                        inputDropdown.options.Clear();
                        inputDropdown.options = Constants.GetExpressionVariableListTMPOptionData;
                        inputDropdown.GetComponent<MoccaTMPDropdownHelper>().type = MoccaTMPDropdownHelper.Type.ExpressionVariableList;

                        if (inputDropdown.options.Count > 0)
                        {
                            inputDropdown.gameObject.SetActive(true);

                            if (Utils.IsNullOrEmptyOrWhiteSpace(nodeData.inputs[ix].default_value) == false)
                            {
                                var nameList = MCWorkspaceManager.Instance.GetVariableNameListWithType(DataType.EXPRESSION);
                                for (int index = 0; index < nameList.Length; ++index)
                                {
                                    if (nameList[index].Equals(nodeData.inputs[ix].default_value))
                                    {
                                        inputDropdown.value = index;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            inputDropdown.gameObject.SetActive(false);
                        }
                    }
                    else    // 그 외의 타입 (Facial/Motion/Mobility).
                    {
                        string defaultValue = nodeData.inputs[ix].default_value;
                        inputDropdown.GetComponent<MoccaTMPDropdownHelper>().SetType(nodeInput.parameterType, () =>
                        {
                            inputDropdown.value = Constants.GetDropdownValueIndex(defaultValue, nodeInput.parameterType);
                        });

                        nodeInput.input.default_value = nodeData.inputs[ix].default_value;
                    }
                }
                else
                {
                    TMP_InputField inputField = input.GetComponent<TMP_InputField>();
                    inputField.onValueChanged.AddListener(nodeInput.SetAlterData);
                    inputField.text = nodeData.inputs[ix].default_value;
                }
                //else if (input.GetComponent<TMP_Dropdown>() != null)
                //{
                //    //Dropdown inputDropdown = input.GetComponent<Dropdown>();
                //    TMP_Dropdown inputDropdown = input.GetComponent<TMP_Dropdown>();
                //    inputDropdown.onValueChanged.AddListener(nodeInput.SetAlterData);
                //    // 드롭다운인 경우 타입 설정(Facial, Motion, Mobility).
                //    //inputDropdown.GetComponent<MoccaDropdownHelper>().SetType(nodeInput.parameterType);
                //    string defaultValue = nodeData.inputs[ix].default_value;
                //    inputDropdown.GetComponent<MoccaTMPDropdownHelper>().SetType(nodeInput.parameterType, () =>
                //    {
                //        inputDropdown.value = Constants.GetDropdownValueIndex(defaultValue, nodeInput.parameterType);
                //    });
                //    //inputDropdown.value = Constants.GetDropdownValueIndex(nodeData.inputs[ix].default_value, nodeInput.parameterType);
                //    nodeInput.input.default_value = nodeData.inputs[ix].default_value;
                //}

                // MCNode 입력에 추가.
                inputs[ix] = nodeInput;
            }
        }

        protected void SetDropdownType(DataType dataType, RectTransform input)
        {
            switch (dataType)
            {
                case DataType.FACIAL:
                case DataType.MOBILITY:
                case DataType.MOTION:
                    input.GetComponent<MoccaDropdownHelper>().SetType(dataType);
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
                case DataType.EXPRESSION:
                    return dropdownPrefab;

                default: return inputfieldPrefab;
            }
        }

        protected bool IsInputDropdownType(DataType type)
        {
            return type == DataType.FACIAL
                || type == DataType.MOTION
                || type == DataType.MOBILITY
                || type == DataType.EXPRESSION;
        }

        protected void InitOutputs(float yOffset /* 블록 크기의 Y위치 값 */)
        {
            // 출력 프리팹 생성 및 위치 설정(출력 개수 대로 생성).
            //List<RectTransform> outputRT = new List<RectTransform>();
            outputs = new MCNodeOutput[nodeData.outputs.Length];
            for (int ix = 0; ix < nodeData.outputs.Length; ++ix)
            {
                // 출력 소켓 생성.
                RectTransform output = Instantiate(outputSocketPrefab, transform).GetComponent<RectTransform>();
                Vector2 position = epRight.anchoredPosition;
                //position.y = yOffset - ySizeOffset * (ix + 1);
                position.y = yOffset - ySizeOffset * ix;
                output.anchoredPosition = position;

                outputSockets.Add(output.gameObject);

                MCNodeOutput nodeOutput = output.GetComponent<MCNodeOutput>();
                nodeOutput.parameterType = nodeData.outputs[ix].type;
                nodeOutput.SetOutputData(nodeData.outputs[ix]);

                // 라인 색상 설정.
                nodeOutput.SetLineColor(Utils.GetParameterColor(nodeData.outputs[ix].type));

                // MCNode 출력에 추가.
                outputs[ix] = nodeOutput;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            MCWorkspaceManager.Instance.SubscribeFunctionUpdate(OnFunctionUpdate);

            // 블록 초기화.
            //InitializeBlock();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MCWorkspaceManager.Instance.UnsubscribeFunctionUpdate(OnFunctionUpdate);
        }

        public void OpenFunctionTab()
        {
            MCFunctionTable table = FindObjectOfType<MCFunctionTable>();

            Utils.GetTabManager().CreateTab(FunctionName, Constants.OwnerGroup.FUNCTION);
        }

        private void OnFunctionUpdate(FunctionData data)
        {
            // data null 체크.
            if (data == null)
            {
                return;
            }

            // 같은 함수가 맞는지 확인.
            if (Utils.IsTheSameFunction(FunctionID, data.functionID, FunctionName, data.name) == false)
            {
                return;
            }

            // Todo.
            // 함수 삭제 처리 해야함.
            if (data == null || FunctionID != data.functionID)
            {
                return;
            }

            // 함수 노드에서 필요한 업데이트 처리.
            FunctionName = data.name;

            // Todo.
            // 입/출력 내용 변경됐을 때(입력 타입, 추가, 삭제 등)에 따른 업데이트.
            // -> 연결돼있던 선 삭제, 소켓 삭제, 소켓 추가 등.

            // 소켓 초기화.
            ResetSockets();

            // 입출력 데이터 설정.
            SetInput(data.inputs, true);
            SetOutput(data.outputs, true);

            // 블록 다시 연결.
            InitializeBlock(true);

            // 선 복구.
            CreateInputOutputLine();
        }

        private void CreateInputOutputLine()
        {
            if (tables is null)
            {
                tables = Utils.GetTables();
            }

            // 함수 노드를 source로 가진 노드와 선연결.
            foreach (MCNode node in tables.locatedNodes)
            {
                if (node.nodeData.inputs != null)
                {
                    if (node.NodeID.Equals(NodeID) == true)
                    {
                        continue;
                    }

                    //foreach (PROJECT.Input input in node.nodeData.inputs)
                    for (int ix = 0; ix < node.NodeInputCount; ++ix)
                    {
                        PROJECT.Input input = node.GetNodeInputWithIndex(ix).input;
                        if (input is null)
                        {
                            continue;
                        }

                        // 노드의 입력이 함수 노드를 가리키는 경우.
                        if (input.source.Equals(NodeID))
                        {
                            MCNodeOutput left = GetNodeOutputWithIndex(input.subid);
                            //MCNodeInput right = node.GetNodeInputWithIndex(input.id);
                            MCNodeInput right = node.GetNodeInputWithIndex(ix);

                            if (left == null || right == null)
                            {
                                continue;
                            }

                            if (left.CheckTargetSocketType(right.socketType, right) == false)
                            {
                                if (left.line != null)
                                {
                                    MCWorkspaceManager.Instance.RequestLineDelete(left.line.LineID);

                                    //Utils.LogRed("left.line != null");
                                }
                                else if (right.line != null)
                                {
                                    MCWorkspaceManager.Instance.RequestLineDelete(right.line.LineID);

                                    //Utils.LogRed("right.line != null");
                                }

                                left.RemoveLine();
                                left.LineDeleted();

                                //if (left.Node.nodeData.outputs != null
                                //    && input.subid <= left.Node.nodeData.outputs.Length -1)
                                //{
                                //    left.Node.nodeData.outputs[input.subid].value = string.Empty;
                                //}
                                if (left.output != null)
                                {
                                    left.output.value = string.Empty;
                                    //left.Node.nodeData.outputs[input.subid].value = string.Empty;
                                }

                                right.RemoveLine();
                                right.LineDeleted();

                                //if (right.Node.nodeData.inputs != null
                                //    && ix <= right.Node.nodeData.inputs.Length - 1)
                                //{
                                //    right.Node.nodeData.inputs[ix].source = -1;
                                //    right.Node.nodeData.inputs[ix].subid = -1;
                                //}
                                if (right.input != null)
                                {
                                    //right.Node.nodeData.inputs[input.id].source = -1;
                                    //right.Node.nodeData.inputs[input.id].subid = -1;
                                    right.Node.nodeData.inputs[ix].source = -1;
                                    right.Node.nodeData.inputs[ix].subid = -1;
                                }

                                continue;
                            }

                            // 기존에 연결됐던 선 삭제 후 새로 생성.
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
                            //left.SetOutputData(left.output);

                            right.LineSet();
                            //right.SetInputData(right.input);
                        }
                    }
                }
            }

            // 함수 노드 입력(input)과 연결된 선 연결.
            //foreach (PROJECT.Input input in nodeData.inputs)
            for (int ix = 0; ix < nodeData.inputs.Length; ++ix)
            {
                PROJECT.Input input = nodeData.inputs[ix];
                if (input.source.Equals(-1) || input.source.Equals(0))
                {
                    continue;
                }

                MCNode targetNode = MCWorkspaceManager.Instance.FindNodeWithID(input.source);
                if (targetNode is null)
                {
                    continue;
                }

                MCNodeOutput left = targetNode.GetNodeOutputWithIndex(input.subid);
                //if (left is null)
                //{
                //    Debug.Log($"<color=red>left is null. input.source: {input.source} / input.subid: {input.subid}</color>");
                //    continue;
                //}

                MCNodeInput right = GetNodeInputWithIndex(input.id);
                //if (right is null)
                //{
                //    //Utils.LogRed("right is null");
                //    continue;
                //}

                if (right.HasLine == true)
                {
                    continue;
                }

                if (right.CheckTargetSocketType(left.socketType, left) == false)
                {
                    left.RemoveLine();
                    left.LineDeleted();
                    if (left.Node.nodeData.outputs != null
                        && left.Node.nodeData.outputs.Length > 0
                        && input.subid <= left.Node.nodeData.outputs.Length - 1
                        && left.Node.nodeData.outputs[input.subid] != null)
                    {
                        left.Node.nodeData.outputs[input.subid].value = string.Empty;
                    }

                    right.RemoveLine();
                    right.LineDeleted();
                    if (right.Node.nodeData.inputs != null
                        && right.Node.nodeData.inputs.Length > 0
                        && input.id <= right.Node.nodeData.inputs.Length - 1
                        //&& right.Node.nodeData.inputs[input.id] != null)
                        && right.Node.nodeData.inputs[ix] != null)
                    {
                        //right.Node.nodeData.inputs[input.id].source = -1;
                        //right.Node.nodeData.inputs[input.id].subid = -1;
                        right.Node.nodeData.inputs[ix].source = -1;
                        right.Node.nodeData.inputs[ix].subid = -1;
                    }

                    continue;
                }

                // 기존 라인 삭제 후 새로 생성.
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

                //Debug.Log("Line Added 2");

                line.SetLinePoint(left, right);
                MCWorkspaceManager.Instance.AddLine(line);

                left.LineSet();
                //left.SetOutputData(left.output);

                right.LineSet();
                //right.SetInputData(right.input);
            }

            //Utils.LogGreen("[CreateInputOutputLine]");

            // 소켓 리셋 이후에 선이 연결됐는데, 소켓 표시에는 선이 연결 안되는 경우가 발생함.
            // 한번 더 소켓 상태를 확인하기 위해 호출.
            //Invoke("CheckLineSockets", 0.1f);
            Utils.GetTables().DelayCheckSocketState();
        }

        private MCTables tables = null;
        private void ResetSockets()
        {
            if (tables is null)
            {
                tables = FindObjectOfType<MCTables>();
            }

            // 함수 명세가 변경되면 일단 Socket/Input/Dropdown 모두 삭제한 다음,다시 생성.
            // 입력 소켓.
            if (inputSocketAndInputList != null && inputSocketAndInputList.Count > 0)
            {
                foreach (GameObject go in inputSocketAndInputList)
                {
                    if (go.GetComponent<MCNodeSocket>() != null)
                    {
                        MCNodeSocket socket = go.GetComponent<MCNodeSocket>();
                        if (socket.HasLine == true)
                        {
                            for (int ix = tables.locatedLines.Count - 1; ix >= 0; --ix)
                            {
                                MCBezierLine line = tables.locatedLines[ix];
                                if (line.LineID.Equals(socket.line.LineID))
                                {
                                    UpdateNodeData();

                                    socket.line.UnsubscribeAllDragNodes();
                                    socket.line.dontChangeSocketInfo = true;
                                    Destroy(socket.line.gameObject);
                                    tables.RemoveLine(line);
                                    break;
                                }
                            }
                        }
                    }

                    Destroy(go);
                }

                inputSocketAndInputList.Clear();
            }

            // 출력 소켓.
            if (outputSockets != null && outputSockets.Count > 0)
            {
                foreach (GameObject go in outputSockets)
                {
                    if (go.GetComponent<MCNodeSocket>() != null)
                    {
                        MCNodeSocket socket = go.GetComponent<MCNodeSocket>();
                        if (socket.HasLine == true && socket.line != null)
                        {
                            foreach (MCBezierLine line in tables.locatedLines)
                            {
                                if (socket is MCNodeOutput)
                                {
                                    MCNodeOutput nodeOutput = socket as MCNodeOutput;

                                    //Utils.LogGreen($"Here 1, nodeOutput.lines.Count: {nodeOutput.lines.Count}");

                                    foreach (MCBezierLine socketLine in nodeOutput.lines)
                                    {
                                        if (line.LineID.Equals(socketLine.LineID))
                                        {
                                            //Utils.LogGreen("Here 2");

                                            //socketLine.left.Node.MakeNode();
                                            //socketLine.right.Node.MakeNode();

                                            socketLine.UnsubscribeAllDragNodes();
                                            socketLine.dontChangeSocketInfo = true;
                                            Destroy(socketLine.gameObject);
                                            tables.RemoveLine(socketLine);
                                            break;
                                        }
                                    }

                                    nodeOutput.lines.Clear();
                                }
                            }
                        }
                    }

                    Destroy(go);
                }

                outputSockets.Clear();
            }
        }

        public void InitializeBlock(bool isFunctionUpdate = false)
        {
            // 블록 크기 설정.
            Vector2 size = SetTotalSizeOfBlock();

            // 첫번째 소켓의 y위치 계산.
            Vector2 socketPos = SetPositionOfFirstSocket(size);
            float yOffset = socketPos.y;

            // 실행 소켓에 첫번째 y위치 설정.
            SetPositionOfFirstExecuteSocket(socketPos);

            // 입력 프리팹 생성 및 위치 설정(입력 개수 대로 생성).
            //InitInputs(yOffset, isFunctionUpdate);

            InitInputs(yOffset - offsetBWexecSocketAndFirstSocket, isFunctionUpdate);
            //StartCoroutine(InitInputs(yOffset - offsetBWexecSocketAndFirstSocket, isFunctionUpdate));

            // 출력 프리팹 생성 및 위치 설정(출력 개수 대로 생성).
            //InitOutputs(yOffset);
            InitOutputs(yOffset - offsetBWexecSocketAndFirstSocket);
        }

        public void SetInput(PROJECT.Input[] inputs, bool shouldReset = false)
        {
            List<PROJECT.Input> prevInputs = new List<PROJECT.Input>();
            if (shouldReset is true)
            {
                foreach (PROJECT.Input prevInput in nodeData.inputs)
                {
                    prevInputs.Add(prevInput);
                }

                if (nodeData.inputs != null && nodeData.inputs.Length > 0)
                {
                    for (int ix = 0; ix < nodeData.inputs.Length; ++ix)
                    {
                        nodeData.inputs[ix] = null;
                    }

                    Array.Clear(nodeData.inputs, 0, nodeData.inputs.Length);
                }
            }

            nodeData.inputs = new PROJECT.Input[inputs.Length];
            for (int ix = 0; ix < nodeData.inputs.Length; ++ix)
            {
                nodeData.inputs[ix] = new PROJECT.Input();
                nodeData.inputs[ix].id = inputs[ix].id;
                nodeData.inputs[ix].type = inputs[ix].type;
                nodeData.inputs[ix].source = inputs[ix].source;
                nodeData.inputs[ix].subid = inputs[ix].subid;
                nodeData.inputs[ix].default_value = inputs[ix].default_value;
                nodeData.inputs[ix].name = inputs[ix].name;

                if (shouldReset is true && ix < prevInputs.Count)
                {
                    if (inputs[ix].type == prevInputs[ix].type)
                    {
                        nodeData.inputs[ix].source = prevInputs[ix].source;
                        nodeData.inputs[ix].subid = prevInputs[ix].subid;
                        nodeData.inputs[ix].default_value = prevInputs[ix].default_value;
                    }
                }
            }
        }

        public void SetOutput(PROJECT.Output[] outputs, bool shouldReset = false)
        {
            if (shouldReset is true)
            {
                if (nodeData.outputs != null && nodeData.outputs.Length > 0)
                {
                    for (int ix = 0; ix < nodeData.outputs.Length; ++ix)
                    {
                        nodeData.outputs[ix] = null;
                    }

                    Array.Clear(nodeData.outputs, 0, nodeData.outputs.Length);
                }
            }

            nodeData.outputs = new Output[outputs.Length];
            for (int ix = 0; ix < nodeData.outputs.Length; ++ix)
            {
                nodeData.outputs[ix] = new Output();
                nodeData.outputs[ix].id = outputs[ix].id;
                nodeData.outputs[ix].type = outputs[ix].type;
                nodeData.outputs[ix].value = outputs[ix].value;
                nodeData.outputs[ix].name = outputs[ix].name;
            }
        }

        protected override void UpdateNodeData()
        {
            //base.UpdateNodeData();
            nodeData.inputs = new PROJECT.Input[inputs.Length];
            for (int ix = 0; ix < inputs.Length; ++ix)
            {
                nodeData.inputs[ix] = new PROJECT.Input();
                nodeData.inputs[ix].id = inputs[ix].input.id;
                nodeData.inputs[ix].type = inputs[ix].input.type;
                nodeData.inputs[ix].source = inputs[ix].input.source;
                nodeData.inputs[ix].subid = inputs[ix].input.subid;
                nodeData.inputs[ix].name = inputs[ix].input.name;

                if (IsInputDropdownType(inputs[ix].input.type) == true)
                {
                    int dropdownValue = -1;
                    if (inputs[ix].altImage != null)
                    {
                        dropdownValue = inputs[ix].altImage.GetComponent<TMP_Dropdown>().value;
                    }

                    if (dropdownValue != -1)
                    {
                        switch (inputs[ix].input.type)
                        {
                            case DataType.EXPRESSION:
                                nodeData.inputs[ix].default_value
                                    = Constants.GetExpressionVariableListTMPOptionData[dropdownValue].text;
                                break;
                            case DataType.FACIAL:
                                nodeData.inputs[ix].default_value
                                    = Constants.facialListData[dropdownValue].nameEnglish;
                                break;
                            case DataType.MOTION:
                                nodeData.inputs[ix].default_value
                                    = Constants.motionListData[dropdownValue].nameEnglish;
                                break;
                            case DataType.MOBILITY:
                                nodeData.inputs[ix].default_value
                                    = Constants.mobilityListData[dropdownValue].nameEnglish;
                                break;
                            default: break;
                        }
                    }
                }
                else
                {
                    nodeData.inputs[ix].default_value = inputs[ix].input.default_value;
                }

                if (inputs[ix].HasLine)
                {
                    nodeData.inputs[ix].default_value = string.Empty;
                }
            }

            nodeData.outputs = new Output[outputs.Length];
            for (int ix = 0; ix < outputs.Length; ++ix)
            {
                nodeData.outputs[ix] = new Output();
                nodeData.outputs[ix].id = outputs[ix].output.id;
                nodeData.outputs[ix].type = outputs[ix].output.type;
                nodeData.outputs[ix].value = outputs[ix].output.value;
                nodeData.outputs[ix].name = outputs[ix].output.name;
            }

            nodeData.nexts = new Next[nexts.Length];
            for (int ix = 0; ix < nexts.Length; ++ix)
            {
                nodeData.nexts[ix] = new Next();
                nodeData.nexts[ix].value = nexts[ix].next.value;
                nodeData.nexts[ix].next = nexts[ix].next.next;
            }

            // Test.
            // BreakPoint 옵션 설정.
            // 0:설정 안됨, 1:설정됨.
            nodeData.hasBreakPoint = hasBreakPoint;

            LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithName(FunctionName);
            if (function == null)
            {
                function = MCWorkspaceManager.Instance.GetFunctionItemWithID(FunctionID);
            }

            if (function == null)
            {
                Utils.LogRed("Can't find the function");
                return;
            }

            Body body = new Body();
            body.name = FunctionName;
            body.type = DataType.FUNCTION;

            StartID = function.FunctionData.startID;
            body.value = "{\"start_id\": " + StartID + "}";
            nodeData.body = body;
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            FunctionName = node.body.name;

            SetInput(node.inputs);
            SetOutput(node.outputs);
            //LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithName(node.body.name);
            LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithID(FunctionID);
            if (function == null)
            {
                function = MCWorkspaceManager.Instance.GetFunctionItemWithName(node.body.name);
            }

            if (function == null)
            {
                Utils.LogRed("Can't find the function");
                MCWorkspaceManager.Instance.RequestNodeDelete(this);
                return;
            }

            StartID = function.FunctionData.startID;
            InitializeBlock();

            Body body = new Body();
            body.name = node.body.name;
            body.type = DataType.FUNCTION;
            body.value = "{\"start_id\": " + StartID + "}";
            nodeData.body = body;

            if (FunctionName.Equals(function.name) == false)
            {
                // 함수 이름 확인.
                // 함수 탭에서 함수 설정 창을 열고 이름을 변경한 다음, 프로젝트 탭으로 이동하면,
                // 이미 배치돼있는 함수 노드에는 반영안되서 해야함.
                FunctionName = function.name;
            }
        }

        private int GetStartIDFromBodyValue(string bodyValue)
        {
            string[] split = bodyValue
                .Split(new char[] { ':' })[1]
                .Split(new char[] { '}' });

            int retNumber = -1;
            int.TryParse(split[0], out retNumber);

            return retNumber;
        }
    }
}