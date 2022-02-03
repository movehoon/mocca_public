using System;
using UnityEngine;

namespace REEL.D2EEditor
{
    [Serializable]
    public class LineExecutePoint
    {
        public int blockID = 0;
        public int executePointID = 0;

        public LineExecutePoint(int blockID)
        {
            this.blockID = blockID;
        }

        public LineExecutePoint(int blockID, int executePointID)
        {
            this.blockID = blockID;
            this.executePointID = executePointID;
        }
    }

    [Serializable]
    public class LineBlock
    {
        public LineExecutePoint left;
        public LineExecutePoint right;

        public LineBlock(int leftBlockID, int rightBlockID)
        {
            left = new LineExecutePoint(leftBlockID);
            right = new LineExecutePoint(rightBlockID);
        }

        public LineBlock(int leftBlockID, int leftExecutePointID, int rightBlockID)
        {
            left = new LineExecutePoint(leftBlockID, leftExecutePointID);
            right = new LineExecutePoint(rightBlockID);
        }
    }

    [Serializable]
    public class LineBlockArray
    {
        [SerializeField] private LineBlock[] lineData;

        public int Length
        {
            get { return lineData == null ? -1 : lineData.Length; }
        }

        public LineBlock this[int index]
        {
            get { return lineData[index]; }
            set { lineData[index] = value; }
        }

        public void Add(LineBlock block)
        {
            if (lineData == null)
            {
                lineData = new LineBlock[1];
                lineData[0] = block;
                return;
            }

            LineBlock[] tempArray = new LineBlock[lineData.Length];
            for (int ix = 0; ix < tempArray.Length; ++ix)
            {
                tempArray[ix] = lineData[ix];
            }

            lineData = new LineBlock[lineData.Length + 1];
            for (int ix = 0; ix < tempArray.Length; ++ix)
            {
                lineData[ix] = tempArray[ix];
            }

            lineData[tempArray.Length] = block;
        }
    }
}