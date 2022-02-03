using REEL.PROJECT;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCAddNodeByCopyPasteCommand : MCCommand
    {
        List<WillBeCreatedNodeInfo> willbeCreatedInfo = new List<WillBeCreatedNodeInfo>();
        private Dictionary<int, int> idTable = new Dictionary<int, int>();
        List<MCNode> createdNodes = new List<MCNode>();

        // 작성자: 장세윤.
        // 변수/함수가 누락됐을 때 사용하기 위한 리스트.
        private List<VariableInfomation> copiedVariables = new List<VariableInfomation>();
        private List<FunctionData> copiedFunction = new List<FunctionData>();

        Node NodeCopy(Node node)
        {
            string json = JsonUtility.ToJson(node);
            return JsonUtility.FromJson<Node>(json);
        }

        public MCAddNodeByCopyPasteCommand(List<Node> selected)
        {
            Vector2 offset = new Vector2(120f, 120f);

            foreach (Node node in selected)
            {
                //node.MakeNode();
                WillBeCreatedNodeInfo info = new WillBeCreatedNodeInfo();
                info.nodeData = node;
                info.nodeData.nodePosition = node.nodePosition - offset;
                info.nodeID = Utils.NewGUID;

                willbeCreatedInfo.Add(info);
                idTable.Add(node.id, info.nodeID);
            }
        }

        public MCAddNodeByCopyPasteCommand(List<Node> selected,
            List<VariableInfomation> copiedVariables,
            List<FunctionData> copiedFunction)
        {
            Vector2 offset = new Vector2(190f, 0f);

            foreach (Node node in selected)
            {
                //node.MakeNode();
                WillBeCreatedNodeInfo info = new WillBeCreatedNodeInfo();
                info.nodeData = node;
                info.nodeData.nodePosition = node.nodePosition + offset;
                info.nodeID = Utils.NewGUID;

                willbeCreatedInfo.Add(info);
                idTable.Add(node.id, info.nodeID);
            }

            foreach (VariableInfomation info in copiedVariables)
            {
                this.copiedVariables.Add(info);
            }

            foreach (FunctionData data in copiedFunction)
            {
                this.copiedFunction.Add(data);
            }
        }


        public MCAddNodeByCopyPasteCommand(List<MCNode> selected)
        {
            Vector3 offset = new Vector3(120f, 120f);

            foreach (MCNode node in selected)
            {
                node.MakeNode();
                WillBeCreatedNodeInfo info = new WillBeCreatedNodeInfo();
                info.nodeData = NodeCopy(node.nodeData);
                info.nodeData.nodePosition = node.transform.localPosition - offset;
                info.nodeID = Utils.NewGUID;

                willbeCreatedInfo.Add(info);
                idTable.Add(node.NodeID, info.nodeID);
            }
        }

        public void Execute()
        {
            GraphPane graphPane = Utils.GetGraphPane();

            foreach (WillBeCreatedNodeInfo info in willbeCreatedInfo)
            {
                // 추가할 노드가 GET/SET 노드인 경우 변수 존재 여부 확인.
                if (info.nodeData.type == NodeType.GET || info.nodeData.type == NodeType.SET)
                {
                    // 지역 변수일 때 노드 생성하면 안되는 경우 예외처리.
                    if (info.nodeData.body.isLocalVariable == true)
                    {
                        // 현재 탭이 프로젝트라면, 지역 변수는 복사하면 안됨.
                        TabManager tabManager = Utils.GetTabManager();
                        if (tabManager.CurrentTab.OwnerGroup == Constants.OwnerGroup.PROJECT)
                        {
                            continue;
                        }

                        // 지역 변수를 검색했는데 못 찾으면 노드 생성하면 안됨.
                        int variableIndex = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(info.nodeData.body.name);
                        if (variableIndex == -1)
                        {
                            continue;
                        }
                    }

                    int index = MCWorkspaceManager.Instance.GetVariableIndexWithName(info.nodeData.body.name);

                    // 변수 못찾았으면 복사해둔 변수 정보에서 찾기 시도.
                    if (index == -1)
                    {
                        // 복사해둔 변수 정보에서 같은 이름의 변수 정보를 검색한 다음,
                        foreach (VariableInfomation variableInfo in copiedVariables)
                        {
                            //Utils.LogRed($"variableInfo.name {variableInfo.name} / info.nodeData.body.name: {info.nodeData.body.name}");

                            // 같은 이름의 변수 정보가 있으면, 변수 추가.
                            if (variableInfo.name.ToLower().Equals(info.nodeData.body.name.ToLower()))
                            {
                                MCWorkspaceManager.Instance.AddVariable(
                                    variableInfo.name, variableInfo.type, variableInfo.nodeType, variableInfo.value, true
                                );

                                break;
                            }
                        }
                    }
                }

                // 추가할 노드가 Function 노드인 경우, 함수 존재 여부 확인.
                if (info.nodeData.type == NodeType.FUNCTION)
                {
                    int index = MCWorkspaceManager.Instance.GetFunctionIndexWithName(info.nodeData.body.name);

                    // 함수를 못찾았으면 복사해둔 함수 정보에서 찾기 시도.
                    if (index == -1)
                    {
                        // 복사해둔 함수 정보에서 같은 이름의 함수 정보를 검색한 다음,
                        foreach (FunctionData data in copiedFunction)
                        {
                            // 같은 이름의 함수 정보가 있으면, 함수 추가.
                            if (data.name.ToLower().Equals(info.nodeData.body.name.ToLower()))
                            {
                                MCWorkspaceManager.Instance.AddFunction(data.name, data.inputs, data.outputs);
                                LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithName(data.name);
                                function.FunctionData = data;
                                break;
                            }
                        }
                    }
                }

                MCNode node = graphPane.AddNode(info.nodeData.nodePosition, info.nodeData.type, info.nodeID, false, true, true);
                node.SetData(info.nodeData);
                node.IsSelected = true;

                // 함수 노드의 경우 FunctionID 설정.
                if (node is MCFunctionNode)
                {
                    MCFunctionNode functionNode = node as MCFunctionNode;
                    LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithName(info.nodeData.body.name);
                    functionNode.FunctionID = function.FunctionID;
                    functionNode.FunctionName = function.FunctionData.name;
                    //Debug.Log($"{functionNode.FunctionID} / {function.FunctionID}");
                    //Debug.Log($"{functionNode.FunctionID} / {function.FunctionID} / {functionNode.FunctionName}");
                }

                createdNodes.Add(node);
            }

            // 참조 ID 정보 갱신.
            foreach (MCNode node in createdNodes)
            {
                // 다음 실행선 정보 갱신.
                for (int ix = 0; ix < node.NodeNextCount; ++ix)
                {
                    Next next = node.nodeData.nexts[ix];
                    if (next.next != -1)
                    {
                        if (idTable.TryGetValue(next.next, out int newID))
                        {
                            next.next = newID;
                        }
                    }
                }

                // 입력 정보 갱신.
                for (int ix = 0; ix < node.NodeInputCount; ++ix)
                {
                    PROJECT.Input input = node.nodeData.inputs[ix];

                    if (input != null && input.source != 0)
                    {
                        if (idTable.TryGetValue(input.source, out int newID))
                        {
                            input.source = newID;
                        }
                        else
                        {
                            MCNodeInput mcInput = node.GetNodeInputWithIndex(ix);
                            if (mcInput != null)
                            {
                                mcInput.input.source = -1;
                                mcInput.input.subid = -1;
                                input.source = -1;
                                input.subid = -1;
                            }
                        }
                    }
                }
            }

            CreateLineWithCreatedList(createdNodes);

            // 프로세스 정보 업데이트.
            MCWorkspaceManager.Instance.UpdateProcess();
        }

        public void Undo()
        {
            foreach (MCNode node in createdNodes)
            {
                MCWorkspaceManager.Instance.RequestNodeDelete(node);
            }

            // 프로세스 정보 업데이트.
            MCWorkspaceManager.Instance.UpdateProcess();
        }

        private void CreateLineWithCreatedList(List<MCNode> createdList)
        {
            // 라인 생성.
            foreach (var node in createdList)
            {
                if (node.nodeData.nexts != null && node.nodeData.nexts.Length > 0)
                {
                    for (int ix = 0; ix < node.nodeData.nexts.Length; ++ix)
                    {
                        Next next = node.nodeData.nexts[ix];

                        MCNode leftNode = FindNodeWithIDFromList(createdList, node.NodeID);
                        MCNode rightNode = FindNodeWithIDFromList(createdList, next.next);
                        if (leftNode != null && rightNode != null)
                        {
                            if (!leftNode.OwnedProcess.id.Equals(rightNode.OwnedProcess.id))
                            {
                                continue;
                            }

                            MCNodeNext left = leftNode.GetNodeNextWithIndex(ix);
                            MCNodeStart right = rightNode.GetComponentInChildren<MCNodeStart>();

                            if (left != null && right != null && !left.HasLine && !right.HasLine)
                            {
                                MCBezierLine line = Utils.CreateNewLineGO().GetComponent<MCBezierLine>();

                                line.SetLinePoint(left, right);
                                MCWorkspaceManager.Instance.AddLine(line);

                                left.LineSet();
                                right.LineSet();
                            }
                        }
                    }
                }

                // input/output 라인.
                CreateNodeInputLinesFromList(createdList, node.nodeData);
                //CreateNodeInputLines(process, node);
            }
        }

        private MCNode FindNodeWithIDFromList(List<MCNode> createdList, int id)
        {
            foreach (var node in createdList)
            {
                if (node.NodeID.Equals(id))
                {
                    return node;
                }
            }

            return null;
        }

        private void CreateNodeInputLinesFromList(List<MCNode> createdList, Node node)
        {
            MCNode mcNode = FindNodeWithIDFromList(createdList, node.id);
            if (mcNode != null && mcNode.nodeData.inputs != null)
            {
                foreach (PROJECT.Input input in node.inputs)
                {
                    CreateInputLineFromList(createdList, node.id, input);

                    // 작성자: 장세윤.
                    // MCLogNode의 경우 특수한 input이 있어서 예외 처리함
                    // Log Option의 경우 기존의 input과 다름.
                    if (input == null)
                    {
                        continue;
                    }

                    if (input.source != -1)
                    {
                        if (FindNodeWithIDFromList(createdList, input.source) == null)
                        {
                            continue;
                        }

                        CreateNodeInputLinesFromList(createdList, FindNodeWithIDFromList(createdList, input.source).nodeData);
                    }
                }
            }
        }

        private MCNode CreateInputLineFromList(List<MCNode> createdList, int nodeID, PROJECT.Input input)
        {
            if (input == null)
            {
                //Utils.LogRed("input == null");
                return null;
            }

            MCNode leftNode = FindNodeWithIDFromList(createdList, input.source);
            MCNode rightNode = FindNodeWithIDFromList(createdList, nodeID);

            if (leftNode != null && rightNode != null)
            {
                if (!leftNode.OwnedProcess.id.Equals(rightNode.OwnedProcess.id))
                {
                    return null;
                }

                MCNodeOutput left = leftNode.GetNodeOutputWithIndex(input.subid);
                MCNodeInput right = rightNode.GetNodeInputWithIndex(input.id);

                if (left != null && right != null && !right.HasLine)
                {
                    MCBezierLine line = Utils.CreateNewLineGO().GetComponent<MCBezierLine>();

                    // 왼쪽 노드가 GetNode인 경우 라인 색상 설정.
                    if (leftNode is MCGetNode)
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
                    // 그 외의 경우 라인 색상 설정.
                    else
                    {
                        // 변수 타입이 Boolean인 경우 None으로 저장되는 경우가 발생함.
                        // 기존에 None으로 저장되어 있는 프로젝트는 Bool로 설정되도록 변경.
                        // 참고: 새로 저장하는 프로젝트에서는 None으로 저장되는 문제가 발생하지 않음.
                        if (input.type == DataType.NONE)
                        {
                            input.type = DataType.BOOL;
                        }

                        //line.SetLineColor(Utils.GetParameterColor(input.type));
                        line.SetLineColor(Utils.GetParameterColor(right.parameterType));
                    }

                    line.SetLinePoint(left, right);
                    MCWorkspaceManager.Instance.AddLine(line);

                    left.LineSet();
                    right.LineSet();
                }
            }

            return leftNode;
        }
    }
}