using TMPro;
using UnityEngine;
using UnityEngine.UI;
using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCLoopNode : MCNode
    {
        public TMP_InputField initialInput;
        public TMP_InputField conditionInput;
        public TMP_InputField incrementInput;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            nodeData.body = null;
            nodeData.outputs = null;

            // 블록 처음 생성할 때.
            if (nodeData.inputs == null || nodeData.inputs.Length == 0)
            {
                inputs[0].input.default_value = "0";
                initialInput.text = "0";
                
                inputs[2].input.default_value = "1";
                incrementInput.text = "1";
            }

            if (nodeData.inputs.Length > 1)
            {
                if (Utils.IsNullOrEmptyOrWhiteSpace(nodeData.inputs[0].default_value) == true)
                {
                    initialInput.text = "0";
                }

                if (Utils.IsNullOrEmptyOrWhiteSpace(nodeData.inputs[2].default_value) == true)
                {
                    incrementInput.text = "1";
                }
            }
            else if (nodeData.inputs.Length == 1)
            {
                conditionInput.text = nodeData.inputs[0].default_value;
            }
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            //countInput.text = nodeData.inputs[0].default_value;
            if (node.inputs.Length == 1)
            {
                initialInput.text = "0";
                conditionInput.text = node.inputs[0].default_value;
                incrementInput.text = "1";
            }
            else
            {
                initialInput.text = node.inputs[0].default_value;
                conditionInput.text = node.inputs[1].default_value;
                incrementInput.text = node.inputs[2].default_value;
            }
        }
    }
}