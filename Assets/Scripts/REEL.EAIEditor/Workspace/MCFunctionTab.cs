using REEL.PROJECT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCFunctionTab : TabComponent
    {
        public GameObject functionInputNodePrefab;
        public GameObject functionOutputNodePrefab;

        public Vector2 inputStartPosition = new Vector2(140f, -200f);
        public Vector2 outputStartPosition = new Vector2(900f, -200f);

        private FunctionInputNode inputNode = null;
        private FunctionOutputNode outputNode = null;

        public int FunctionID { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            ownerGroup = Constants.OwnerGroup.FUNCTION;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            inputNode = null;
            outputNode = null;

            MCWorkspaceManager.Instance.UnsubscribeFunctionUpdate(OnFunctionUpdate);
        }

        public void CreateInputOutputNode(LeftMenuFunctionItem function)
        {
            int inputNodeID = 0;
            int outputNodeID = 0;
            foreach (var node in function.FunctionData.nodes)
            {
                if (node.type == NodeType.FUNCTION_INPUT)
                {
                    inputNodeID = node.id;
                }
                else if (node.type == NodeType.FUNCTION_OUTPUT)
                {
                    outputNodeID = node.id;
                }
            }

            inputNodeID = inputNodeID == 0 ? Utils.NewGUID : inputNodeID;

            FunctionInputNode inputNode = Utils.GetGraphPane()
                .AddNode(inputStartPosition, functionInputNodePrefab, inputNodeID)
                .GetComponent<FunctionInputNode>();

            inputNode.SetInputInfo(function.FunctionData.inputs);
            inputNode.InitializeBlock();

            outputNodeID = outputNodeID == 0 ? Utils.NewGUID : outputNodeID;
            FunctionOutputNode outputNode = Utils.GetGraphPane()
                .AddNode(outputStartPosition, functionOutputNodePrefab, outputNodeID)
                .GetComponent<FunctionOutputNode>();

            outputNode.SetOutputInfo(function.FunctionData.outputs);
            outputNode.InitializeBlock();

            this.inputNode = inputNode;
            this.outputNode = outputNode;
        }

        public void DeleteInputOutputNode()
        {
            if (inputNode != null)
            {
                Destroy(inputNode.gameObject);
            }
            if (outputNode != null)
            {
                Destroy(outputNode.gameObject);
            }
            
            inputNode = null;
            outputNode = null;
        }

        protected override void OnSelected()
        {
            base.OnSelected();

            // 프로젝트 -> 함수 탭 전환.
            // 1.기존에 추가됐던 지역변수 삭제.
            MCVariableFunctionManager manager = FindObjectOfType<MCVariableFunctionManager>();
            if (manager != null)
            {
                manager.RemoveAllLocalVariables();
            }
            
            // 2. 함수 로직 노드 로드.
            LoadFunctionLogic();
        }

        private void OnFunctionUpdate(FunctionData data)
        {
            // 탭이 열린 상태에서, 함수가 삭제됐는지 확인.
            // 이 때는 탭을 닫아야함.
            if (data == null)
            {
                return;
            }

            // 같은 함수가 맞는지 확인.
            //if (data.functionID != FunctionID)
            //{
            //    return;
            //}
            if (Utils.IsTheSameFunction(data.functionID, FunctionID, data.name, tabUI.TabName) == false)
            {
                return;
            }

            tabUI.TabName = data.name;
        }

        private void LoadFunctionLogic()
        {
            // 변수 생성.
            //MCWorkspaceManager.Instance.CreateVariableFromProject();

            // Todo.
            // 프로젝트 탭이 열린 상태에서 함수 명세가 변경됐을 수도 있기 때문에,
            // 함수 탭을 열면 input/output 소켓 간의 데이터 확인이 추가적으로 필요함.
            // 함수 명세가 변경되기 전에는 연결 가능했지만, 변경되면서 연결 불가능해질 수 있기 때문.

            // 1. 입출력 노드 생성.
            LeftMenuFunctionItem functionItem = CurrentFunctionItem;
            MCWorkspaceManager.Instance.SubscribeFunctionUpdate(OnFunctionUpdate);

            // 지역 변수 생성.
            if (functionItem.FunctionData.variables != null)
            {
                foreach (Node localVariable in functionItem.FunctionData.variables)
                {
                    MCWorkspaceManager.Instance.AddLocalVariable(
                        localVariable.body.name,
                        localVariable.body.type,
                        localVariable.type,
                        localVariable.body.value,
                        functionItem.nameText.text,
                        true
                    );
                }
            }

            //CreateInputOutputNode(functionItem.Inputs, functionItem.Outputs);

            // FunctionID 설정.
            FunctionID = functionItem.FunctionID;

            // 입출력 노드 생성.
            CreateInputOutputNode(functionItem);

            //// 함수 입력을 지역변수로 추가(20211202).
            //if (functionItem.Inputs != null && functionItem.Inputs.Length > 0)
            //{
            //    foreach (var input in functionItem.Inputs)
            //    {
            //        int index = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(input.name);
            //        var localVariable = MCWorkspaceManager.Instance.GetLocalVariable(index);

            //        // 파라미터가 지역변수가 추가되지 않았을 때만 추가.
            //        if (localVariable == null)
            //        {
            //            MCWorkspaceManager.Instance.AddLocalVariable(
            //                input.name,
            //                input.type,
            //                NodeType.VARIABLE,
            //                "",
            //                functionItem.FunctionData.name
            //            );
            //        }
            //    }
            //}

            if (functionItem.FunctionData == null || functionItem.FunctionData.nodes == null)
            {
                return;
            }

            // 2. FunctionData에 저장된 노드 로드.
            foreach (var node in functionItem.FunctionData.nodes)
            {
                //if (node.type == PROJECT.NodeType.FUNCTION_INPUT || node.type == PROJECT.NodeType.FUNCTION_OUTPUT)
                //{
                //    continue;
                //}

                // 입력 노드 위치 설정.
                if (node.type == NodeType.FUNCTION_INPUT)
                {
                    if (inputNode != null)
                    {
                        inputNode.transform.localPosition = node.nodePosition;
                    }

                    continue;
                }

                // 출력 노드 위치 설정.
                else if (node.type == NodeType.FUNCTION_OUTPUT)
                {
                    if (outputNode != null)
                    {
                        outputNode.transform.localPosition = node.nodePosition;
                    }

                    continue;
                }

                if (node.type == NodeType.BYPASS)
                {
                    foreach (var node2 in functionItem.FunctionData.nodes)
                    {
                        foreach (var node2Input in node2.inputs)
                        {
                            if (node2Input.source.Equals(node.id))
                            {
                                node2Input.source = inputNode.NodeID;
                                node2Input.subid = node.inputs[0].subid;
                            }
                        }
                    }

                    continue;
                }

                // 노드의 입력이 Bypass를 참조하면 입력 노드를 참조하도록 source id 변경.
                // -> 로드시에만 처리 (UI에서 입력노드와 연결되도록 하기 위해서).
                //foreach (var inputSource in node.inputs)
                //for (int ix = 0; ix < node.inputs.Length; ++ix)
                //{
                //    foreach (var node2 in functionItem.FunctionData.nodes)
                //    {
                //        if (!node2.id.Equals(node.inputs[ix].source))
                //        {
                //            continue;
                //        }

                //        if (node2.type == NodeType.BYPASS)
                //        {
                //            node.inputs[ix].source = inputNode.NodeID;
                //            node.inputs[ix].subid = node2.inputs[0].subid;
                //        }
                //    }
                //}

                // Next id가 -2인 경우(함수의 최종 노드)에는 함수 출력 노드와 연결.
                foreach (var next in node.nexts)
                {
                    if (next.next.Equals(-2))
                    {
                        next.next = outputNode.NodeID;
                        break;
                    }
                }

                // 프로젝트 탭에서 변수를 삭제한 다음, 함수 탭으로 넘어오면,
                // 이미 배채했던 Get/Set 노드가 삭제되지 않고 그대로 남아어서 확인 필요.
                if (node.type == NodeType.GET || node.type == NodeType.SET)
                {
                    string variableName = node.body.name;
                    int index = -1;
                    if (node.body.isLocalVariable == true)
                    {
                        index = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(variableName);
                    }
                    else
                    {
                        index = MCWorkspaceManager.Instance.GetVariableIndexWithName(variableName);
                    }

                    //Debug.Log($"[MCFunctionTab] node.type: {node.type} / variableName: {variableName}");

                    // 변수가 검색되지 않으면 -> 삭제되었으면,
                    // 노드 생성하지 않고 루프 건너뜀.
                    if (index.Equals(-1))
                    {
                        continue;
                    }
                }

                MCNode newNode = Utils.GetGraphPane().AddNode(node.nodePosition, node.type, node.id, true, true);
                newNode.SetData(node);

                if (newNode is MCGetNode || newNode is MCSetNode)
                {
                    string variableName = node.body.name;
                    int index = -1;
                    if (node.body.isLocalVariable == true)
                    {
                        index = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(variableName);

                        MoccaTMPDropdownHelper helper = newNode.GetComponentInChildren<MoccaTMPDropdownHelper>();
                        if (helper != null)
                        {
                            helper.type = MoccaTMPDropdownHelper.Type.LocalVariableList;
                            helper.InitializeDropdownOptionData();
                        }
                    }
                    else
                    {
                        index = MCWorkspaceManager.Instance.GetVariableIndexWithName(variableName);
                    }

                    if (index != -1)
                    {
                        if (newNode is MCGetNode)
                        {
                            MCGetNode getNode = newNode as MCGetNode;
                            getNode.IsLocalVariable = node.body.isLocalVariable;
                            getNode.SetDropdownIndex(index);
                        }
                        else if (newNode is MCSetNode)
                        {
                            MCSetNode setNode = newNode as MCSetNode;
                            setNode.IsLocalVariable = node.body.isLocalVariable;
                            setNode.SetDropdownIndex(index);
                        }
                    }
                }
            }

            // 입력 노드 - 함수 Start 노드 간 실행선 연결.
            if (functionItem.FunctionData.startID != -2)
            {
                //int startID = functionItem.StartNodeID == -2 ? 
                //    functionItem.FunctionData.startID : functionItem.StartNodeID;
                int startID = functionItem.FunctionData.startID;
                MCNode leftNode = FunctionInputNode;
                MCNode rightNode = GetNodeWithID(startID);
                if (leftNode != null && rightNode != null)
                {
                    MCNodeNext left = leftNode.GetNodeNextWithIndex(0);
                    MCNodeStart right = rightNode.GetComponentInChildren<MCNodeStart>();

                    MCBezierLine line = Utils.CreateNewLine();

                    //Utils.LogGreen($"[input-start node]leftNode.NodeID: {leftNode.NodeID} / rightNode.NodeID: {rightNode.NodeID}");

                    line.SetLinePoint(left, right);
                    MCWorkspaceManager.Instance.AddLine(line);

                    left.LineSet();
                    right.LineSet();
                }
            }

            // 출력 노드 - 함수 Last 노드 간 실행선 연결.
            foreach (FunctionLastNodeInfo info in functionItem.FunctionData.lastNodes)
            //for (int ix = functionItem.FunctionData.lastNodes.Count - 1; ix >=0; --ix)
            {
                //int lastID = functionItem.LastNodeID == -2 ?
                //    functionItem.FunctionData.lastID : functionItem.LastNodeID;
                int lastID = info.nodeID;
                MCNode leftNode = GetNodeWithID(lastID);
                MCNode rightNode = FunctionOutputNode;
                if (leftNode != null && rightNode != null)
                {
                    int socketIndex = info.socketIndex;
                    //MCNodeNext left = leftNode.GetNodeNextWithIndex(0);
                    MCNodeNext left = leftNode.GetNodeNextWithIndex(socketIndex);
                    MCNodeStart right = rightNode.GetComponentInChildren<MCNodeStart>();

                    if (left != null && right != null)
                    {
                        MCBezierLine line = Utils.CreateNewLine();

                        line.SetLinePoint(left, right);
                        MCWorkspaceManager.Instance.AddLine(line);

                        info.lineID = line.LineID;

                        //left.LineSet();
                        //right.LineSet();
                    }
                }
            }

            //// 출력 노드 - 함수 Last 노드 간 실행선 연결.
            //if (functionItem.FunctionData.lastNode.nodeID != -2)
            //{
            //    //int lastID = functionItem.LastNodeID == -2 ?
            //    //    functionItem.FunctionData.lastID : functionItem.LastNodeID;
            //    int lastID = functionItem.FunctionData.lastNode.nodeID;
            //    MCNode leftNode = GetNodeWithID(lastID);
            //    MCNode rightNode = FunctionOutputNode;
            //    if (leftNode != null && rightNode != null)
            //    {
            //        int socketIndex = functionItem.FunctionData.lastNode.socketIndex;
            //        //MCNodeNext left = leftNode.GetNodeNextWithIndex(0);
            //        MCNodeNext left = leftNode.GetNodeNextWithIndex(socketIndex);
            //        MCNodeStart right = rightNode.GetComponentInChildren<MCNodeStart>();

            //        if (left != null && right != null)
            //        {
            //            MCBezierLine line = Utils.CreateNewLine();

            //            line.SetLinePoint(left, right);
            //            MCWorkspaceManager.Instance.AddLine(line);

            //            //left.LineSet();
            //            //right.LineSet();
            //        }
            //    }
            //}

            // 3. 노드 연결 (실행선/입출력).
            foreach (Node node in functionItem.FunctionData.nodes)
            {
                // 3-1 실행선 연결.
                if (node.nexts != null && node.nexts.Length > 0)
                {
                    for (int ix = 0; ix < node.nexts.Length; ++ix)
                    {
                        Next next = node.nexts[ix];

                        MCNode leftNode = GetNodeWithID(node.id);
                        MCNode rightNode = GetNodeWithID(next.next);

                        if (node.type == NodeType.SAY)
                        {
                            Utils.LogRed("Hello");
                        }

                        if (leftNode != null && leftNode != FunctionInputNode && rightNode != null && rightNode != FunctionOutputNode)
                        {
                            MCNodeNext left = leftNode.GetNodeNextWithIndex(ix);
                            MCNodeStart right = rightNode.GetComponentInChildren<MCNodeStart>();

                            MCBezierLine line = Utils.CreateNewLine();

                            line.SetLinePoint(left, right);
                            MCWorkspaceManager.Instance.AddLine(line);

                            left.LineSet();
                            right.LineSet();
                        }
                    }
                }

                // 3-2 입력 출력 라인 연결.
                CreateNodeInputLines(node);
            }
        }

        // For Test.
        private void CreateNodeInputLines(Node node)
        {
            //Utils.LogRed($"{node.type}");

            MCNode mcNode = GetNodeWithID(node.id);
            if (mcNode != null && mcNode.nodeData.inputs != null)
            {
                foreach (PROJECT.Input input in node.inputs)
                {
                    //Utils.LogRed($"NodeType: {node.type}/input.source: {input.source}");
                    CreateInputLine(node.id, input);

                    if (input != null && input.source != -1)
                    {
                        if (GetNodeWithID(input.source) == null)
                        {
                            //Utils.LogRed($"Can't find the node id: {input.source}");
                            continue;
                        }

                        CreateNodeInputLines(GetNodeWithID(input.source).nodeData);
                    }
                }
            }
        }

        MCTables manager;
        private bool CheckIfSameLineAlreadyPlaced(MCNodeOutput left, MCNodeInput right)
        {
            if (manager is null)
            {
                manager = FindObjectOfType<MCTables>();
            }

            if (left == null || right == null)
            {
                return false;
            }

            foreach (MCBezierLine locatedLine in manager.locatedLines)
            {
                if (locatedLine == null)
                {
                    continue;
                }

                if (locatedLine.left.GetInstanceID().Equals(left.GetInstanceID())
                    && locatedLine.right.GetInstanceID().Equals(right.GetInstanceID()))
                {
                    return true;
                }
            }

            return false;
        }

        private MCNode CreateInputLine(int nodeID, PROJECT.Input input)
        {
            if (input == null)
            {
                return null;
            }
            
            MCNode leftNode = GetNodeWithID(input.source);
            MCNode rightNode = GetNodeWithID(nodeID);

            if (leftNode != null && rightNode != null)
            {
                // 함수 탭 안에 배치된 노드에서는 프로세스 확인은 필요 없을 듯.
                //if (!leftNode.OwnedProcess.id.Equals(rightNode.OwnedProcess.id))
                //{
                //    return null;
                //}

                MCNodeOutput left = leftNode.GetNodeOutputWithIndex(input.subid);
                // 2021.12.02
                // For 루프 선 연결 예외처리.
                // For 루프의 입력이 기존 1개에서 3개로 변경되면서 이전 버전 호환성을 위한 처리.
                //MCNodeInput right = rightNode.GetNodeInputWithIndex(input.id);
                MCNodeInput right = null;
                if (rightNode is MCLoopNode && rightNode.nodeData.inputs != null && rightNode.nodeData.inputs.Length == 1)
                {
                    right = rightNode.GetNodeInputWithIndex(1);
                }
                else
                {
                    right = rightNode.GetNodeInputWithIndex(input.id);
                }

                if (CheckIfSameLineAlreadyPlaced(left, right) == true)
                {
                    return null;
                }

                // 소켓 간 데이터 타입 확인 로직 추가.
                // 프로젝트 탭에서 함수 명세가 바뀐 다음에 함수 탭이 열리면 기존에 있던 정보가 업데이트 되기 전이기 때문에,
                // 타입 확인 필요.
                if (left != null && left.CheckTargetSocketType(right.socketType, right) == false)
                {
                    Debug.Log($"CheckTargetSocketType->false: {left.Node.nodeData.type} / {left.parameterType} / {right.Node.nodeData.type} / {right.parameterType}");
                    return null;
                }

                MCBezierLine line = Utils.CreateNewLine();

                // 왼쪽 노드가 GetNode인 경우 라인 색상 설정.
                if (leftNode != null && leftNode is MCGetNode)
                {
                    MCGetNode getNode = leftNode as MCGetNode;
                    LeftMenuVariableItem variable;
                    if (getNode.IsLocalVariable == true)
                    {
                        variable = MCWorkspaceManager.Instance.GetLocalVariable(getNode.CurrentVariableIndex);
                    }
                    else
                    {
                        variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
                    }
                    
                    bool isList = variable.nodeType == NodeType.LIST;

                    //line.SetLineColor(Utils.GetParameterColor(isList ? DataType.LIST : variable.dataType));
                    line.SetLineColor(Utils.GetParameterColor(isList ? DataType.LIST : right.parameterType));
                }
                else if (leftNode != null && leftNode is MCGetElementNode)
                {
                    MCGetElementNode getElementNode = leftNode as MCGetElementNode;
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

                        //line.SetLineColor(Utils.GetParameterColor(variable.dataType));
                        line.SetLineColor(Utils.GetParameterColor(right.parameterType));
                    }
                }
                // 그 외의 경우 라인 색상 설정.
                else
                {
                    //line.SetLineColor(Utils.GetParameterColor(input.type));
                    line.SetLineColor(Utils.GetParameterColor(right.parameterType));
                }

                if (left != null && right != null)
                {
                    line.SetLinePoint(left, right);
                    MCWorkspaceManager.Instance.AddLine(line);

                    left.SetLine(line);
                    left.LineSet();

                    right.SetLine(line);
                    right.LineSet();
                }
            }

            //else
            //{
            //    if (leftNode == null)
            //    {
            //        Utils.LogRed($"[CreateInputLine] LeftNode is null: {input.source}");
            //    }

            //    if (rightNode == null)
            //    {
            //        Utils.LogRed($"[CreateInputLine] RightNode is null: {nodeID}");
            //    }
            //}

            return leftNode;
        }

        public LeftMenuFunctionItem CurrentFunctionItem
        {
            get
            {
                int index = MCWorkspaceManager.Instance.GetFunctionIndexWithName(TabName);
                return MCWorkspaceManager.Instance.GetFunction(index);
            }
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

        private MCNode FunctionInputNode
        {
            get
            {
                LeftMenuFunctionItem functionItem = CurrentFunctionItem;
                foreach (var node in functionItem.LogicNodes)
                {
                    if (node.nodeData.type == NodeType.FUNCTION_INPUT)
                    {
                        return node;
                    }
                }

                return null;
            }
        }

        private MCNode FunctionOutputNode
        {
            get
            {
                LeftMenuFunctionItem functionItem = CurrentFunctionItem;
                foreach (var node in functionItem.LogicNodes)
                {
                    if (node.nodeData.type == NodeType.FUNCTION_OUTPUT)
                    {
                        return node;
                    }
                }

                return null;
            }
        }

        //protected override void OnUnSelected()
        //{
        //    //// 1. 함수 컴파일.
        //    //int index = MCWorkspaceManager.Instance.GetFunctionIndexWithName(TabName);
        //    //LeftMenuFunctionItem functionItem = MCWorkspaceManager.Instance.GetFunction(index);
        //    //functionItem.CompileFunctionData();

        //    //// 2. 함수 노드 GO 삭제.
        //    //MCFunctionTable.Instance.DeleteAllFunctionNodes();
        //}

        public void CompileFunctionData()
        {
            int index = MCWorkspaceManager.Instance.GetFunctionIndexWithName(TabName);
            LeftMenuFunctionItem functionItem = MCWorkspaceManager.Instance.GetFunction(index);
            if (functionItem == null)
            {
                return;
            }

            functionItem.CompileFunctionData();
        }

        public override void CloseTab()
        {
            if (MCPlayStateManager.Instance.IsSimulation == true)
            {
                return;
            }

            if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup != Constants.OwnerGroup.FUNCTION)
            {
                //Utils.LogGreen("1");
                tabManager.RemoveTab(this);
                return;
            }

            //Utils.LogGreen($"Delete Tab: {TabName} / Current Tab: {tabManager.CurrentTab.TabName}");
            if (TabName.Equals(tabManager.CurrentTab.TabName))
            {
                CompileFunctionData();

                // 1. 함수 노드 GO 삭제.
                //MCFunctionTable.Instance.DeleteAllFunctionNodes();
                LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithName(TabName);
                MCFunctionTable.Instance.DeleteAllFunctionNodes(function);

                // 2. 연결된 선 GO 삭제.
                MCWorkspaceManager.Instance.DeleteAllLines();
            }

            // 2. 탭 삭제.
            tabManager.RemoveTab(this);
        }
    }
};