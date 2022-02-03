using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCStopNode : MCNode
    {
        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            nodeData.body = null;
            nodeData.inputs = null;
            nodeData.outputs = null;
            nodeData.nexts = null;
        }
    }
}


