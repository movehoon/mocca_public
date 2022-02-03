using TMPro;
using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class MCRandomNode : MCNode
	{
        public TMP_InputField input1;
        public TMP_InputField input2;

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

            if (nodeData.inputs.Length > 1)
            {
                input1.text = nodeData.inputs[0].default_value;
                input2.text = nodeData.inputs[1].default_value;
            }
            else if (nodeData.inputs.Length == 1)
            {
                input1.text = "1";
                //input2.text = nodeData.inputs[0].default_value;
                input2.text = node.inputs[0].default_value;
            }
        }
    }
}