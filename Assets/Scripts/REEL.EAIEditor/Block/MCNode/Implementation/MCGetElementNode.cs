#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCGetElementNode : MCNode
    {
#if USINGTMPPRO
        public TMP_InputField indexInput;
#else
        public InputField indexInput;
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

            GetNodeInputWithIndex(0).SubscribeOnListConnected(OnListConnected);
            GetNodeInputWithIndex(0).SubscribeOnListDisConnected(OnListDisConnected);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            indexInput.onValueChanged.RemoveListener(inputs[1].SetAlterData);

            GetNodeInputWithIndex(0).UnSubscribeOnListConnected(OnListConnected);
            GetNodeInputWithIndex(0).UnSubscribeOnListDisConnected(OnListDisConnected);
        }

        private void OnListDisConnected()
        {
            if (GetNodeInputWithIndex(0).input.source == -1)
            {
                GetNodeOutputWithIndex(0).parameterType = DataType.NONE;
                GetNodeOutputWithIndex(0).output.type = DataType.NONE;
                GetNodeOutputWithIndex(0).SetLineColor(Utils.GetParameterColor(DataType.NONE));
            }

            MCWorkspaceManager.Instance.ValidateNodeInputOutput(this);
        }

        private void OnListConnected()
        {
            MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(GetNodeInputWithIndex(0).input.source);
            if (connectedNode is MCGetNode)
            {
                MCGetNode getNode = connectedNode as MCGetNode;
                LeftMenuVariableItem variable;
                if (getNode.IsLocalVariable == true)
                {
                    variable = MCWorkspaceManager.Instance.GetLocalVariable(getNode.CurrentVariableIndex);
                }
                else
                {
                    variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
                }
                
                GetNodeOutputWithIndex(0).parameterType = variable.dataType;
                GetNodeOutputWithIndex(0).output.type = variable.dataType;
                GetNodeOutputWithIndex(0).SetLineColor(Utils.GetParameterColor(variable.dataType));
            }
            else if (connectedNode is MCDetectPersonNode || connectedNode is MCAgeGenderNode)
            {
                MCDetectPersonNode detectPersonNode = connectedNode as MCDetectPersonNode;
                GetNodeOutputWithIndex(0).parameterType = DataType.STRING;
                GetNodeOutputWithIndex(0).output.type = DataType.STRING;
                GetNodeOutputWithIndex(0).SetLineColor(Utils.GetParameterColor(DataType.STRING));
            }
        }

        protected override void UpdateNodeData()
        {
            nodeData.inputs = new PROJECT.Input[inputs.Length];
            for (int ix = 0; ix < inputs.Length; ++ix)
            {
                nodeData.inputs[ix] = new PROJECT.Input();
                nodeData.inputs[ix].id = inputs[ix].input.id;
                nodeData.inputs[ix].type = inputs[ix].input.type;
                nodeData.inputs[ix].source = inputs[ix].input.source;
                nodeData.inputs[ix].subid = inputs[ix].input.subid;
                nodeData.inputs[ix].default_value = inputs[ix].input.default_value;

                if (inputs[ix].HasLine)
                {
                    nodeData.inputs[ix].default_value = string.Empty;
                }
            }

            nodeData.outputs = new PROJECT.Output[1]
            {
                new PROJECT.Output()
                {
                    id = 0,
                    type = PROJECT.DataType.STRING,
                    value = "_result"
                }
            };

            // Test.
            // BreakPoint 옵션 설정.
            // 0:설정 안됨, 1:설정됨.
            nodeData.hasBreakPoint = hasBreakPoint;
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            indexInput.text = nodeData.inputs[1].default_value;
            outputs[0].parameterType = nodeData.outputs[0].type;
        }
    }
}