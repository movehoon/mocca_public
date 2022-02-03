using TMPro;

using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class MCSayNode : MCNode
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

            //input.text = node.inputs[0].default_value;
            input.text = nodeData.inputs[0].default_value;
            //Debug.Log($"[MCSayNode.SetData] node.inputs[0].default_value: {node.inputs[0].default_value}");
        }
    }
}