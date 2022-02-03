#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using REEL.PROJECT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCResumeNode : MCNode
    {
#if USINGTMPPRO
        public TMP_Dropdown processDropdown;
#else
        public Dropdown processDropdown;
#endif

        public string GetCurrentProcessName { get { return processDropdown.options[processDropdown.value].text; } }

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

        protected override void UpdateNodeData()
        {
            base.UpdateNodeData();

            int processIndex = processDropdown.value;
            var process = MCWorkspaceManager.Instance.Processes[processIndex];
            nodeData.inputs[0].default_value = process.id.ToString();
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            processDropdown.value = Constants.GetProcessIndexWithProcessID(node.inputs[0].default_value);
        }
    }
}

