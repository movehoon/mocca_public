using UnityEngine;
using System.Collections.Generic;

namespace REEL.D2EEditor
{
    public enum SocketType
    {
        None, Execute, Input, Output
    }

    public class WillBeCreatedNodeInfo
    {
        public int nodeID = 0;
        public PROJECT.Node nodeData;
    }

    public class NodeSocketInfo
    {
        public int nodeID = 0;
        public int socketID = -1;
        public SocketType socketType = SocketType.None;
    }

    public class DeleteNodeInfo
    {
        public PROJECT.Node nodeData;
        public PROJECT.Process process;

        public DeleteNodeInfo()
        {
            nodeData = new PROJECT.Node();
            process = new PROJECT.Process();
        }
    }

    public class DeleteLineInfo
    {
        public int lineID = 0;
        public NodeSocketInfo left;
        public NodeSocketInfo right;
        public Color lineColor;
    }

    public class DeleteCommentInfo
    {
        public string commentName = string.Empty;
        public Vector2 position = Vector2.zero;
        public Vector2 size = Vector2.one;
        public int commentID = 0;
    }

    public class ChangePositionInfo
    {
        public int nodeID = -1;
        public Vector3 position = Vector3.zero;
    }

    public class NodeMovementInfo
    {
        public int nodeID = -1;
        public Vector3 origin;
        public Vector3 destination;

        public override string ToString()
        {
            return $"nodeID: {nodeID} / origin: {origin} / destination: {destination}";
        }
    }

    public static class MCCommandHelper
    {
        // 삭제된 단일 노드 복원하는 함수.
        public static void RestoreDeletedNode(DeleteNodeInfo info, GraphPane graphPane = null, bool isLocalVariable = false)
        {
            if (graphPane == null)
            {
                graphPane = Utils.GetGraphPane();

                if (graphPane == null)
                {
                    Debug.LogWarning("Can't find GraphPane");
                    return;
                }
            }

            // 삭제된 노드 복구.
            MCNode node = graphPane.AddNode(info.nodeData.nodePosition, info.nodeData.type, info.nodeData.id, true, true);
            if (node is MCGetNode)
            {
                MCGetNode getNode = node as MCGetNode;
                getNode.IsLocalVariable = isLocalVariable;
            }
            else if (node is MCSetNode)
            {
                MCSetNode setNode = node as MCSetNode;
                setNode.IsLocalVariable = isLocalVariable;
            }

            node.OwnedProcess.name = info.process.name;
            node.OwnedProcess.id = info.process.id;
            node.OwnedProcess.priority = info.process.priority;
            node.SetData(info.nodeData);
        }

        // deletedNodeInfos 리스트를 기반으로 삭제된 노드 복원하는 함수.
        public static void RestoreDeletedNodes(List<DeleteNodeInfo> deletedNodeInfos, bool isLocalVariable = false)
        {
            GraphPane graphPane = Utils.GetGraphPane();

            // 삭제된 노드 복원.
            foreach (DeleteNodeInfo info in deletedNodeInfos)
            {
                RestoreDeletedNode(info, graphPane, isLocalVariable);
            }
        }

        // deletedLines 리스트를 기반으로 삭제된 라인 복원하는 함수.
        public static void RestoreDeletedLines(List<DeleteLineInfo> deletedLines)
        {
            // 삭제된 라인 복구.
            foreach (DeleteLineInfo info in deletedLines)
            {
                MCBezierLine line = Utils.CreateNewLineGO().GetComponent<MCBezierLine>();
                info.lineID = line.LineID;
                line.SetLineColor(info.lineColor);

                MCNode leftNode = MCWorkspaceManager.Instance.FindNodeWithID(info.left.nodeID);
                MCNode rightNode = MCWorkspaceManager.Instance.FindNodeWithID(info.right.nodeID);

                if (leftNode != null && rightNode != null)
                {
                    MCNodeSocket leftSocket = null;
                    if (info.left.socketType == SocketType.Execute)
                    {
                        leftSocket = leftNode.GetNodeNextWithIndex(info.left.socketID);
                    }
                    else if (info.left.socketType == SocketType.Output)
                    {
                        leftSocket = leftNode.GetNodeOutputWithIndex(info.left.socketID);
                    }

                    MCNodeSocket rightSocket = null;
                    if (info.right.socketType == SocketType.Execute)
                    {
                        rightSocket = rightNode.GetComponentInChildren<MCNodeStart>();
                        //Utils.LogGreen($"rightSocket:{rightSocket == null}");
                    }
                    else if (info.right.socketType == SocketType.Input)
                    {
                        rightSocket = rightNode.GetNodeInputWithIndex(info.right.socketID);
                        //Utils.LogGreen($"rightSocket:{rightSocket == null} / info.right.socketID:{info.right.socketID}");
                    }

                    if (leftSocket != null && rightSocket != null)
                    {
                        line.SetLinePoint(leftSocket, rightSocket);
                        leftSocket.LineSet();
                        rightSocket.LineSet();

                        MCWorkspaceManager.Instance.AddLine(line);
                    }

                    else
                    {
                        //Utils.LogRed("leftSocket or rightSocket is null");

                        //if (leftSocket == null)
                        //{
                        //    Utils.LogRed("leftSocket is null");
                        //}

                        //else if (rightSocket == null)
                        //{
                        //    Utils.LogRed("rightSocket is null");
                        //}
                    }
                }
                else
                {
                    //Utils.LogRed("leftNode or rightNode is null");

                    //if (leftNode == null)
                    //{
                    //    Utils.LogRed("leftNode is null");
                    //}
                    //else if (rightNode == null)
                    //{
                    //    Utils.LogRed("rightNode is null");
                    //}

                    return;
                }
            }
        }

