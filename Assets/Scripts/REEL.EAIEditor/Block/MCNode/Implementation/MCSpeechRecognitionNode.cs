using REEL.PROJECT;
using UnityEngine;

namespace REEL.D2EEditor
{
	public class MCSpeechRecognitionNode : MCNode
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