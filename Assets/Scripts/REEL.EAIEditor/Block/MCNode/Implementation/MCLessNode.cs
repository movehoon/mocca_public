#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class MCLessNode : MCNode
	{
#if USINGTMPPRO
        public TMP_InputField input1;
        public TMP_InputField input2;
#else
        public InputField input1;
        public InputField input2;
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

            input1.text = nodeData.inputs[0].default_value;
            input2.text = nodeData.inputs[1].default_value;
        }
    }
}