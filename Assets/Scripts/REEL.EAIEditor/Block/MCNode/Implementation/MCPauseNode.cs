using TMPro;

using REEL.PROJECT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCPauseNode : MCNode
    {
        public TMP_Dropdown processDropdown;

        public string GetCurrentProcessName { get { return processDropdown.options[processDropdown.value].text; } }

        private bool shouldDelayUpdate = false;
        private int targetValue = -1;

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

        
        private void LateUpdate()
        {
            if (shouldDelayUpdate == true)
            {
                if (targetValue != -1)
                {
                    processDropdown.value = targetValue;
                    targetValue = -1;
                }

                shouldDelayUpdate = false;
            }
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

            shouldDelayUpdate = true;
            targetValue = Constants.GetProcessIndexWithProcessID(node.inputs[0].default_value);
            //processDropdown.value = Constants.GetProcessIndexWithProcessID(node.inputs[0].default_value);
        }
    }
}