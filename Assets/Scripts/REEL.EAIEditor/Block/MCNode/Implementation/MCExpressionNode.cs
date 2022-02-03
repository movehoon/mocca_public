using REEL.PROJECT;
using TMPro;

namespace REEL.D2EEditor
{
    public class MCExpressionNode : MCNode
	{
        public TMP_Dropdown expressionDropdown;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            MCVariableFunctionManager manager = FindObjectOfType<MCVariableFunctionManager>();
            if (manager != null)
            {
                manager.SubscribeVariableUpdate(OnVariableStateUpdate);
            }

            GetNodeInputWithIndex(0).SubscribeOnLineConnected(OnInputConnected);
            GetNodeInputWithIndex(0).SubscribeOnLineDelete(OnInputDisConnected);

            nodeData.body = null;
            nodeData.outputs = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MCVariableFunctionManager manager = FindObjectOfType<MCVariableFunctionManager>();
            if (manager != null)
            {
                manager.UnsubscribeVariableUpdate(OnVariableStateUpdate);
            }

            GetNodeInputWithIndex(0).UnSubscribeOnLineConnected(OnInputConnected);
            GetNodeInputWithIndex(0).UnSubscribeOnLineDelete(OnInputDisConnected);
        }

        protected override void UpdateNodeData()
        {
            Input input = new Input();
            input.id = 0;
            input.source = inputs[0].input.source;
            input.subid = inputs[0].input.subid;
            input.type = REEL.PROJECT.DataType.EXPRESSION;

            if (expressionDropdown.gameObject.activeSelf == true)
            {
                input.default_value = expressionDropdown.options[expressionDropdown.value].text;
            }
            else
            {
                input.default_value = string.Empty;
            }

			nodeData.inputs = new Input[] { input };

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

            expressionDropdown.value = Constants.GetExpressionVariableIndexWithName(node.inputs[0].default_value);
        }

        private void OnVariableStateUpdate()
        {
            expressionDropdown.options.Clear();
            expressionDropdown.options = Constants.GetExpressionVariableListTMPOptionData;

            if (GetNodeInputWithIndex(0).HasLine == false && expressionDropdown.options.Count > 0)
            {
                expressionDropdown.gameObject.SetActive(true);
            }
            else
            {
                expressionDropdown.gameObject.SetActive(false);
            }
        }

        int tempValue = -1;
        private void OnInputConnected(MCBezierLine line)
        {
            tempValue = expressionDropdown.value;
            expressionDropdown.gameObject.SetActive(false);
        }

        private void OnInputDisConnected()
        {
            if (expressionDropdown.options.Count > 0)
            {
                expressionDropdown.gameObject.SetActive(true);
                if (tempValue != -1)
                {
                    expressionDropdown.value = tempValue;
                    tempValue = -1;
                }
            }
        }
    }
}