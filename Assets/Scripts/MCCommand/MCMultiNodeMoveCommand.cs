using System;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCMultiNodeMoveCommand : MCCommand
    {
        private List<NodeMovementInfo> infos;

        public MCMultiNodeMoveCommand(List<MCNodeDrag> nodeDrags)
        {
            infos = new List<NodeMovementInfo>();
            foreach (MCNodeDrag drag in nodeDrags)
            {
                NodeMovementInfo info = new NodeMovementInfo();
                MCNode node = drag.GetComponent<MCNode>();
                info.nodeID = node.NodeID;
                info.origin = node.RefRect.localPosition;

                infos.Add(info);
            }
        }

        public void SetDestinations(List<MCNodeDrag> nodeDrags)
        {
            if (infos != null)
            {
                foreach (var info in infos)
                {
                    foreach (var drag in nodeDrags)
                    {
                        MCNode node = drag.GetComponent<MCNode>();
                        if (info.nodeID.Equals(node.NodeID) == true)
                        {
                            info.destination = node.RefRect.localPosition;

                            continue;
                        }
                    }
                }
            }
        }

        public void Execute()
        {
            if (infos != null && infos.Count > 0)
            {
                foreach (var info in infos)
                {
                    MCCommandHelper.MoveNodeTo(info.nodeID, info.destination);
                }
            }
        }

        public void Undo()
        {
            if (infos != null && infos.Count > 0)
            {
                foreach (var info in infos)
                {
                    MCCommandHelper.MoveNodeTo(info.nodeID, info.origin);
                }
            }
        }
    }
}