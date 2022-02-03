using UnityEngine;
using UnityEngine.UI;
using REEL.PROJECT;
using TMPro;

namespace REEL.D2EEditor
{
    public class MCFacialNode : MCNode
    {
        public TMP_Dropdown facialDropdown;

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
            facialDropdown.ClearOptions();
            facialDropdown.options = Constants.GetFacialTMPOptionData;
        }

        protected override void UpdateNodeData()
        {
            nodeData.inputs = new PROJECT.Input[inputs.Length];
            for (int ix = 0; ix < inputs.Length; ++ix)
            {
                nodeData.inputs[ix] = new PROJECT.Input();
                nodeData.inputs[ix].id = inputs[ix].input.id;
                nodeData.inputs[ix].type = DataType.STRING;
                nodeData.inputs[ix].source = inputs[ix].input.source;
                nodeData.inputs[ix].subid = inputs[ix].input.subid;

                if (inputs[ix].input.default_value == string.Empty)
                {
                    nodeData.inputs[ix].default_value = Constants.facialListData[0].nameEnglish;
                }
                else
                {
                    nodeData.inputs[ix].default_value = inputs[ix].input.default_value;
                }

                if (inputs[ix].HasLine)
                {
                    nodeData.inputs[ix].default_value = string.Empty;
                }
            }

            nodeData.nexts = new Next[nexts.Length];
            for (int ix = 0; ix < nexts.Length; ++ix)
            {
                nodeData.nexts[ix] = nexts[ix].next;
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

            facialDropdown.value = Constants.GetFacialIndexFromEnglish(node.inputs[0].default_value);
        }
    }
}