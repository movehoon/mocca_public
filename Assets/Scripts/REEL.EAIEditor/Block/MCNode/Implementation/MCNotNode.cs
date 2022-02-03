using UnityEngine;

namespace REEL.D2EEditor
{
	public class MCNotNode : MCNode
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