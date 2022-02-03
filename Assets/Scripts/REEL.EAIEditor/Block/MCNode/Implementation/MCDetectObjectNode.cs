using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCDetectObjectNode : MCNode
    {
        public TMPro.TMP_InputField inputField;

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

        public override void SetData(Node node)
        {
            base.SetData(node);

            inputField.text = node.inputs[0].default_value;
        }
    }
}