        // 노드에 연결된 라인이(삭제할 라인 후보) 있는지 확인해 deletedLines 리스트에 추가해주는 함수.
        public static void CheckIfDeleteLines(int nodeID, int processID, ref List<DeleteLineInfo> deletedLines)
        {
            MCTables tables = GameObject.FindObjectOfType<MCTables>();
            if (tables == null)
            {
                Utils.LogRed("Can't find MCTables");
                return;
            }

            if (tables.locatedLines.Count == 0)
            {
                return;
            }

            foreach (MCBezierLine line in tables.locatedLines)
            {
                if (line.left.Node.OwnedProcess.id.Equals(processID) && line.left.Node.NodeID.Equals(nodeID) ||
                    line.right.Node.OwnedProcess.id.Equals(processID) && line.right.Node.NodeID.Equals(nodeID))
                {
                    //deletedLines.Add(line);
                    DeleteLineInfo info = new DeleteLineInfo();
                    info.lineColor = line.color;
                    info.lineID = line.LineID;

                    info.left = new NodeSocketInfo();
                    info.left.nodeID = line.left.Node.NodeID;
                    info.left.socketType = GetSocketType(line.left);
                    if (info.left.socketType == SocketType.Execute)
                    {
                        info.left.socketID = line.left.Node.GetNodeNextIndexWithSocket(line.left);
                    }
                    else if (info.left.socketType == SocketType.Output)
                    {
                        info.left.socketID = line.left.Node.GetNodeOutputIndexWithSocket(line.left);
                    }

                    info.right = new NodeSocketInfo();
                    info.right.nodeID = line.right.Node.NodeID;
                    info.right.socketType = GetSocketType(line.right);
                    if (info.right.socketType == SocketType.Execute)
                    {
                        info.right.socketID = 0;
                    }
                    else if (info.right.socketType == SocketType.Input)
                    {
                        info.right.socketID = line.right.Node.GetNodeInputIndexWithSocket(line.right);
                    }

                    // 기존에 추가되지 않은 라인(선)만 삭제 후보 리스트에 추가.
                    bool shouldAdd = true;
                    foreach (DeleteLineInfo lineInfo in deletedLines)
                    {
                        if (lineInfo.lineID.Equals(info.lineID))
                        {
                            shouldAdd = false;
                            break;
                        }
                    }

                    if (shouldAdd == true)
                    {
                        deletedLines.Add(info);
                    }
                }
            }
        }

        // 라인이 연결된 소켓 타입을 구분해주는 함수.
        public static SocketType GetSocketType(MCNodeSocket socket)
        {
            if (socket is MCNodeNext || socket is MCNodeStart)
            {
                return SocketType.Execute;
            }
            else if (socket is MCNodeInput || socket is MCNodeExpressionInput)
            {
                return SocketType.Input;
            }
            else if (socket is MCNodeOutput)
            {
                return SocketType.Output;
            }

            return SocketType.None;
        }

        // 노드 이동 명령 임시 저장용 변수.
        public static MCMultiNodeMoveCommand nodeMoveCommand = null;

        // 노드 이동처리해주는 함수.
        public static void MoveNodeTo(int nodeID, Vector3 position)
        {
            MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(nodeID);
            if (node != null)
            {
                node.RefRect.localPosition = position;
                node.GetComponent<MCNodeDrag>().ExecuteOnChanged();
            }
        }
    }
}