using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCChangeNodePositionsCommand : MCCommand
    {
        private List<ChangePositionInfo> originPositionInfos = new List<ChangePositionInfo>();
        private ChangePositionInfo[] changePositionInfos;

        public MCChangeNodePositionsCommand(List<ChangePositionInfo> changePositionInfos)
        {
            this.changePositionInfos = new ChangePositionInfo[changePositionInfos.Count];
            changePositionInfos.CopyTo(this.changePositionInfos);

            foreach (ChangePositionInfo info in changePositionInfos)
            {
                MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(info.nodeID);
                originPositionInfos.Add(new ChangePositionInfo()
                {
                    nodeID = info.nodeID,
                    //position = node.transform.localPosition
                    position = node.transform.position
                });
            }
        }

        public void Execute()
        {
            foreach (ChangePositionInfo info in changePositionInfos)
            {
                MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(info.nodeID);
                //node.transform.localPosition = info.position;
                node.transform.position = info.position;
            }

            MCWorkspaceManager.Instance.UpdateAllLineUpdate();
        }

        public void Undo()
        {
            foreach (ChangePositionInfo info in originPositionInfos)
            {
                MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(info.nodeID);
                //node.transform.localPosition = info.position;
                node.transform.position = info.position;
            }

            MCWorkspaceManager.Instance.UpdateAllLineUpdate();
        }
    }
}