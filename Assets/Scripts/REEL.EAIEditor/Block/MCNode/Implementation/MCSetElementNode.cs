#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCSetElementNode : MCNode
    {
#if USINGTMPPRO
        public TMP_InputField indexInput;
        public TMP_InputField dataInput;
        public TMP_Dropdown dataDropdown;
#else
        public InputField indexInput;
        public InputField dataInput;
        public Dropdown dataDropdown;
#endif
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
            //else if(connectedNode is MCSetNode)
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

            bool isDropdown = false;
            string defaultValue = string.Empty;

            DataType type = variable.dataType;
            switch (type)
            {
                case DataType.FACIAL:
#if USINGTMPPRO
                    SetDropdownHelperType(MoccaTMPDropdownHelper.Type.Facial);
#else
                    SetDropdownHelperType(MoccaDropdownHelper.Type.Facial);
#endif
                    int indexValue = 0;
                    //Debug.LogWarning($"indexInput.text: {indexInput.text}");
                    if (int.TryParse(indexInput.text, out indexValue))
                    {
                        defaultValue = Constants.ParseFacialKoreanToEnglish(indexValue);
                        isDropdown = true;
                        //Debug.LogWarning($"defaultValue: {defaultValue}");
                    }
                    
                    break;
                case DataType.MOTION:
#if USINGTMPPRO
                    SetDropdownHelperType(MoccaTMPDropdownHelper.Type.Motion);
#else
                    SetDropdownHelperType(MoccaDropdownHelper.Type.Motion);
#endif
                    indexValue = 0;
                    if (int.TryParse(indexInput.text, out indexValue))
                    {
                        defaultValue = Constants.ParseMotionKoreanToEnglish(dataDropdown.value);
                        isDropdown = true;
                    }
                    
                    break;
                case DataType.MOBILITY:
#if USINGTMPPRO
                    SetDropdownHelperType(MoccaTMPDropdownHelper.Type.Mobility);
#else
                    SetDropdownHelperType(MoccaDropdownHelper.Type.Mobility);
#endif
                    indexValue = 0;
                    if (int.TryParse(indexInput.text, out indexValue))
                    {
                        defaultValue = Constants.ParseMobilityKoreanToEnglish(dataDropdown.value);
                        isDropdown = true;
                    }
                        
                    break;
            }

            // 드롭다운일 경우 드롭다운 값을 기본 값으로 설정.
            if (isDropdown)
            {
                GetNodeInputWithIndex(2).input.default_value = defaultValue;
            }
        }

#if USINGTMPPRO
        private void SetDropdownHelperType(MoccaTMPDropdownHelper.Type type)
#else
        private void SetDropdownHelperType(MoccaDropdownHelper.Type type)
#endif
        {
#if USINGTMPPRO
            dataDropdown.AddOptions(Constants.GetTMPOptionDataWithType(type));
            dataDropdown.GetComponent<MoccaTMPDropdownHelper>().type = type;
#else
            dataDropdown.AddOptions(Constants.GetOptionDataWithType(type));
            dataDropdown.GetComponent<MoccaDropdownHelper>().type = type;
#endif
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
            dataInput.text = nodeData.inputs[2].default_value;
            //dropdown일 경우도 추가해야함.
        }
    }
}