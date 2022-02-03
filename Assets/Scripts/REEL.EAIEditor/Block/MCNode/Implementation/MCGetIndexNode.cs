using TMPro;
using UnityEngine;
using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCGetIndexNode : MCNode
    {
        public TMP_Dropdown valueDropdown;
        public TMP_InputField valueInput;

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
            GetNodeInputWithIndex(0).SubscribeOnListDisConnected(OnListDisconnected);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            GetNodeInputWithIndex(0).UnSubscribeOnListConnected(OnListConnected);
            GetNodeInputWithIndex(0).UnSubscribeOnListDisConnected(OnListDisconnected);
        }

        private void SetDataInput(DataType type)
        {
            inputs[1].parameterType = type;
            inputs[1].input.type = type;
            //inputs[1].input.default_value = string.Empty;
            inputs[1].SetLineColor(Utils.GetParameterColor(type));
        }

        private void OnListConnected()
        {
            MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(inputs[0].input.source);
            if (connectedNode is MCGetNode)
            {
                MCGetNode getNode = connectedNode as MCGetNode;
                getNode.SubscribeOnNodeStateChanged(OnConnectedNodeDropdownChanged);
                UpdateDataState(getNode);
            }
        }

        private void OnListDisconnected()
        {
            if (GetNodeInputWithIndex(0).input.source == -1)
            {
                SetDataInput(DataType.NONE);
                nodeData.inputs[1].default_value = string.Empty;
                valueInput.text = string.Empty;
                valueInput.image.color = Utils.GetParameterColor(DataType.NONE);
                TurnOnOffInputFieldDropdown(false, false);
            }

            MCWorkspaceManager.Instance.ValidateNodeInputOutput(this);
        }

        private void TurnOnOffInputFieldDropdown(bool inputField, bool dropdown)
        {
            valueInput.gameObject.SetActive(inputField);
            valueDropdown.gameObject.SetActive(dropdown);
        }

        private void OnConnectedNodeDropdownChanged()
        {
            if (GetNodeInputWithIndex(0).input.source == -1)
            {
                return;
            }

            MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(GetNodeInputWithIndex(0).input.source);
            if (connectedNode != null && connectedNode is MCGetNode)
            {
                MCGetNode getNode = connectedNode as MCGetNode;
                UpdateDataState(getNode);
            }
        }

        private void UpdateDataState(MCGetNode getNode)
        {
            LeftMenuVariableItem variable;
            if (getNode.IsLocalVariable == true)
            {
                variable = MCWorkspaceManager.Instance.GetLocalVariable(getNode.CurrentVariableIndex);
            }
            else
            {
                variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
            }

            SetDataInput(variable.dataType);
            ChangeInputFieldDropdown(variable);
            if (IsInputDropdownType(variable.dataType) == true)
            {
                SetDropdownOptiondata(variable);
            }

            OnNodeStateChanged?.Invoke();
        }

        private void SetDropdownHelperType(MoccaTMPDropdownHelper.Type type)
        {
            valueDropdown.AddOptions(Constants.GetTMPOptionDataWithType(type));
            valueDropdown.GetComponent<MoccaTMPDropdownHelper>().type = type;
        }

        private void ChangeInputFieldDropdown(LeftMenuVariableItem variable)
        {
            DataType type = variable.dataType;
            if (type == DataType.BOOL || type == DataType.NUMBER || type == DataType.STRING)
            {
                TurnOnOffInputFieldDropdown(true, false);
                SetAltImage(Constants.UIType.InputField);
                valueInput.image.color = Utils.GetParameterColor(variable.dataType);
            }
            else
            {
                TurnOnOffInputFieldDropdown(false, true);
                SetAltImage(Constants.UIType.DropDown);
                valueDropdown.image.color = Utils.GetParameterColor(variable.dataType);
            }
        }

        private void SetDropdownOptiondata(LeftMenuVariableItem variable)
        {
            valueDropdown.ClearOptions();

            DataType type = variable.dataType;
            inputs[1].SetAlterData(0);
            switch (type)
            {
                case DataType.FACIAL:
                    SetDropdownHelperType(MoccaTMPDropdownHelper.Type.Facial);
                    break;
                case DataType.MOTION:
                    SetDropdownHelperType(MoccaTMPDropdownHelper.Type.Motion);
                    break;
                case DataType.MOBILITY:
                    SetDropdownHelperType(MoccaTMPDropdownHelper.Type.Mobility);
                    break;
            }

            if (inputs[1].input.default_value != string.Empty)
            {
                valueDropdown.value = Constants.GetDropdownValueIndex(nodeData.inputs[1].default_value, type);
            }
        }

        private void SetDropdownOptiondata(DataType type)
        {
            valueDropdown.ClearOptions();

            inputs[1].SetAlterData(0);
            switch (type)
            {
                case DataType.FACIAL:
                    SetDropdownHelperType(MoccaTMPDropdownHelper.Type.Facial);
                    break;
                case DataType.MOTION:
                    SetDropdownHelperType(MoccaTMPDropdownHelper.Type.Motion);
                    break;
                case DataType.MOBILITY:
                    SetDropdownHelperType(MoccaTMPDropdownHelper.Type.Mobility);
                    break;
            }

            if (inputs[1].input.default_value != string.Empty)
            {
                valueDropdown.value = Constants.GetDropdownValueIndex(nodeData.inputs[1].default_value, type);
            }
        }

        private void SetAltImage(Constants.UIType type)
        {
            switch (type)
            {
                case Constants.UIType.None:
                    inputs[1].altImage = null;
                    break;
                case Constants.UIType.DropDown:
                    inputs[1].altImage = valueDropdown.image;
                    break;
                case Constants.UIType.InputField:
                    inputs[1].altImage = valueInput.image;
                    break;
            }
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            DataType type = nodeData.inputs[1].type;
            inputs[1].input.type = type;
            inputs[1].input.default_value = nodeData.inputs[1].default_value;
            if (IsInputDropdownType(type) == true)
            {
                SetDropdownOptiondata(nodeData.inputs[1].type);
                valueDropdown.value = Constants.GetDropdownValueIndex(nodeData.inputs[1].default_value, type);
            }
            else
            {
                valueInput.text = nodeData.inputs[1].default_value;
            }
        }

        private bool IsInputDropdownType(DataType type)
        {
            return type == DataType.FACIAL
                || type == DataType.MOTION
                || type == DataType.MOBILITY;
        }
    }
}