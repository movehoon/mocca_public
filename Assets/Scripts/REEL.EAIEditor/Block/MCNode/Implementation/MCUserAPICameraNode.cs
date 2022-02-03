#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using REEL.PROJECT;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class MCUserAPICameraNode : MCNode
	{
#if USINGTMPPRO
        public TMP_InputField apiInput;
#else
        public InputField apiInput;
#endif

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

            apiInput.text = node.inputs[0].default_value;
        }
    }
}