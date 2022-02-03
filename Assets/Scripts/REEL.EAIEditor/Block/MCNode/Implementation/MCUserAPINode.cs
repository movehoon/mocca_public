using TMPro;
using System.Collections;
using System.Collections.Generic;
using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCUserAPINode : MCNode
    {
        public TMP_InputField apiInput;

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