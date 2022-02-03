#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class StartNodeWindow : MCNodeWindow
    {
#if USINGTMPPRO
        public TMP_Text processNameText;
        public TMP_InputField ownedProcessPriorityInput;
#else
        public Text processNameText;
        public InputField ownedProcessPriorityInput;
#endif

        protected override void OnEnable()
        {
            ownedProcessPriorityInput.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ownedProcessPriorityInput.onValueChanged.RemoveListener(OnValueChanged);
        }

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            processNameText.text = node.OwnedProcess.name;
            ownedProcessPriorityInput.text = node.OwnedProcess.priority.ToString();
        }

        private void OnValueChanged(string input)
        {
            if (refNode != null)
            {
                refNode.OwnedProcess.priority = int.Parse(input);
            }
        }
    }
}