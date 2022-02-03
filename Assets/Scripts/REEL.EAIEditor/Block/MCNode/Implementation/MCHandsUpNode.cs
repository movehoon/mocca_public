using REEL.PROJECT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCHandsUpNode : MCNode
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
            nodeData.outputs = new Output[outputs.Length];
            for (int ix = 0; ix < outputs.Length; ++ix)
            {
                nodeData.outputs[ix] = new Output();
                nodeData.outputs[ix].id = outputs[ix].output.id;
                nodeData.outputs[ix].type = outputs[ix].output.type;
                nodeData.outputs[ix].value = outputs[ix].output.value;
            }

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