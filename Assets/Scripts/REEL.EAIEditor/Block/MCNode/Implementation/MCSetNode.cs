using TMPro;

using REEL.PROJECT;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace REEL.D2EEditor
{
    public class MCSetNode : MCNode
    {
        public TMP_Dropdown variableDropdown;
        public TMP_Dropdown variableChangeDropdown;
        public TMP_InputField variableChangeInputField;
        public MCNodeInput mcNodeInput;

        private int changeDropdownValue;

        MoccaTMPDropdownHelper helper;

        private bool isLocalVariable = false;
        public bool IsLocalVariable
        {
            get
            {
                return isLocalVariable;
            }
            set
            {
                isLocalVariable = value;
                if (isLocalVariable == true)
                {
                    if (helper == null)
                    {
                        helper = GetComponentInChildren<MoccaTMPDropdownHelper>();
                    }

                    helper.type = MoccaTMPDropdownHelper.Type.LocalVariableList;
                    helper.InitializeDropdownOptionData();

                    SetInputOutputNode(variableDropdown.value);

                    MCWorkspaceManager.Instance.UnsubscribeVariableUpdate(OnTypeChanged);
                    MCWorkspaceManager.Instance.SubscribeLocalVariableUpdate(OnLocalTypeChanged);

                    MCWorkspaceManager.Instance.UnsubscribeVariableUpdate(OnVariableUpdate);
                    MCWorkspaceManager.Instance.SubscribeLocalVariableUpdate(OnLocalVariableUpdate);
                }
            }
        }

        public int CurrentVariableIndex
        {
            get
            {
                return variableDropdown.value;
            }
            private set
            {
                variableDropdown.value = value;
            }
        }

        private int currentVariableID = 0;
        public int CurrentVariableID
        {
            get
            {
                return currentVariableID;
            }
            set
            {
                currentVariableID = value;
            }
        }

        public string CurrentVariableName { get { return variableDropdown.options[variableDropdown.value].text; } }

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            variableDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            if (mcNodeInput.HasLine == false)
            {
                SetInputOutputNode(variableDropdown.value);
            }

            MCWorkspaceManager.Instance.SubscribeVariableUpdate(OnTypeChanged);
            MCWorkspaceManager.Instance.SubscribeVariableUpdate(OnVariableUpdate);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            variableDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);

            if (isLocalVariable == true)
            {
                MCWorkspaceManager.Instance.UnsubscribeLocalVariableUpdate(OnLocalTypeChanged);
                MCWorkspaceManager.Instance.UnsubscribeLocalVariableUpdate(OnLocalVariableUpdate);
            }
            else
            {
                MCWorkspaceManager.Instance.UnsubscribeVariableUpdate(OnTypeChanged);
                MCWorkspaceManager.Instance.UnsubscribeVariableUpdate(OnVariableUpdate);
            }
            
        }

        private bool shouldSetInputOutputNode = false;

        private void DelaySetInputOutputNode()
        {
            shouldSetInputOutputNode = true;
        }

        protected void LateUpdate()
        {
            if (shouldSetInputOutputNode == true)
            {
                SetInputOutputNode();
                shouldSetInputOutputNode = false;
            }
        }

        private void SetAltImage(Constants.UIType type)
        {
            switch (type)
            {
                case Constants.UIType.None:
                    mcNodeInput.altImage = null;
                    break;
                case Constants.UIType.DropDown:
                    mcNodeInput.altImage = variableChangeDropdown.GetComponent<Image>();
                    break;
                case Constants.UIType.InputField:
                    mcNodeInput.altImage = variableChangeInputField.GetComponent<Image>();
                    break;
            }
        }

        private void SetChangeUI(Action function)
        {
            LeftMenuVariableItem currentVariable;
            if (IsLocalVariable == true)
            {
                currentVariable = MCWorkspaceManager.Instance.GetLocalVariable(CurrentVariableIndex);
            }
            else
            {
                currentVariable = MCWorkspaceManager.Instance.GetVariable(CurrentVariableIndex);
            }
            
            if (currentVariable == null)
            {
                return;
            }

            DataType type = currentVariable.dataType;
            switch (type)
            {
                case DataType.BOOL:
                case DataType.NUMBER:
                case DataType.STRING:
                    UpdateUI(Constants.UIType.InputField);
                    break;

                case DataType.FACIAL:
                case DataType.MOTION:
                case DataType.MOBILITY:
                    variableChangeDropdown.GetComponent<MoccaTMPDropdownHelper>().SetType(type, function);
                    UpdateUI(Constants.UIType.DropDown);
                    break;

                case DataType.EXPRESSION:
                    //case DataType.LIST:
                    UpdateUI(Constants.UIType.None);
                    break;
            }

            if (currentVariable.nodeType == NodeType.LIST)
                UpdateUI(Constants.UIType.None);
        }

        private void SetChangeUI()
        {
            LeftMenuVariableItem currentVariable;
            if (IsLocalVariable == true)
            {
                currentVariable = MCWorkspaceManager.Instance.GetLocalVariable(CurrentVariableIndex);
            }
            else
            {
                currentVariable = MCWorkspaceManager.Instance.GetVariable(CurrentVariableIndex);
            }
            
            if (currentVariable == null)
            {
                return;
            }

            DataType type = currentVariable.dataType;
            switch (type)
            {
                case DataType.BOOL:
                case DataType.NUMBER:
                case DataType.STRING:
                    UpdateUI(Constants.UIType.InputField);
                    break;

                case DataType.FACIAL:
                case DataType.MOTION:
                case DataType.MOBILITY:
                    variableChangeDropdown.GetComponent<MoccaTMPDropdownHelper>().SetType(type);
                    UpdateUI(Constants.UIType.DropDown);
                    break;

                case DataType.EXPRESSION:
                    //case DataType.LIST:
                    UpdateUI(Constants.UIType.None);
                    break;
            }

            if (currentVariable.nodeType == NodeType.LIST)
                UpdateUI(Constants.UIType.None);
        }

        private void UpdateUI(Constants.UIType type)
        {
            switch (type)
            {
                case Constants.UIType.None:
                    TurnOnOffInputFieldDropdown(false, false);
                    break;
                case Constants.UIType.DropDown:
                    //Utils.LogBlue("UIType.DropDown");
                    TurnOnOffInputFieldDropdown(false, true);
                    break;
                case Constants.UIType.InputField:
                    //Utils.LogBlue("UIType.InputField");
                    TurnOnOffInputFieldDropdown(true, false);
                    break;
            }

            SetAltImage(type);
        }

        private void TurnOnOffInputFieldDropdown(bool inputfield, bool dropdown)
        {
            variableChangeInputField.gameObject.SetActive(inputfield);
            variableChangeDropdown.gameObject.SetActive(dropdown);
        }

        public void SetChangeDropdownIndex(int value)
        {
            variableChangeDropdown.value = value;
        }

        public void SetDropdownIndex(int value)
        {
            variableDropdown.value = value;
            SetInputOutputNode(value);
        }

        public void OnDropdownValueChanged(int index)
        {
            //Debug.Log($"[OnDropdownValueChanged] index: {index}");

            // 변수가 변경되지 않았다면 리턴.
            int variableID = IsLocalVariable ?
                MCWorkspaceManager.Instance.GetLocalVariable(index).VariableID : 
                MCWorkspaceManager.Instance.GetVariable(index).VariableID;

            //if (currentVariableID.Equals(variableID))
            //{
            //    Debug.Log($"[OnDropdownValueChanged] variableID is same");
            //    return;
            //}

            //Debug.Log($"[OnDropdownValueChanged] CurrentVariableID: {CurrentVariableID} / variableID: {variableID}");

            CurrentVariableID = variableID;
            SetInputOutputNode(index);
        }

        private void OnNodeChanged()
        {
            OnNodeStateChanged?.Invoke();
        }

        // Delay를 줘서 Invoke로 호출하기 위한 메소드(함수).
        private int tempIndex = -1;
        private void SetInputOutputNode()
        {
            if (tempIndex.Equals(-1))
            {
                return;
            }

            visitedNode.Clear();
            MCWorkspaceManager.Instance.ValidateNodeInputOutput(this);

            LeftMenuVariableItem currentVariable;
            if (isLocalVariable == true)
            {
                currentVariable = MCWorkspaceManager.Instance.GetLocalVariable(tempIndex);
                CurrentVariableID = MCWorkspaceManager.Instance.GetLocalVariable(tempIndex).VariableID;
                CurrentVariableIndex = tempIndex;
            }
            else
            {
                currentVariable = MCWorkspaceManager.Instance.GetVariable(tempIndex);
                CurrentVariableID = MCWorkspaceManager.Instance.GetVariable(tempIndex).VariableID;
                CurrentVariableIndex = tempIndex;
            }

            changeDropdownValue = Constants.GetDropdownValueIndex(currentVariable.value, currentVariable.dataType);
            SetChangeDropdownIndex(changeDropdownValue);
            //Utils.LogRed($"[MCSetNode] changeDropdownValue: {changeDropdownValue}");
            tempIndex = -1;
            OnNodeStateChanged?.Invoke();

            // 선 연결 시 InputField/Dropdown 비활성화.
            if (inputs[0].HasLine == true && inputs[0].altImage != null)
            {
                inputs[0].altImage.gameObject.SetActive(false);
            }

            // 선 연결안됐으면 InputField/Dropdown 활성화.
            if (inputs[0].HasLine == false && inputs[0].altImage != null)
            {
                inputs[0].altImage.gameObject.SetActive(true);
            }
        }

        private void SetInputOutputNode(int index)
        {
            SetInputOutputNodeType(index);
            SetNodeComponentsColor(index);

            if (index.Equals(-1))
            {
                Utils.LogRed("[MCSetNode] inde is -1");
                return;
            }

            //Utils.LogRed("[MCSetNode] before SetChangeUI");

            SetChangeUI();

            tempIndex = index;
            //Invoke("SetInputOutputNode", 0.1f);
            DelaySetInputOutputNode();

            #region 코드 백업
            //SetChangeUI(() =>
            //{
            //    //LeftMenuVariableItem currentVariable = MCWorkspaceManager.Instance.GetVariableWithID(CurrentVariableID);
            //    //if (currentVariable == null)
            //    //{
            //    //    return;
            //    //}

            //    var currentVariable = MCWorkspaceManager.Instance.GetVariable(index);
            //    CurrentVariableID = MCWorkspaceManager.Instance.GetVariable(index).VariableID;
            //    CurrentVariableIndex = index;

            //    changeDropdownValue = Constants.GetDropdownValueIndex(currentVariable.value, currentVariable.dataType);
            //    SetChangeDropdownIndex(changeDropdownValue);
            //    Utils.LogRed($"[MCSetNode] changeDropdownValue: {changeDropdownValue}");
            //});
            #endregion
        }

        private void SetInputOutputNodeType(int index)
        {
            LeftMenuVariableItem variable;
            if (isLocalVariable == true)
            {
                variable = MCWorkspaceManager.Instance.GetLocalVariable(index);
            }
            else
            {
                variable = MCWorkspaceManager.Instance.GetVariable(index);
            }
            
            if (variable == null)
            {
                return;
            }

            //outputs[0].GetComponent<MCNodeOutput>().parameterType = variable.dataType;
            inputs[0].GetComponent<MCNodeInput>().parameterType = variable.dataType;
        }

        private void SetNodeComponentsColor(int index)
        {
            LeftMenuVariableItem variable;
            if (isLocalVariable == true)
            {
                variable = MCWorkspaceManager.Instance.GetLocalVariable(index);
            }
            else
            {
                variable = MCWorkspaceManager.Instance.GetVariable(index);
            }

            if (variable == null)
            {
                return;
            }

            //outputs[0].GetComponent<Image>().color = Utils.GetParameterColor(variable.dataType, variable.nodeType);
            inputs[0].GetComponent<Image>().color = Utils.GetParameterColor(variable.dataType, variable.nodeType);
            variableDropdown.GetComponent<Image>().color = Utils.GetParameterColor(variable.dataType, variable.nodeType);
            variableChangeDropdown.GetComponent<Image>().color = Utils.GetParameterColor(variable.dataType, variable.nodeType);
            variableChangeInputField.GetComponent<Image>().color = Utils.GetParameterColor(variable.dataType, variable.nodeType);
            SetLineColor(Utils.GetParameterColor(variable.dataType, variable.nodeType));
        }

        private void SetLineColor(Color color)
        {
            //outputs[0].SetLineColor(color);
            inputs[0].SetLineColor(color);
        }

        protected override void UpdateNodeData()
        {
            //LeftMenuVariableItem currentVariable = MCWorkspaceManager.Instance.GetVariable(variableDropdown.value);
            LeftMenuVariableItem currentVariable;
            if (isLocalVariable == true)
            {
                currentVariable = MCWorkspaceManager.Instance.GetLocalVariableWithID(CurrentVariableID);
            }
            else
            {
                currentVariable = MCWorkspaceManager.Instance.GetVariableWithID(CurrentVariableID);
            }
            
            if (currentVariable == null)
            {
                if (isLocalVariable == true)
                {
                    currentVariable = MCWorkspaceManager.Instance.GetLocalVariable(variableDropdown.value);
                }
                else
                {
                    currentVariable = MCWorkspaceManager.Instance.GetVariable(variableDropdown.value);
                }

                
                if (currentVariable is null)
                {
                    return;
                }
            }

            PROJECT.Input input = new PROJECT.Input();
            input.id = 0;
            input.type = currentVariable.dataType;
            input.source = inputs[0].input.source;
            input.subid = inputs[0].input.subid;
            input.default_value = inputs[0].input.default_value;

            nodeData.inputs = new PROJECT.Input[] { input };

            if (inputs[0].HasLine)
            {
                nodeData.inputs[0].default_value = string.Empty;
            }

            Body body = new Body();
            body.name = currentVariable.nameText.text;
            body.value = currentVariable.value;
            body.isLocalVariable = IsLocalVariable;
            if (currentVariable.nodeType == PROJECT.NodeType.VARIABLE)
            {
                body.type = DataType.VARIABLE;
            }
            else if (currentVariable.nodeType == PROJECT.NodeType.LIST)
            {
                body.type = DataType.LIST;
            }
            else if (currentVariable.nodeType == PROJECT.NodeType.EXPRESSION)
            {
                body.type = DataType.EXPRESSION;
            }

            nodeData.body = body;

            //Output output = new Output();
            //output.id = 0;
            //output.type = currentVariable.dataType;
            //output.value = MCWorkspaceManager.Instance.VariableNameList[variableDropdown.value];
            //nodeData.outputs = new Output[] { output };

            nodeData.nexts = new Next[nexts.Length];
            for (int ix = 0; ix < nexts.Length; ++ix)
            {
                nodeData.nexts[ix] = nexts[ix].next;
            }   

            // Test.
            // BreakPoint 옵션 설정.
            // 0:설정 안됨, 1:설정됨.
            nodeData.hasBreakPoint = hasBreakPoint;

            // check.
            //Utils.LogRed($"[MCSetNode.UpdateNodeData]variableName: {body.name}");
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            IsLocalVariable = node.body.isLocalVariable;

            int variableIndex = IsLocalVariable ?
                MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(node.body.name) : 
            MCWorkspaceManager.Instance.GetVariableIndexWithName(node.body.name);
            CurrentVariableIndex = variableIndex;
            variableDropdown.value = variableIndex;

            if (!inputs[0].HasLine)
            {
                variableChangeInputField.text = nodeData.inputs[0].default_value;
            }
        }

        private void OnTypeChanged()
        {
            //Debug.Log("[OnTypeChanged]");
            if (variableDropdown.options.Count != MCWorkspaceManager.Instance.VariableNameList.Length)
            {
                UpdateVariableDropdownOptions();
            }

            LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetVariableWithID(CurrentVariableID);

            if (variable != null)
            {
                int index = MCWorkspaceManager.Instance.GetVariableIndexWithName(variable.VariableName);
                if (index != -1)
                {
                    OnDropdownValueChanged(index);
                }

                //Utils.LogRed($"[MCSetNode.OnTypeChanged] index: {index}");
            }
        }

        private void OnLocalTypeChanged()
        {
            if (IsLocalVariable == false)
            {
                return;
            }

            //Debug.Log("[OnLocalTypeChanged]");
            if (variableDropdown.options.Count != MCWorkspaceManager.Instance.LocalVariableNameList.Length)
            {
                UpdateLocalVariableDropdownOptions();
            }

            LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetLocalVariableWithID(CurrentVariableID);

            if (variable != null)
            {
                int index = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(variable.VariableName); 
                if (index != -1)
                {
                    OnDropdownValueChanged(index);
                }

                //Utils.LogRed($"[MCSetNode.OnLocalTypeChanged] index: {index}");
            }
        }

        private void OnVariableUpdate()
        {
            var variable = MCWorkspaceManager.Instance.GetVariableWithID(CurrentVariableID);
            if (variable == null)
            {
                return;
            }

            UpdateVariableDropdownOptions();

            int index = MCWorkspaceManager.Instance.GetVariableIndexWithName(variable.VariableName);
            OnDropdownValueChanged(index);
        }

        private void OnLocalVariableUpdate()
        {
            if (IsLocalVariable == false)
            {
                return;
            }

            var variable = MCWorkspaceManager.Instance.GetLocalVariableWithID(CurrentVariableID);
            if (variable == null)
            {
                return;
            }

            UpdateLocalVariableDropdownOptions();

            int index = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(variable.VariableName);
            OnDropdownValueChanged(index);
        }

        private void UpdateVariableDropdownOptions()
        {
            variableDropdown.options = new List<TMP_Dropdown.OptionData>();
            variableDropdown.options = Constants.TMPVariableList;
        }

        private void UpdateLocalVariableDropdownOptions()
        {
            variableDropdown.options = new List<TMP_Dropdown.OptionData>();
            variableDropdown.options = Constants.TMPLocalVariableList;
        }
    }
}