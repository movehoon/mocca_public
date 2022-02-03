using UnityEngine;
using REEL.PROJECT;

using TMPro;

namespace REEL.D2EEditor
{
	public class MCWhileNode : MCNode
	{
        public TMP_InputField input;

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

        public override void SetData(Node node)
        {
            base.SetData(node);

            input.text = node.inputs[0].default_value;
        }
    }
}