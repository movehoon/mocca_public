using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCDeleteLineCommand : MCCommand
    {
        private int lineID;
        private MCNodeSocket left;
        private MCNodeSocket right;
        private Color lineColor;

        public MCDeleteLineCommand(int lineID, MCNodeSocket left, MCNodeSocket right, Color lineColor)
        {
            this.lineID = lineID;
            this.left = left;
            this.right = right;
            this.lineColor = lineColor;
        }

        public void Execute()
        {   
            MCWorkspaceManager.Instance.RequestLineDelete(lineID);
        }

        public void Undo()
        {
            MCBezierLine line = Utils.CreateNewLineGO().GetComponent<MCBezierLine>();

            lineID = line.LineID;
            line.SetLineColor(lineColor);
            line.SetLinePoint(left, right);
            left.LineSet();
            right.LineSet();

            MCWorkspaceManager.Instance.AddLine(line);
        }
    }
}
