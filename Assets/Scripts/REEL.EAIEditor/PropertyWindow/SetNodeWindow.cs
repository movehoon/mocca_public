using TMPro;

using Newtonsoft.Json;
using REEL.PROJECT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace REEL.D2EEditor
{
    public class SetNodeWindow : GetSetNodeWindow
    {
        // 속성창 프로퍼티.
        [SerializeField] private TMP_Dropdown setVarDropdown;
        [SerializeField] private TMP_Dropdown nodeValueDropdown;
        [SerializeField] private TMP_InputField nodeValueInput;

        // 연결된 노드 프로퍼티.
        //[SerializeField] private TMP_Dropdown targetNodeVarDropdown;
        //[SerializeField] private TMP_Dropdown targetNodeValueDropdown;
        //[SerializeField] private TMP_InputField targetNodeValueInput;

        [SerializeField] private TMP_Text variableType;
        [SerializeField] private TMP_Text variableName;

        protected override void OnDisable()
        {
            base.OnDisable();
            if (nodeValueDropdown != null)
            {
                nodeValueDropdown.onValueChanged.RemoveAllListeners();
            }
            
            if (nodeValueInput != null)
            {
                nodeValueInput.onValueChanged.RemoveAllListeners();
            }
        }

        private void SyncTwoInputFields()
        {
            nodeValueInput.onValueChanged.AddListener((text) =>
            {
                valueBlocks[0].value.text = text;
            });

            nodeValueDropdown.onValueChanged.AddListener((value) =>
            {
                valueBlocks[0].value.text = nodeValueDropdown.options[value].text;
            });
        }

        protected override void UpdateProperty()
        {
            base.UpdateProperty();

            MCNode node = refNode;

            SetTargetInputFieldAndDropdown(node);

            MCSetNode setNode = node as MCSetNode;

            LeftMenuVariableItem variable;
            if (setNode.IsLocalVariable == true)
            {
                variable = MCWorkspaceManager.Instance.GetLocalVariable(setVarDropdown.value);
            }
            else
            {
                variable = MCWorkspaceManager.Instance.GetVariable(setVarDropdown.value);
            }

            variableType.text = variable.dataType.ToString();
            variableName.text = setVarDropdown.options[setVarDropdown.value].text;

            int connectedNodeID = node.GetNodeInputWithIndex(0).input.source;
            MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);

            
            MCNodeInput input1 = setNode.GetNodeInputWithIndex(0);
            MCNodeInput input2 = setNode.GetNodeInputWithIndex(1);

            if (variable.nodeType == PROJECT.NodeType.VARIABLE)
            {
                currentVarCount = 1;
                SetBlocks(variable.nodeType, currentVarCount);

                // input 연결X
                if (input1.HasLine == false)
                {
                    //SelectInputType(variable, node);

                    SyncTwoInputFields();
                    DataType dataType = variable.dataType;
                    if (dataType == DataType.BOOL || dataType == DataType.NUMBER || dataType == DataType.STRING)
                    {
                        valueBlocks[0].value.text = setNode.variableChangeInputField.text;
                    }

                    else if (dataType == DataType.FACIAL || dataType == DataType.MOTION || dataType == DataType.MOBILITY)
                    {
                        //Utils.LogRed($"setNode.variableChangeDropdown.options.Count: {setNode.variableChangeDropdown.options.Count} / setNode.variableChangeDropdown.value: {setNode.variableChangeDropdown.value}");
                        valueBlocks[0].value.text = setNode.variableChangeDropdown.options[setNode.variableChangeDropdown.value].text;
                    }
                }

                if (input1.HasLine == true)
                {
                    //SelectInputType(variable, node);
                    DataType dataType = variable.dataType;
                    if (connectedNode is MCGetNode)
                    {
                        valueBlocks[0].value.text = Constants.GetValueForKorean(variable.value, variable.dataType);
                    }
                }
            }
            else if (variable.nodeType == PROJECT.NodeType.LIST)
            {
                ListValue listValue = JsonConvert.DeserializeObject<PROJECT.ListValue>(variable.value);

                currentVarCount = listValue.listValue.Length;
                SetBlocks(variable.nodeType, currentVarCount);

                //SelectInputType(variable, node);
                for (int ix = 0; ix < currentVarCount; ix++)
                {
                    valueBlocks[ix].value.text = Constants.GetValueForKorean(listValue.listValue[ix], variable.dataType);
                }
            }
            else if (variable.nodeType == PROJECT.NodeType.EXPRESSION)
            {
                if (input1.HasLine == false) //input 연결X
                {
                    SetExpressProperty(variable);
                }
                
                if (input1.HasLine == true)
                {
                    MCGetNode getNode = connectedNode as MCGetNode;
                    LeftMenuVariableItem expVariable;
                    if (getNode.IsLocalVariable == true)
                    {
                        expVariable = MCWorkspaceManager.Instance.GetLocalVariable(getNode.CurrentVariableIndex);
                    }
                    else
                    {
                        expVariable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
                    }
                    
                    SetExpressProperty(expVariable);
                }

                scrollRect.content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight + currentVarCount * itemHeight * 4 + itemHeight);
                return;
            }

            scrollRect.content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight + (currentVarCount + 1) * itemHeight);
        }

        //        protected override void UpdateProperty()
        //        {
        //            base.UpdateProperty();

        //            MCNode node = refNode;

        //            SetTargetInputFieldAndDropdown(node);

        //            LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetVariable(setVarDropdown.value);

        //            variableType.text = variable.dataType.ToString();
        //            variableName.text = setVarDropdown.options[setVarDropdown.value].text;

        //            int connectedNodeID = node.GetNodeInputWithIndex(0).input.source;
        //            MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);

        //            if (variable.nodeType == PROJECT.NodeType.VARIABLE)
        //            {
        //                currentVarCount = 1;
        //                SetBlocks(variable.nodeType, currentVarCount);

        //                if (connectedNode == null) //input 연결X
        //                {
        //                    SelectInputType(variable, node);
        //                    if (nodeValueInput != null && nodeValueInput.gameObject.activeSelf)
        //                    {
        //                        nodeValueInput.interactable = true;
        //#if USINGTMPPRO
        //                        nodeValueInput.text = node.GetComponentInChildren<TMP_InputField>().text;
        //#else
        //                                nodeValueInput.text = node.GetComponentInChildren<InputField>().text;
        //#endif
        //                        nodeValueInput.onValueChanged.AddListener(SyncInputField);
        //                    }
        //                    else if (nodeValueDropdown != null && nodeValueDropdown.gameObject.activeSelf)
        //                    {
        //                        nodeValueDropdown.interactable = true;
        //#if USINGTMPPRO
        //                        nodeValueDropdown.value = node.GetComponentsInChildren<TMP_Dropdown>()[1].value;
        //#else
        //                                nodeValueDropdown.value = node.GetComponentsInChildren<Dropdown>()[1].value;
        //#endif
        //                        nodeValueDropdown.onValueChanged.AddListener(SyncDropdown);
        //                    }
        //                }
        //                else //input 연결o
        //                {
        //                    SelectInputType(variable, node);
        //                    if (nodeValueInput != null && nodeValueInput.gameObject.activeSelf)
        //                    {
        //                        nodeValueInput.interactable = false;
        //                        if (connectedNode is MCGetNode)
        //                        {
        //                            nodeValueInput.text = Constants.GetValueForKorean(variable.value, variable.dataType);
        //#if USINGTMPPRO
        //                            onOffBlocks[0].GetComponentsInChildren<TMP_Text>()[1].text = string.Empty;
        //#else
        //                                    onOffBlocks[0].GetComponentsInChildren<Text>()[1].text = string.Empty;
        //#endif
        //                        }
        //                        else
        //                            onOffBlocks[0].SetActive(false);
        //                    }
        //                    else if (nodeValueDropdown != null && nodeValueDropdown.gameObject.activeSelf)
        //                    {
        //                        nodeValueDropdown.interactable = false;
        //                        if (connectedNode is MCGetNode)
        //                        {
        //                            nodeValueDropdown.captionText.text = Constants.GetValueForKorean(variable.value, variable.dataType);
        //#if USINGTMPPRO
        //                            onOffBlocks[0].GetComponentsInChildren<TMP_Text>()[1].text = string.Empty;
        //#else
        //                                    onOffBlocks[0].GetComponentsInChildren<Text>()[1].text = string.Empty;
        //#endif
        //                        }
        //                        else
        //                            onOffBlocks[0].SetActive(false);
        //                    }
        //                }
        //            }
        //            else if (variable.nodeType == PROJECT.NodeType.LIST)
        //            {
        //                PROJECT.ListValue listValue = JsonConvert.DeserializeObject<PROJECT.ListValue>(variable.value);

        //                currentVarCount = listValue.listValue.Length;
        //                SetBlocks(variable.nodeType, currentVarCount);

        //                SelectInputType(variable, node);
        //                for (int i = 0; i < currentVarCount; i++)
        //                {
        //                    valueBlocks[i].value.text = Constants.GetValueForKorean(listValue.listValue[i], variable.dataType);
        //                }
        //            }
        //            else if (variable.nodeType == PROJECT.NodeType.EXPRESSION)
        //            {
        //                if (connectedNode == null) //input 연결X
        //                {
        //                    SetExpressProperty(variable);
        //                }
        //                else //input 연결o, Expression은 input으로 들어올 수 있는게 get노드 밖에 없음.
        //                {
        //                    MCGetNode getNode = connectedNode as MCGetNode;
        //                    LeftMenuVariableItem expVariable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
        //                    SetExpressProperty(expVariable);
        //                }
        //            }

        //            if (variable.nodeType == PROJECT.NodeType.EXPRESSION)
        //            {
        //                scrollRect.content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight + currentVarCount * itemHeight * 4 + itemHeight);
        //            }
        //            else
        //            {
        //                scrollRect.content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight + (currentVarCount + 1) * itemHeight);
        //            }
        //        }

        private void SetTargetInputFieldAndDropdown(MCNode node)
        {
            MCSetNode setNode = node as MCSetNode;
            setVarDropdown = setNode.variableDropdown;
            nodeValueDropdown = setNode.variableChangeDropdown;
            nodeValueInput = setNode.variableChangeInputField;

            //targetNodeVarDropdown = setNode.variableDropdown;
            //targetNodeValueDropdown = setNode.variableChangeDropdown;
            //targetNodeValueInput = setNode.variableChangeInputField;
        }

        #region 기존 ShowProperty 백업
        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            UpdateProperty();
        }
        #endregion

        private void SelectInputType(LeftMenuVariableItem variable, MCNode node)
        {
            //if (variable.nodeType == NodeType.LIST)
            //{
            //    if (nodeValueInput != null)
            //    {
            //        nodeValueInput.gameObject.SetActive(false);
            //    }
                
            //    if (nodeValueDropdown != null)
            //    {
            //        nodeValueDropdown.gameObject.SetActive(false);
            //    }
                
            //    return;
            //}

            //DataType type = variable.dataType;
            //switch (type)
            //{
            //    case DataType.BOOL:
            //    case DataType.NUMBER:
            //    case DataType.STRING:
            //        if (nodeValueInput != null)
            //        {
            //            nodeValueInput.gameObject.SetActive(true);
            //        }

            //        if (nodeValueDropdown != null)
            //        {
            //            nodeValueDropdown.gameObject.SetActive(false);
            //        }
            //        break;

            //    case DataType.FACIAL:
            //    case DataType.MOTION:
            //    case DataType.MOBILITY:
            //        if (nodeValueInput != null)
            //        {
            //            nodeValueInput.gameObject.SetActive(false);
            //        }

            //        if (nodeValueDropdown != null)
            //        {
            //            nodeValueDropdown.gameObject.SetActive(true);
            //        }
            //        SetNodeValueDropdown(node);
            //        break;
            //}
        }

        private void SetNodeValueDropdown(MCNode node)
        {
            MCSetNode setNode = node as MCSetNode;
            nodeValueDropdown.ClearOptions();
            nodeValueDropdown.AddOptions(setNode.variableChangeDropdown.options);
        }

        private void SetExpressProperty(LeftMenuVariableItem variable)
        {
            PROJECT.Expression[] expArray = JsonConvert.DeserializeObject<PROJECT.Expression[]>(variable.value);
            currentVarCount = expArray.Length;
            SetBlocks(variable.nodeType, currentVarCount);

            for (int i = 0; i < expArray.Length; i++)
            {
                expressionBlocks[i].tts.text = expArray[i].tts;
                expressionBlocks[i].facial.text = Constants.GetValueForKorean(expArray[i].facial, PROJECT.DataType.FACIAL);
                expressionBlocks[i].motion.text = Constants.GetValueForKorean(expArray[i].motion, PROJECT.DataType.MOTION);
            }
        }

        private void SyncInputField(string text)
        {
            text = nodeValueInput.text;
            refNode.GetComponentInChildren<TMP_InputField>().text = text;
        }

        private void SyncDropdown(int value)
        {
            value = nodeValueDropdown.value;
            refNode.GetComponentsInChildren<TMP_Dropdown>()[1].value = value;
        }
    }
}