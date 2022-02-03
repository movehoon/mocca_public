#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class MCRemoveNode : MCNode
	{
#if USINGTMPPRO
        public TMP_InputField indexInput;
#else
        public InputField indexInput;
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

            indexInput.text = nodeData.inputs[1].default_value;
        }
    }
}