using TMPro;
using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCPowerNode : MCNode
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

            input1.text = nodeData.inputs[0].default_value;
            input2.text = nodeData.inputs[1].default_value;
        }
    }
}