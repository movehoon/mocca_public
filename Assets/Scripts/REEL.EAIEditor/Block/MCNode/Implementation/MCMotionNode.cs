using TMPro;

using UnityEngine;
using UnityEngine.UI;
using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCMotionNode : MCNode
    {
        public TMP_Dropdown motionDropdown;

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
            motionDropdown.ClearOptions();
            motionDropdown.options = Constants.GetMotionTMPOptionData;
        }

        protected override void UpdateNodeData()
        {
            nodeData.inputs = new PROJECT.Input[inputs.Length];
            for (int ix = 0; ix < inputs.Length; ix++)
            {
                nodeData.inputs[ix] = new PROJECT.Input();
                nodeData.inputs[ix].id = inputs[ix].input.id;
                nodeData.inputs[ix].type = DataType.STRING;
                nodeData.inputs[ix].source = inputs[ix].input.source;
                nodeData.inputs[ix].subid = inputs[ix].input.subid;

                if (inputs[ix].input.default_value == string.Empty)
                {
                    nodeData.inputs[ix].default_value = Constants.motionListData[0].nameEnglish;
                    //Utils.LogRed($"1: {nodeData.inputs[ix].default_value}");
                }

                else
                {
                    nodeData.inputs[ix].default_value = inputs[ix].input.default_value;
                    //Utils.LogRed($"2: {nodeData.inputs[ix].default_value}");
                }

                if (inputs[ix].HasLine)
                {
                    nodeData.inputs[ix].default_value = string.Empty;
                    //Utils.LogRed($"3: {nodeData.inputs[ix].default_value}");
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

            motionDropdown.value = Constants.GetMotionIndexFromEnglish(node.inputs[0].default_value);
        }
    }
}