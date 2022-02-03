using REEL.PROJECT;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    public class MCAddNodeCommand : MCCommand
    {
        private Vector3 itemPosition;
        private NodeType nodeType;
        private int nodeID = 0;
        private bool isLoaded = false;
        private bool isLocalPos = false;
        private bool isDuplicate = false;

        // Only GET/SET 노드용도.
        private string variableName = "";

        // 현재는 GET/SET 노드에서 사용하는데, 노드 생성 후 추가로 설정이 필요하면 delegate를 넘겨 처리할 수 있음.
        private Action<MCNode> functionToCall = null;

        public MCAddNodeCommand(
            Vector3 itemPosition, 
            NodeType nodeType, 
            int nodeID = 0, 
            bool isLoaded = false, 
            bool isLocalPos = false, 
            bool isDuplicate = false,
            string variableName = "",
            Action<MCNode> functionToCall = null)
        {
            this.itemPosition = itemPosition;
            this.nodeType = nodeType;
            this.nodeID = nodeID;
            this.isLoaded = isLoaded;
            this.isLocalPos = isLocalPos;
            this.isDuplicate = isDuplicate;
            this.variableName = variableName;
            this.functionToCall = functionToCall;
        }

        public void Execute()
        {
            GraphPane graphPane = Utils.GetGraphPane();

            if (nodeType == NodeType.START)
            {
                AddEntryNode();
            }

            else
            {
                AddNormalNode(graphPane);
            }

            //graphPane.AddNode(itemPosition, nodeType, nodeID, isLoaded, isLocalPos, isDuplicate);
            //MCWorkspaceManager.Instance.AddNode(node, nodeID);
        }

        private void AddNormalNode(GraphPane graphPane)
        {
            MCNode node = graphPane.AddNode(itemPosition, nodeType, nodeID, isLoaded, isLocalPos, isDuplicate);

            // GET/SET 노드인 경우, 노드 생성 후 변수 속성 관련 추가 설징이 필요해 처리함.
            if (Utils.IsNullOrEmptyOrWhiteSpace(variableName) == false)
            {
                if (node.nodeData.body == null)
                {
                    node.nodeData.body = new Body();
                }

                node.nodeData.body.name = variableName;
            }

            functionToCall?.Invoke(node);
        }

        private void AddEntryNode()
        {
            //Debug.Log($"[MCAddNodeCommand] nodeID: {nodeID}");
            MCWorkspaceManager.Instance.AddEntryNode(itemPosition, nodeID);
        }

        public void Undo()
        {
            MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(nodeID);
            MCWorkspaceManager.Instance.RequestNodeDelete(node);

            // 프로세스 정보 업데이트.
            MCWorkspaceManager.Instance.UpdateProcess();
        }
    }
}