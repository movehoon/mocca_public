using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCAgeGenderNode : MCNode
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
    }
}
