using TMPro;
using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCFloorNode : MCNode
    {
        public TMP_InputField input1;

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
        }
    }
}