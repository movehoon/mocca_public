using TMPro;
using UnityEngine;
using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCChoiceNode : MCNode
    {
        [SerializeField] private TMP_InputField input;
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

            input.text = nodeData.inputs[1].default_value;
        }
    }
}
