using UnityEngine;
using System.Collections.Generic;

namespace REEL.D2EEditor
{
    public class MCDeleteMultipleNodeCommand : MCCommand
    {
        private List<DeleteNodeInfo> deleteNodeInfos = new List<DeleteNodeInfo>();
        private List<DeleteLineInfo> deletedLines = new List<DeleteLineInfo>();

        public MCDeleteMultipleNodeCommand(List<MCNode> selected)
        {
            foreach (MCNode node in selected)
            {
                if (node.DontDestroy == true)
                {
                    continue;
                }

                node.MakeNode();

                deleteNodeInfos.Add(new DeleteNodeInfo()
                {
                    nodeData = node.nodeData,
                    process = node.OwnedProcess
                });
            }
        }

        public void Execute()
        {
            MCTables tables = GameObject.FindObjectOfType<MCTables>();

            if (tables == null)
            {
                Debug.LogWarning("Can't find MCTables");
                return;
            }

            // 노드 삭제.
            deletedLines = new List<DeleteLineInfo>();
            foreach (DeleteNodeInfo info in deleteNodeInfos)
            {
                // 노드에 연결된(삭제가 필요한) 라인이 있는지 확인 후 리스트에 추가.
                MCCommandHelper.CheckIfDeleteLines(info.nodeData.id, info.process.id, ref deletedLines);

                // 노드 삭제.
                MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(info.nodeData.id);
                if (node.DontDestroy == true)
                {
                    continue;
                }

                tables.RemoveNode(node);
                GameObject.Destroy(node.gameObject);
            }

            // 노드에 연결된 라인 삭제.
            foreach (DeleteLineInfo info in deletedLines)
            {
                MCBezierLine line = tables.FindLineWithID(info.lineID);
                tables.RemoveLine(info.lineID);
                GameObject.Destroy(line.gameObject);
            }

            // 프로세스 정보 업데이트.
            MCWorkspaceManager.Instance.UpdateProcess();
        }

        public void Undo()
        {
            // 삭제된 노드 복원.
            MCCommandHelper.RestoreDeletedNodes(deleteNodeInfos);

            // 삭제된 라인 복원.
            MCCommandHelper.RestoreDeletedLines(deletedLines);

            // 프로세스 정보 업데이트.
            MCWorkspaceManager.Instance.UpdateProcess();
        }
    }
}