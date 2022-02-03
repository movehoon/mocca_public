using REEL.PROJECT;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCStartNode : MCNode
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
    }
}