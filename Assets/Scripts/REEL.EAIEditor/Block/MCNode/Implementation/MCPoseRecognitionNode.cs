using UnityEngine;
using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCPoseRecognitionNode : MCNode
    {
        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            nodeData.body = null;
            nodeData.outputs = null;
        }

        protected override void UpdateNodeData()
        {
            Output output = new Output();
            output.id = 0;
            output.type = DataType.STRING;
            output.value = "_RECOGNIZED_POSE_";
            nodeData.outputs = new Output[] { output };

            nodeData.nexts = new Next[nexts.Length];
            for (int ix = 0; ix < nexts.Length; ++ix)
            {
                nodeData.nexts[ix] = new Next();
                nodeData.nexts[ix].value = nexts[ix].next.value;
                nodeData.nexts[ix].next = nexts[ix].next.next;
            }

            // BreakPoint 옵션 설정.
            // 0:설정 안됨, 1:설정됨.
            nodeData.hasBreakPoint = hasBreakPoint;
        }
    }
}