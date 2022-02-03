using TMPro;

using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCExistElementNode : MCNode
    {
        public TMP_InputField valueInput;
        public TMP_Dropdown dropdownInput;

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

            valueInput.onValueChanged.RemoveListener(inputs[1].SetAlterData);

            GetNodeInputWithIndex(0).UnSubscribeOnListConnected(OnListConnected);
            GetNodeInputWithIndex(0).UnSubscribeOnListDisConnected(OnListDisConnected);
        }

        private void OnListDisConnected()
        {
            if (GetNodeInputWithIndex(0).input.source == -1)
            {
                GetNodeInputWithIndex(1).parameterType = DataType.NONE;
                GetNodeInputWithIndex(1).input.type = DataType.NONE;
                GetNodeInputWithIndex(1).SetLineColor(Utils.GetParameterColor(DataType.NONE));

                valueInput.gameObject.SetActive(false);
                dropdownInput.gameObject.SetActive(false);
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

                GetNodeInputWithIndex(1).parameterType = variable.dataType;
                GetNodeInputWithIndex(1).input.type = variable.dataType;
                GetNodeInputWithIndex(1).SetLineColor(Utils.GetParameterColor(variable.dataType));

                if (IsDropdownType(variable.dataType) == true)
                {
                    dropdownInput.gameObject.SetActive(true);
                    valueInput.gameObject.SetActive(false);

                    dropdownInput.GetComponent<Image>().color = Utils.GetParameterColor(variable.dataType);

                    MoccaTMPDropdownHelper helper = dropdownInput.GetComponent<MoccaTMPDropdownHelper>();
                    helper.SetType(variable.dataType);
                }

                if (IsInputType(variable.dataType) == true)
                {
                    dropdownInput.gameObject.SetActive(false);
                    valueInput.gameObject.SetActive(true);

                    valueInput.GetComponent<Image>().color = Utils.GetParameterColor(variable.dataType);
                }
            }
            else if (connectedNode is MCDetectPersonNode || connectedNode is MCAgeGenderNode)
            {
                MCDetectPersonNode detectPersonNode = connectedNode as MCDetectPersonNode;
                GetNodeInputWithIndex(1).parameterType = DataType.STRING;
                GetNodeInputWithIndex(1).input.type = DataType.STRING;
                GetNodeInputWithIndex(1).SetLineColor(Utils.GetParameterColor(DataType.STRING));
            }
        }

        private bool IsDropdownType(DataType dataType)
        {
            if (dataType == DataType.FACIAL || dataType == DataType.MOTION || dataType == DataType.MOBILITY)
            {
                return true;
            }

            return false;
        }

        private bool IsInputType(DataType dataType)
        {
            if (dataType == DataType.BOOL || dataType  == DataType.NUMBER || dataType == DataType.STRING)
            {
                return true;
            }

            return false;
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
                    type = PROJECT.DataType.BOOL,
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

            valueInput.text = nodeData.inputs[1].default_value;
        }
    }
}