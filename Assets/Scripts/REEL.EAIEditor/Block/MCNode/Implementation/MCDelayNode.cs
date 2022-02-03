#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using System.Collections;
using System.Collections.Generic;
using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCDelayNode : MCNode
    {
#if USINGTMPPRO
        public TMP_InputField input;
#else
        public InputField input;
#endif

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            nodeData.body = null;
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            input.text = nodeData.inputs[0].default_value;
        }
    }
}