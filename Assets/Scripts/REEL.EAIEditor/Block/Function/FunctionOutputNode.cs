using REEL.PROJECT;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class FunctionOutputNode : MCNode
    {
        public RectTransform epLeft;

        public GameObject inputSocketPrefab;
        public GameObject inputfieldPrefab;
        public GameObject dropdownPrefab;

        public List<PROJECT.Output> functionOutputs = new List<PROJECT.Output>();

        public float socketOriginYPos = -19f;
        public float offsetBWExecSocketAndFirstSocket = 49f;
        public float blockOriginYSize = 90f;
        public float ySizeOffset = 22.5f;

        private LeftMenuFunctionItem targetFunctionItem;

        private List<GameObject> socketList = new List<GameObject>();

        protected override void OnEnable()
        {
            base.OnEnable();

            MCNodeStart nodeStart = epLeft.GetComponent<MCNodeStart>();
            nodeStart.SubscribeOnLineConnected(OnLastNodeConnected);
            nodeStart.SubscribeOnLineDelete(OnLastNodeDisconnected);
            nodeStart.SubscribeOnRemoveLine(OnLastNodeDisconnected);

            targetFunctionItem = MCWorkspaceManager.Instance.GetFunctionItemWithName(Utils.CurrentTabName);

            MCWorkspaceManager.Instance.SubscribeFunctionUpdate(OnFunctionUpdate);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MCNodeStart nodeStart = epLeft.GetComponent<MCNodeStart>();
            nodeStart.UnSubscribeOnLineConnected(OnLastNodeConnected);
            nodeStart.UnSubscribeOnLineDelete(OnLastNodeDisconnected);
            nodeStart.UnSubscribeOnRemoveLine(OnLastNodeDisconnected);

            MCWorkspaceManager.Instance.UnsubscribeFunctionUpdate(OnFunctionUpdate);

            targetFunctionItem = null;
        }

        public void SetOutputInfo(PROJECT.Output[] outputs, bool shouldReset = false)
        {
            if (shouldReset is true)
            {
                for (int ix = 0; ix < functionOutputs.Count; ++ix)
                {
                    functionOutputs[ix] = null;
                }

                functionOutputs.Clear();
            }

            foreach (PROJECT.Output output in outputs)
            {
                functionOutputs.Add(output);
            }
        }

        protected Vector2 SetTotalSizeOfBlock()
        {
            // 블록 크기 설정.
            RectTransform refRect = GetComponent<RectTransform>();
            Vector2 size = refRect.sizeDelta;
            //size.y = blockOriginYSize + functionOutputs.Count * ySizeOffset;
            size.y = blockOriginYSize + Mathf.Max(0, functionOutputs.Count - 1) * ySizeOffset;
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
            Vector2 newPos = epLeft.anchoredPosition;
            newPos.y = yOffset;

            return newPos;
        }

        protected void SetPositionOfFirstExecuteSocket(Vector2 firstSocketPos)
        {
            // 실행 소켓에 첫번째 y위치 설정.
            epLeft.anchoredPosition = firstSocketPos;
            firstSocketPos.x = epLeft.anchoredPosition.x;
            epLeft.anchoredPosition = firstSocketPos;
        }

        protected void InitOutputs(float yOffset /* 블록 크기의 Y위치 값 */)
        {
            // 출력 프리팹 생성 및 위치 설정(출력 개수 대로 생성).
            List<RectTransform> outputRT = new List<RectTransform>();
            inputs = new MCNodeInput[functionOutputs.Count];

            Node functionOutputNode = null;
            foreach (Node node in targetFunctionItem.FunctionData.nodes)
            {
                if (node.type == NodeType.FUNCTION_OUTPUT)
                {
                    functionOutputNode = node;
                }
            }

            if (inputs != null && functionOutputNode != null && functionOutputNode.inputs != null)
            {
                if (inputs.Length != functionOutputNode.inputs.Length)
                {
                    functionOutputNode.inputs = new PROJECT.Input[inputs.Length];
                    for (int ix = 0; ix < functionOutputNode.inputs.Length; ++ix)
                    {
                        functionOutputNode.inputs[ix] = new PROJECT.Input();
                    }
                }
            }

            for (int ix = 0; ix < functionOutputs.Count; ++ix)
            {
                // 출력 소켓 생성.
                RectTransform output = Instantiate(inputSocketPrefab, transform).GetComponent<RectTransform>();
                Vector2 position = epLeft.anchoredPosition;
                //position.y = yOffset - ySizeOffset * (ix + 1);
                position.y = yOffset - ySizeOffset * ix;
                output.anchoredPosition = position;

                socketList.Add(output.gameObject);

                MCNodeInput nodeInput = output.GetComponent<MCNodeInput>();
                nodeInput.parameterType = functionOutputs[ix].type;

                // 라인 색상 설정.
                nodeInput.SetLineColor(Utils.GetParameterColor(functionOutputs[ix].type));

                // MCNode 출력에 추가.
                inputs[ix] = nodeInput;

                // input id 설정.
                inputs[ix].input = new PROJECT.Input();
                inputs[ix].input.id = ix;
                if (functionOutputNode != null)
                {
                    //inputs[ix].input.type = functionOutputNode.inputs[ix].type;
                    //inputs[ix].input.source = functionOutputNode.inputs[ix].source;
                    //inputs[ix].input.subid = functionOutputNode.inputs[ix].subid;
                    inputs[ix].input.type = nodeInput.parameterType;
                    inputs[ix].input.source = functionOutputNode.inputs[ix].source;
                    inputs[ix].input.subid = functionOutputNode.inputs[ix].subid;
                }
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

            // 출력 프리팹 생성 및 위치 설정(출력 개수 대로 생성).
            //InitOutputs(yOffset);
            InitOutputs(yOffset - offsetBWExecSocketAndFirstSocket);
        }

        private void OnLastNodeConnected(MCBezierLine line)
        {
            if (line == null)
            {
                return;
            }

            int nodeID = line.left.Node.NodeID;
            int socketIndex = line.left.Node.GetNodeNextIndexWithSocket(line.left);
            if (CheckIfLastNodeAlreadyConnected(nodeID, socketIndex) == false)
            {
                FunctionLastNodeInfo lastNodeInfo = new FunctionLastNodeInfo()
                {
                    lineID = line.LineID,
                    nodeID = nodeID,
                    socketIndex = socketIndex
                };

                targetFunctionItem.FunctionData.lastNodes.Add(lastNodeInfo);
            }

            //targetFunctionItem.FunctionData.lastNode.nodeID = line.left.Node.NodeID;
            //targetFunctionItem.FunctionData.lastNode.socketIndex = line.left.Node.GetNodeNextIndexWithSocket(line.left);
            //targetFunctionItem.FunctionData.lastID = line.left.Node.NodeID;
        }

        private bool CheckIfLastNodeAlreadyConnected(int nodeID, int socketIndex)
        {
            foreach (FunctionLastNodeInfo info in targetFunctionItem.FunctionData.lastNodes)
            {
                if (info.nodeID.Equals(nodeID) && info.socketIndex.Equals(socketIndex))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnLastNodeDisconnected(int lineID)
        {
            //Utils.LogRed($"[FunctionOutputNode.OnLastNodeDisconnected] lineID: {lineID}");
            for (int ix = targetFunctionItem.FunctionData.lastNodes.Count - 1; ix >= 0; --ix)
            {
                FunctionLastNodeInfo info = targetFunctionItem.FunctionData.lastNodes[ix];
                if (info.lineID.Equals(-1) == false && info.lineID.Equals(lineID))
                {
                    //Utils.LogRed($"[FunctionOutputNode.OnLastNodeDisconnected] in if branch lineID: {lineID}");
                    targetFunctionItem.FunctionData.lastNodes.RemoveAt(ix);
                    break;
                }
            }
        }

        private void OnLastNodeDisconnected()
        {
            //targetFunctionItem.FunctionData.lastID = -2;
            //targetFunctionItem.FunctionData.lastID = -1;

            //targetFunctionItem.FunctionData.lastNode.nodeID = -1;
            //targetFunctionItem.FunctionData.lastNode.socketIndex = 0;

            //Utils.LogRed("[OnLastNodeDisconnected]");

            targetFunctionItem.FunctionData.lastNodes.Clear();
        }

        private void OnFunctionUpdate(PROJECT.FunctionData functionData)
        {
            if (functionData == null)
            {
                return;
            }

            if (Utils.IsTheSameFunction(
                targetFunctionItem.FunctionData.functionID,
                functionData.functionID,
                targetFunctionItem.FunctionData.name,
                functionData.name) == false)
            {
                return;
            }

            ResetSockets();

            SetOutputInfo(functionData.outputs, true);
            InitializeBlock();

            CreateInputLine(functionData);

            //Utils.LogRed("[FunctionOutputNode.OnFunctionUpdate]");
        }

        private void ResetSockets()
        {
            MCTables tables = FindObjectOfType<MCTables>();

            // 함수 명세가 변경되면 일단 Socket/Input/Dropdown 모두 삭제한 다음,다시 생성.
            if (socketList != null && socketList.Count > 0)
            {
                foreach (GameObject go in socketList)
                {
                    if (go.GetComponent<MCNodeSocket>() != null)
                    {
                        MCNodeSocket socket = go.GetComponent<MCNodeSocket>();
                        if (socket.HasLine == true)
                        {
                            //foreach (MCBezierLine line in tables.locatedLines)
                            for (int ix = tables.locatedLines.Count - 1; ix >= 0; --ix)
                            {
                                MCBezierLine line = tables.locatedLines[ix];
                                if (line.LineID.Equals(socket.line.LineID))
                                {
                                    socket.line.UnsubscribeAllDragNodes();
                                    socket.line.dontChangeSocketInfo = true;
                                    Destroy(socket.line.gameObject);
                                    tables.RemoveLine(socket.line);
                                }
                            }
                        }
                    }

                    Destroy(go);
                }

                socketList.Clear();
            }
        }

        private LeftMenuFunctionItem GetCurrentFunction(FunctionData functionData)
        {
            //LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithName(functionData.name);
            LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithID(functionData.functionID);

            return function;
        }

        private void CreateInputLine(FunctionData functionData)
        {
            //LeftMenuFunctionItem function = GetCurrentFunction(functionData);
            foreach (PROJECT.Input input in nodeData.inputs)
            {
                if (input.source != -1)
                {
                    MCNode targetNode = MCWorkspaceManager.Instance.FindNodeWithID(input.source);
                    if (targetNode != null)
                    {
                        MCNodeOutput left = targetNode.GetNodeOutputWithIndex(input.subid);
                        MCNodeInput right = GetNodeInputWithIndex(input.id);
                        if (left == null || right == null)
                        {
                            if (left == null && right != null)
                            {
                                right.RemoveLine();
                                right.LineDeleted();
                                continue;
                            }
                            if (left != null && right == null)
                            {
                                left.RemoveLine();
                                left.LineDeleted();
                                continue;
                            }
                            if (left == null && right == null)
                            {
                                continue;
                            }
                        }

                        if (right.CheckTargetSocketType(left.socketType, left) == false)
                        {
                            left.RemoveLine();
                            left.LineDeleted();

                            right.RemoveLine();
                            right.LineDeleted();
                            continue;
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
        }
    }
}