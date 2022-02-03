using TMPro;

using UnityEngine;
using UnityEngine.UI;
using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCMobilityNode : MCNode
    {
        public TMP_Dropdown mobilityDropdown;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            nodeData.body = null;
            nodeData.outputs = null;

            LocalizationManager.OnLanguageChanged += OnLanguageChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            mobilityDropdown.ClearOptions();
            mobilityDropdown.options = Constants.GetMobilityTMPOptionData;
        }

        protected override void UpdateNodeData()
        {
            nodeData.inputs = new PROJECT.Input[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                nodeData.inputs[i] = new PROJECT.Input();
                nodeData.inputs[i].id = inputs[i].input.id;
                nodeData.inputs[i].type = DataType.STRING;
                nodeData.inputs[i].source = inputs[i].input.source;
                nodeData.inputs[i].subid = inputs[i].input.subid;

                if (inputs[i].input.default_value == string.Empty)
                {
                    nodeData.inputs[i].default_value = Constants.mobilityList[0];
                }   
                else
                {
                    nodeData.inputs[i].default_value = inputs[i].input.default_value;
                }

                if (inputs[i].HasLine)
                {
                    nodeData.inputs[i].default_value = string.Empty;
                }
            }

            nodeData.nexts = new Next[nexts.Length];
            for (int i = 0; i < nexts.Length; i++)
            {
                nodeData.nexts[i] = nexts[i].next;
            }

            // Test.
            // BreakPoint 옵션 설정.
            // 0:설정 안됨, 1:설정됨.
            nodeData.hasBreakPoint = hasBreakPoint;
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            if (node.inputs == null || node.inputs.Length == 0)
            {
                return;
            }
            
            mobilityDropdown.value = Constants.GetMobilityIndexFromEnglish(node.inputs[0].default_value);
        }
    }
}