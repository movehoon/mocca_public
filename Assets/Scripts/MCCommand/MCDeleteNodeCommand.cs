using REEL.PROJECT;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCDeleteNodeCommand : MCCommand
    {
        private DeleteNodeInfo deleteNodeInfo;
        private List<DeleteLineInfo> deletedLines = new List<DeleteLineInfo>();

        public MCDeleteNodeCommand(MCNode node)
        {
            node.MakeNode();

            deleteNodeInfo = new DeleteNodeInfo();
            deleteNodeInfo.nodeData = node.nodeData;
            deleteNodeInfo.process = node.OwnedProcess;
        }

        public void Execute()
        {
            MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(deleteNodeInfo.nodeData.id);
            if (node.DontDestroy == true)
            {   
                return;
            }

            // 노드에 연결된(삭제가 필요한) 라인이 있는지 확인 후 리스트에 추가.
            deletedLines = new List<DeleteLineInfo>();
            MCCommandHelper.CheckIfDeleteLines(node.NodeID, node.OwnedProcess.id, ref deletedLines);

            MCTables tables = Utils.GetTables();
            if (tables == null)
            {
                Utils.LogRed("Can't fine MCTables");
                return;
            }

            // 노드 삭제.
            tables.RemoveNode(node);
            GameObject.Destroy(node.gameObject);

            if (deleteNodeInfo.nodeData.type == NodeType.START)
            {
                // 프로세스 정보 업데이트 
                // (Start 노드 삭제하면 Process가 줄어들기 때문에 업데이트 필요함)
                MCWorkspaceManager.Instance.UpdateProcess();
            }

            if (deletedLines.Count == 0)
            {
                return;
            }

            // 노드에 연결된 라인 삭제.
            foreach (DeleteLineInfo info in deletedLines)
            {
                MCBezierLine line = tables.FindLineWithID(info.lineID);

                // 좌우 소켓에 선 삭제 알림.
                line.left.RemoveLine(line.LineID);
                line.right.RemoveLine(line.LineID);

                tables.RemoveLine(info.lineID);
                GameObject.Destroy(line.gameObject);
            }
        }

        public void Undo()
        {
            //GraphPane graphPane = MCEditorManager.Instance
            //    .GetPane(MCEditorManager.PaneType.Graph_Pane)
            //    .GetComponent<GraphPane>();
            GraphPane graphPane = Utils.GetGraphPane();

            if (graphPane == null)
            {
                Debug.LogWarning("Can't find GraphPane");
                return;
            }

            // 삭제된 노드 복구.
            MCNode node = graphPane.AddNode(deleteNodeInfo.nodeData.nodePosition, deleteNodeInfo.nodeData.type, deleteNodeInfo.nodeData.id, true, true);
            node.OwnedProcess.name = deleteNodeInfo.process.name;
            node.OwnedProcess.id = deleteNodeInfo.process.id;
            node.OwnedProcess.priority = deleteNodeInfo.process.priority;
            node.SetData(deleteNodeInfo.nodeData);

            // 삭제된 라인 복구.
            MCCommandHelper.RestoreDeletedLines(deletedLines);

            if (deleteNodeInfo.nodeData.type == NodeType.START)
            {
                // 프로세스 정보 업데이트.
                MCWorkspaceManager.Instance.UpdateProcess();
            }
        }
    }
}