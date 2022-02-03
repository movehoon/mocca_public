using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCExpressStopNode : MCNode
    {
        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            nodeData.inputs = null;
            nodeData.body = null;
            nodeData.outputs = null;
        }

        protected override void UpdateNodeData()
        {
            nodeData.nexts = new Next[nexts.Length];
            for (int ix = 0; ix < nexts.Length; ++ix)
            {
                nodeData.nexts[ix] = nexts[ix].next;
            }   

            // Test.
            // BreakPoint 옵션 설정.
            // 0:설정 안됨, 1:설정됨.
            nodeData.hasBreakPoint = hasBreakPoint;
        }
    }
}
