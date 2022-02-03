using TMPro;

using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCInsertNode : MCNode
    {
        public TMP_InputField indexInput;
        public TMP_InputField dataInput;
        public TMP_Dropdown dataDropdown;
        public MCNodeInput mcNodeInput;

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

            GetNodeInputWithIndex(0).UnSubscribeOnListConnected(OnListConnected);
            GetNodeInputWithIndex(0).UnSubscribeOnListDisConnected(OnListDisConnected);
        }

        private void SetDataInput(DataType type)
        {
            GetNodeInputWithIndex(2).parameterType = type;
            GetNodeInputWithIndex(2).input.type = type;
            GetNodeInputWithIndex(2).SetLineColor(Utils.GetParameterColor(type));
        }

        private void OnListDisConnected()
        {
            if (GetNodeInputWithIndex(0).input.source == -1)
            {
                SetDataInput(DataType.NONE);
                dataInput.image.color = Utils.GetParameterColor(DataType.NONE);
                TurnOnOffInputFieldDropdown(false, false);
            }

            MCWorkspaceManager.Instance.ValidateNodeInputOutput(this);
        }

        private void OnListConnected()
        {
            MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(GetNodeInputWithIndex(0).input.source);
            if (connectedNode is MCGetNode)
            {
                MCGetNode getNode = connectedNode as MCGetNode;
                getNode.SubscribeOnNodeStateChanged(OnConnectedNodeDropdownChanged);
                UpdateDataState(getNode);
                //LeftMenuVariableItem variable;
                //if (getNode.IsLocalVariable == true)
                //{
                //    variable = MCWorkspaceManager.Instance.GetLocalVariable(getNode.CurrentVariableIndex);
                //}
                //else
                //{
                //    variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
                //}

                //SetDataInput(variable.dataType);
                //ChangeInputFieldDropdown(variable);
                //SetDropdownOptiondata(variable);

                //OnNodeStateChanged?.Invoke();
            }
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
                //LeftMenuVariableItem variable;
                //if (getNode.IsLocalVariable == true)
                //{
                //    variable = MCWorkspaceManager.Instance.GetLocalVariable(getNode.CurrentVariableIndex);
                //}
                //else
                //{
                //    variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
                //}

                //SetDataInput(variable.dataType);
                //ChangeInputFieldDropdown(variable);
                //SetDropdownOptiondata(variable);

                //OnNodeStateChanged?.Invoke();
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
            SetDropdownOptiondata(variable);

            OnNodeStateChanged?.Invoke();
        }

        private void SetAltImage(Constants.UIType type)
        {
            switch (type)
            {
                case Constants.UIType.None:
                    mcNodeInput.altImage = null;
                    break;
                case Constants.UIType.DropDown:
                    mcNodeInput.altImage = dataDropdown.image;
                    break;
                case Constants.UIType.InputField:
                    mcNodeInput.altImage = dataInput.image;
                    break;
            }
        }

        private void TurnOnOffInputFieldDropdown(bool inputfield, bool dropdown)
        {
            dataInput.gameObject.SetActive(inputfield);
            dataDropdown.gameObject.SetActive(dropdown);
        }

        private void SetDropdownOptiondata(LeftMenuVariableItem variable)
        {
            dataDropdown.ClearOptions();

            DataType type = variable.dataType;
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
        }

        private void SetDropdownHelperType(MoccaTMPDropdownHelper.Type type)
        {
            dataDropdown.AddOptions(Constants.GetTMPOptionDataWithType(type));
            dataDropdown.GetComponent<MoccaTMPDropdownHelper>().type = type;
        }

        private void ChangeInputFieldDropdown(LeftMenuVariableItem variable)
        {
            DataType type = variable.dataType;
            if (type == DataType.BOOL || type == DataType.NUMBER || type == DataType.STRING)
            {
                TurnOnOffInputFieldDropdown(true, false);
                SetAltImage(Constants.UIType.InputField);
                dataInput.image.color = Utils.GetParameterColor(variable.dataType);
            }
            else
            {
                TurnOnOffInputFieldDropdown(false, true);
                SetAltImage(Constants.UIType.DropDown);
                dataDropdown.image.color = Utils.GetParameterColor(variable.dataType);
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

            indexInput.text = nodeData.inputs[1].default_value;

            DataType type = nodeData.inputs[2].type;
            if (type == DataType.BOOL || type == DataType.STRING || type == DataType.NUMBER)
            {
                dataInput.text = nodeData.inputs[2].default_value;
                return;
            }
                
            if (type == DataType.FACIAL || type == DataType.MOTION || type == DataType.MOBILITY)
            {
                dataDropdown.value = Constants.GetDropdownValueIndex(nodeData.inputs[2].default_value, type);
            }
        }
    }
}