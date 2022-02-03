using TMPro;

using System.Collections.Generic;
using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCGetNode : MCNode
    {
        public TMP_Dropdown variableDropdown;

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

        public int CurrentVariableID { get; set; }

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

                    //Utils.LogGreen("[SubscribeLocalVariableUpdate]");
                    MCWorkspaceManager.Instance.UnsubscribeVariableUpdate(OnTypeChanged);
                    MCWorkspaceManager.Instance.SubscribeLocalVariableUpdate(OnLocalTypeChanged);

                    MCWorkspaceManager.Instance.UnsubscribeVariableUpdate(OnVariableUpdate);
                    MCWorkspaceManager.Instance.SubscribeLocalVariableUpdate(OnLocalVariableUpdate);
                }
            }
        }

        private bool shouldSetOutputNode = false;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            variableDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            tempIndex = variableDropdown.value;
            shouldSetOutputNode = true;
            //Invoke("SetOutputNode", 0.1f);
            //SetOutputNode(variableDropdown.value);

            MCWorkspaceManager.Instance.SubscribeVariableUpdate(OnTypeChanged);
            MCWorkspaceManager.Instance.SubscribeVariableUpdate(OnVariableUpdate);

            //if (IsLocalVariable == true)
            //{
            //    Utils.LogGreen("[SubscribeLocalVariableUpdate]");
            //    MCWorkspaceManager.Instance.SubscribeLocalVariableUpdate(OnLocalTypeChanged);
            //}
            //else
            //{
            //    Utils.LogGreen("[SubscribeVariableUpdate]");
            //    MCWorkspaceManager.Instance.SubscribeVariableUpdate(OnTypeChanged);
            //}
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            variableDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);

            if (IsLocalVariable == true)
            {
                MCWorkspaceManager.Instance.UnsubscribeLocalVariableUpdate(OnLocalTypeChanged);
            }
            else
            {
                MCWorkspaceManager.Instance.UnsubscribeVariableUpdate(OnTypeChanged);
            }
        }

        private void LateUpdate()
        {
            if (shouldSetOutputNode == true)
            {
                shouldSetOutputNode = false;
                SetOutputNode();
            }
        }

        public void SetDropdownIndex(int value)
        {
            variableDropdown.value = value;

            tempIndex = value;
            shouldSetOutputNode = true;
            //Invoke("SetOutputNode", 0.1f);
            //SetOutputNode(value);
        }

        int tempIndex = -1;
        public void OnDropdownValueChanged(int index)
        {
            //Debug.Log($"OnDropdownValueChanged, ID: {NodeID}");

            tempIndex = index;
            shouldSetOutputNode = true;
            //Invoke("SetOutputNode", 0.1f);
            //SetOutputNode(index);

            //MCWorkspaceManager.Instance.ValidateNodeInputOutput(this);
        }
        
        private void SetOutputNode()
        {
            if (tempIndex == -1)
            {
                return;
            }

            SetOutputNodeColor(tempIndex);
            SetOutputNodeType(tempIndex);

            MCNode.visitedNode.Clear();
            MCWorkspaceManager.Instance.ValidateNodeInputOutput(this);

            if (IsLocalVariable == true)
            {
                CurrentVariableID = MCWorkspaceManager.Instance.GetLocalVariable(tempIndex).VariableID;
            }
            else
            {
                CurrentVariableID = MCWorkspaceManager.Instance.GetVariable(tempIndex).VariableID;
            }

            CurrentVariableIndex = tempIndex;
            //Utils.LogRed($"variableName: {MCWorkspaceManager.Instance.GetVariable(CurrentVariableIndex).VariableName}");
            tempIndex = -1;

            OnNodeStateChanged?.Invoke();
        }

        //private void SetOutputNode(int index)
        //{
        //    SetOutputNodeColor(index);
        //    SetOutputNodeType(index);
        //}

        private void SetOutputNodeType(int index)
        {
            LeftMenuVariableItem variable = null;
            if (IsLocalVariable == true)
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

            outputs[0].GetComponent<MCNodeOutput>().parameterType = variable.dataType;
        }

        private void SetOutputNodeColor(int index)
        {
            LeftMenuVariableItem variable = null;
            if (IsLocalVariable == true)
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

            outputs[0].GetComponent<Image>().color = Utils.GetParameterColor(variable.dataType, variable.nodeType);
            variableDropdown.GetComponent<Image>().color = Utils.GetParameterColor(variable.dataType, variable.nodeType);
            SetLineColor(Utils.GetParameterColor(variable.dataType, variable.nodeType));
        }

        private void SetLineColor(Color color)
        {
            outputs[0].SetLineColor(color);
        }

        protected override void UpdateNodeData()
        {
            Body body = new Body();
            LeftMenuVariableItem currentVariable = null;
            if (IsLocalVariable == true)
            {
                currentVariable = MCWorkspaceManager.Instance.GetLocalVariable(variableDropdown.value);
            }
            else
            {
                currentVariable = MCWorkspaceManager.Instance.GetVariable(variableDropdown.value);
            }
            
            if (currentVariable == null)
            {
                return;
            }

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

            Output output = new Output();
            output.id = 0;
            output.type = currentVariable.dataType;
            output.value = body.isLocalVariable ?
                MCWorkspaceManager.Instance.LocalVariableNameList[variableDropdown.value] :
                MCWorkspaceManager.Instance.VariableNameList[variableDropdown.value];
            nodeData.outputs = new Output[] { output };

            // Test.
            // BreakPoint 옵션 설정.
            // 0:설정 안됨, 1:설정됨.
            nodeData.hasBreakPoint = hasBreakPoint;

            //Utils.LogRed($"[MCGetNode.UpdateNodeData]variableName: {body.name}");
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            IsLocalVariable = node.body.isLocalVariable;

            int variableIndex = -1;
            if (IsLocalVariable == true)
            {
                variableIndex = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(node.body.name);
            }
            else
            {
                variableIndex = MCWorkspaceManager.Instance.GetVariableIndexWithName(node.body.name);
            }
            
            variableDropdown.value = variableIndex;
        }

        private void OnTypeChanged()
        {
            if (variableDropdown.options.Count != MCWorkspaceManager.Instance.VariableNameList.Length)
            {
                variableDropdown.options = new List<TMP_Dropdown.OptionData>();
                variableDropdown.options = Constants.TMPVariableList;
            }

            LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetVariableWithID(CurrentVariableID);
            if (variable != null)
            {
                int index = MCWorkspaceManager.Instance.GetVariableIndexWithName(variable.VariableName);
                //Utils.LogRed($"[MCGetNode] variableName: {variable.VariableName}");
                OnDropdownValueChanged(index);
                //Utils.LogRed($"[MCGetNode.OnTypeChanged] index: {index}");
            }
        }

        private void OnLocalTypeChanged()
        {
            if (IsLocalVariable == false)
            {
                return;
            }

            if (variableDropdown.options.Count != MCWorkspaceManager.Instance.LocalVariableNameList.Length)
            {
                UpdateLocalVariableDropdownOptions();
            }

            LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetLocalVariableWithID(CurrentVariableID);
            if (variable != null)
            {
                int index = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(variable.VariableName);
                OnDropdownValueChanged(index);
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