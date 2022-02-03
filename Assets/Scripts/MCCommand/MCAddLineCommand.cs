using UnityEngine;

namespace REEL.D2EEditor
{
    public enum LineType
    {
        Execute, IO
    }

    public class SocketInfo
    {
        public int processID;
        public int nodeID;
        public int socketID;

        public SocketInfo()
        {
        }

        public SocketInfo(int processID, int nodeID, int socketID)
        {
            this.processID = processID;
            this.nodeID = nodeID;
            this.socketID = socketID;
        }

        public override string ToString()
        {
            return $"processID: {processID}/nodeID: {nodeID}/socketID: {socketID}";
        }
    }

    public class MCAddLineCommand : MCCommand
    {
        private MCBezierLine line;
        private int lineID = 0;
        private LineType lineType = LineType.Execute;

        private MCNodeSocket left;
        private MCNodeSocket right;

        // Test.
        private SocketInfo leftSocketInfo;
        private SocketInfo rightSocketInfo;

        //public MCAddLineCommand(MCBezierLine line, int lineID, MCNodeSocket left, MCNodeSocket right, LineType lineType = LineType.Execute)
        public MCAddLineCommand(MCBezierLine line, int lineID, MCNodeSocket left, MCNodeSocket right)
        {
            this.line = line;
            this.lineID = lineID;
            this.left = left;
            this.right = right;

            leftSocketInfo = new SocketInfo()
            {
                processID = left.Node.OwnedProcess.id,
                nodeID = left.Node.NodeID,
                socketID = left.Node.GetNodeOutputIndexWithSocket(left)
            };

            rightSocketInfo = new SocketInfo()
            {
                processID = right.Node.OwnedProcess.id,
                nodeID = right.Node.NodeID,
                socketID = right.Node.GetNodeInputIndexWithSocket(right)
            };
        }

        public void Execute()
        {
            if (left == null)
            {
                MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(
                    leftSocketInfo.nodeID, leftSocketInfo.processID);

                if (node == null)
                {
                    return;
                }

                left = node.GetNodeOutputWithIndex(leftSocketInfo.socketID);
                if (left == null)
                {
                    return;
                }
            }

            if (right == null)
            {
                MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(
                    rightSocketInfo.nodeID, rightSocketInfo.processID);

                if (node == null)
                {
                    return;
                }

                right = node.GetNodeOutputWithIndex(rightSocketInfo.socketID);
                if (right == null)
                {
                    return;
                }
            }

            if (line == null)
            {
                line = Utils.CreateNewLineGO().GetComponent<MCBezierLine>();
                lineID = line.LineID;

                //if (left is MCNodeInputOutputBase)
                //{
                //    MCNodeInputOutputBase io = left as MCNodeInputOutputBase;
                //    line.SetLineColor(Utils.GetParameterColor(io.parameterType));
                //}
                if (right is MCNodeInputOutputBase)
                {
                    MCNodeInputOutputBase io = right as MCNodeInputOutputBase;
                    line.SetLineColor(Utils.GetParameterColor(io.parameterType));
                }
            }

            line.SetLinePoint(left, right);
            if (right != null && right is MCNodeInputOutputBase)
            {
                var io = right as MCNodeInputOutputBase;
                line.SetLineColor(Utils.GetParameterColor(io.parameterType));
            }
            left.LineSet();
            right.LineSet();

            MCWorkspaceManager.Instance.AddLine(line);
        }

        public void Undo()
        {
            bool success = MCWorkspaceManager.Instance.RequestLineDelete(lineID);
            if (success == true)
            {
                line = null;
            }
        }
    }
}