using System;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCDeleteVariableCommand : MCCommand
    {
        private string name = string.Empty;
        private PROJECT.DataType type = PROJECT.DataType.NONE;
        private PROJECT.NodeType nodeType = PROJECT.NodeType.START;
        private string value = string.Empty;
        private int variableIndex = -1;

        private List<DeleteNodeInfo> deletedNodeInfos = new List<DeleteNodeInfo>();
        private List<DeleteLineInfo> deletedLinesInfos = new List<DeleteLineInfo>();

        public MCDeleteVariableCommand(LeftMenuVariableItem variable)
        {
            // Undo 명령을 처리할 때 변수 복원을 위해 변수 속성 저장.
            name = variable.VariableName;
            type = variable.dataType;
            nodeType = variable.nodeType;
            value = variable.value;

            variableIndex = MCWorkspaceManager.Instance.GetVariableIndexWithName(name);
            if (variableIndex == -1)
            {
                Utils.LogRed("[MCDeleteVariableCommand] Can't find Variable");
                return;
            }

            List<MCNode> locatedNodes = new List<MCNode>();

            if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            {
                MCTables tables = Utils.GetTables();
                //if (tables == null)
                //{
                //    Utils.LogRed("[MCDeleteVariableCommand] Can't find MCTables");
                //    return;
                //}

                locatedNodes = tables.locatedNodes;
            }
            if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
            {
                string functionName = Utils.CurrentTabName;
                LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithName(functionName);
                locatedNodes = function.LogicNodes;
            }

            // 변수가 삭제되면 같이 삭제해야하는 노드와 라인 검색.
            if (locatedNodes is null || locatedNodes.Count == 0)
            {
                return;
            }

            deletedNodeInfos = new List<DeleteNodeInfo>();
            deletedLinesInfos = new List<DeleteLineInfo>();
            for (int ix = 0; ix < locatedNodes.Count; ++ix)
            {
                MCNode node = locatedNodes[ix];
                if (node is MCGetNode)
                {
                    MCGetNode getNode = node as MCGetNode;
                    getNode.MakeNode();
                    if (getNode.CurrentVariableIndex.Equals(variableIndex))
                    {
                        AddToDeletedNodeInfos(node, ref deletedLinesInfos);
                    }
                }
                else if (node is MCSetNode)
                {
                    MCSetNode setNode = node as MCSetNode;
                    setNode.MakeNode();
                    if (setNode.CurrentVariableIndex.Equals(variableIndex))
                    {
                        AddToDeletedNodeInfos(node, ref deletedLinesInfos);
                    }
                }
            }
        }

        public void Execute()
        {
            MCTables tables = GameObject.FindObjectOfType<MCTables>();
            MCVariableFunctionManager variableFunctionManager = GameObject.FindObjectOfType<MCVariableFunctionManager>();
            if (tables == null || variableFunctionManager == null)
            {
                Utils.LogRed("[MCDeleteVariableCommand] Can't find MCTables of MCVariableFunctionManager");
                return;
            }

            // 함께 제거해야하는 노드와 라인 삭제.
            if (deletedNodeInfos.Count != 0)
            {
                foreach (DeleteNodeInfo nodeInfo in deletedNodeInfos)
                {
                    MCNode node = null;
                    if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
                    {
                        node = MCWorkspaceManager.Instance.FindNodeWithID(nodeInfo.nodeData.id);
                        tables.RemoveNode(node);
                    }
                    
                    if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
                    {
                        string functionName = Utils.CurrentTabName;
                        LeftMenuFunctionItem function 
                            = MCWorkspaceManager.Instance.GetFunctionItemWithName(functionName);
                        node = function.FindNodeWithID(nodeInfo.nodeData.id);
                        function.RemoveLogicNode(node);
                    }

                    if (node != null)
                    {
                        GameObject.Destroy(node.gameObject);
                    }
                }

                if (deletedLinesInfos.Count != 0)
                {
                    foreach (DeleteLineInfo lineInfo in deletedLinesInfos)
                    {
                        MCBezierLine line = tables.FindLineWithID(lineInfo.lineID);
                        tables.RemoveLine(lineInfo.lineID);
                        GameObject.Destroy(line.gameObject);
                    }
                }
            }

            // 변수 제거.
            variableFunctionManager.RemoveVariable(MCWorkspaceManager.Instance.GetVariable(variableIndex));
        }

        public void Undo()
        {
            // 변수 복원.
            MCWorkspaceManager.Instance.AddVariable(name, type, nodeType, value, true);

            GraphPane graphPane = Utils.GetGraphPane();

            if (graphPane == null)
            {
                Utils.LogRed("[MCDeleteVariableCommand] Can't find GraphPane");
                return;
            }

            // 노드 복원.
            MCCommandHelper.RestoreDeletedNodes(deletedNodeInfos);

            // 라인 복원.
            MCCommandHelper.RestoreDeletedLines(deletedLinesInfos);
        }

        private void AddToDeletedNodeInfos(MCNode node, ref List<DeleteLineInfo> deletedLines)
        {
            deletedNodeInfos.Add(new DeleteNodeInfo()
            {
                nodeData = node.nodeData,
                process = node.OwnedProcess
            });

            // 노드에 연결된(삭제가 필요한) 라인이 있는지 확인 후 리스트에 추가.
            MCCommandHelper.CheckIfDeleteLines(node.NodeID, node.OwnedProcess.id, ref deletedLines);
        }
    }
}