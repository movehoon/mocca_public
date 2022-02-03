#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using REEL.PROJECT;
using System;

namespace REEL.D2EEditor
{
    [System.Serializable]
    public class InputValue
    {
        public bool hasLine = false;
        public string defaultValue = string.Empty;

        public void Clear()
        {
            hasLine = false;
            defaultValue = string.Empty;
        }
    }

    public class InsertNodeWindow : TwoInputFieldWindow
    {
#if USINGTMPPRO
        [SerializeField] private TMP_Dropdown dataDropdown;
        [SerializeField] private TMP_Dropdown targetNodeDropdown;
#else
        [SerializeField] private Dropdown dataDropdown;
        [SerializeField] private Dropdown targetNodeDropdown;
#endif

        private InputValue[] inputValues = new InputValue[3];

        protected override void OnEnable()
        {
            base.OnEnable();

            for (int ix = 0; ix < inputValues.Length; ++ix)
            {
                inputValues[ix] = new InputValue();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            dataDropdown.onValueChanged.RemoveAllListeners();
            if (targetNodeDropdown != null)
            {
                targetNodeDropdown.onValueChanged.RemoveAllListeners();
                targetNodeDropdown = null;
            }

            for (int ix = 0; ix < inputValues.Length; ++ix)
            {
                inputValues[ix].Clear();
            }
        }

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            // 블록의 InputField/Dropdown 참조 설정.
            SetTargetInputFieldAndDropdown(node);

            // hasLine 값 설정.
            for (int ix = 0; ix < inputValues.Length; ++ix)
            {
                inputValues[ix].hasLine = refNode.GetNodeInputWithIndex(ix).HasLine;
                inputValues[ix].defaultValue = refNode.GetNodeInputWithIndex(ix).input.default_value;
            }

            // 블록 - 프로퍼티의 입력 위젯 동기화.
            SyncTwoInputField();
            SyncTwoInputFieldSecond();
            SyncTwoDropdown();

            UpdateProperty();
        }

        private void SetTargetInputFieldAndDropdown(MCNode node)
        {
            if (node is MCInsertNode)
            {
                MCInsertNode insertNode = node as MCInsertNode;
                targetNodeInput = insertNode.indexInput;
                targetNodeInput2 = insertNode.dataInput;
                targetNodeDropdown = insertNode.dataDropdown;
            }
            else if (node is MCSetElementNode)
            {
                MCSetElementNode setElemNode = node as MCSetElementNode;
                targetNodeInput = setElemNode.indexInput;
                targetNodeInput2 = setElemNode.dataInput;
                targetNodeDropdown = setElemNode.dataDropdown;
            }
        }

        #region ShowProperty backup
        //        public override void ShowProperty(MCNode node)
        //        {
        //            base.ShowProperty(node);

        //            TurnOnOffInputFieldDropdown(nodeValueInput2.gameObject, true, dataDropdown.gameObject, false);

        //            int connectedNodeID = node.GetNodeInputWithIndex(0).input.source;
        //            int connectedNodeID2 = node.GetNodeInputWithIndex(1).input.source;
        //            int connectedNodeID3 = node.GetNodeInputWithIndex(2).input.source;

        //            if (node is MCInsertNode)
        //            {
        //                MCInsertNode insertNode = node as MCInsertNode;
        //                targetNodeInput = insertNode.indexInput;
        //                targetNodeInput2 = insertNode.dataInput;
        //                targetNodeDropdown = insertNode.dataDropdown;
        //            }
        //            else if (node is MCSetElementNode)
        //            {
        //                MCSetElementNode setElemNode = node as MCSetElementNode;
        //                targetNodeInput = setElemNode.indexInput;
        //                targetNodeInput2 = setElemNode.dataInput;
        //                targetNodeDropdown = setElemNode.dataDropdown;
        //            }

        //            // 블록 - 프로퍼티의 입력 위젯 동기화.
        //            SyncTwoInputField();
        //            SyncTwoInputFieldSecond();
        //            SyncTwoDropdown();

        //            if (connectedNodeID2 == -1) //index input X
        //            {
        //                nodeValueInput.interactable = true;

        ////#if USINGTMPPRO
        ////                targetNodeInput = node.GetComponentsInChildren<TMP_InputField>()[0];
        ////#else
        ////                targetNodeInput = node.GetComponentsInChildren<InputField>()[0];
        ////#endif
        //                nodeValueInput.text = targetNodeInput.text;
        //                //nodeValueInput.onValueChanged.AddListener(SyncInputField);
        //                //SyncTwoInputField();
        //            }
        //            else //index input O
        //            {
        //                MCNode connectedIndexNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID2);
        //                if (connectedIndexNode is MCGetNode)
        //                {
        //                    nodeValueInput.interactable = false;
        //                    MCGetNode getNode = connectedIndexNode as MCGetNode;
        //                    nodeValueInput.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
        //                }
        //                else
        //                {
        //                    onOffBlocks[0].SetActive(false);
        //                }
        //            }

        //            MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);
        //            if (connectedNodeID3 == -1 && connectedNode != null) //data input X && ListDisconnect
        //            {
        //                CheckInputNode(connectedNode);

        //                if (nodeValueInput2.gameObject.activeSelf) // inputfield type 변수일때
        //                {
        ////#if USINGTMPPRO
        ////                    TMP_InputField valueInput2;
        ////#else
        ////                    InputField valueInput2;
        ////#endif
        ////                    if (connectedNodeID2 == -1)
        ////                    {
        ////#if USINGTMPPRO
        ////                        targetNodeInput2 = node.GetComponentsInChildren<TMP_InputField>()[1];
        ////#else
        ////                        targetNodeInput2 = node.GetComponentsInChildren<InputField>()[1];
        ////#endif
        ////                    }

        ////                    else
        ////                    {
        ////#if USINGTMPPRO
        ////                        targetNodeInput2 = node.GetComponentsInChildren<TMP_InputField>()[0];
        ////#else
        ////                        targetNodeInput2 = node.GetComponentsInChildren<InputField>()[0];
        ////#endif
        ////                    }

        //                    nodeValueInput2.interactable = true;
        //                    nodeValueInput2.text = targetNodeInput2.text;
        //                    //nodeValueInput2.onValueChanged.AddListener(SyncSecondInputField);
        //                    //SyncTwoInputFieldSecond();
        //                }
        //                else if (dataDropdown.gameObject.activeSelf) // dropdown type 변수일때
        //                {
        //                    dataDropdown.interactable = true;
        //                    SetNodeValueDropdown(node);
        //#if USINGTMPPRO
        //                    //targetNodeDropdown = insertNode.dataDropdown;
        //                    //dataDropdown.value = targetNodeDropdown.value;
        //                    //targetNodeDropdown = node.GetComponentInChildren<TMP_Dropdown>();
        //                    //dataDropdown.value = targetNodeDropdown.value;
        //#else
        //                    targetNodeDropdown = node.GetComponentInChildren<Dropdown>();
        //                    dataDropdown.value = targetNodeDropdown.value;
        //#endif
        //                    //dataDropdown.onValueChanged.AddListener(SyncDropdown);
        //                    //SyncTwoDropdown();
        //                }
        //            }
        //            else if (connectedNodeID3 != -1) //data input O && ListDisconnect
        //            {
        //                CheckInputNode(connectedNode);

        //                MCNode connectedDataNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID3);
        //                if (connectedDataNode is MCGetNode)
        //                {
        //                    MCGetNode getNode = connectedDataNode as MCGetNode;
        //                    LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
        //                    if (nodeValueInput2.gameObject.activeSelf) // inputfield type 변수일때
        //                    {
        //                        nodeValueInput2.interactable = false;
        //                        nodeValueInput2.text = Constants.GetValueForKorean(variable.value, variable.dataType);
        //                    }
        //                    else if (dataDropdown.gameObject.activeSelf) // dropdown type 변수일때
        //                    {
        //                        dataDropdown.interactable = false;
        //                        dataDropdown.captionText.text = Constants.GetValueForKorean(variable.value, variable.dataType);
        //                    }
        //                }
        //                else
        //                {
        //                    onOffBlocks[1].SetActive(false);
        //                }
        //            }
        //        }
        #endregion

        protected override void UpdateProperty()
        {
            base.UpdateProperty();

            TurnOnOffInputFieldDropdown(nodeValueInput2.gameObject, true, dataDropdown.gameObject, false);

            // Todo.
            // 여기 고쳐야함.
            // 선 연결 안된 상태에서 Get노드 연결한 다음, 다시 끊으면 변수에 있던 값이 InputField 값을 덮어씀.
            // -> 변수 값이 아니라 InputField에 원래 있던 값을 불러오도록 로직 변경해야 함.

            MCNodeInput input1 = refNode.GetNodeInputWithIndex(0);                 // List Input Socket (No InputField or Dropdown).
            MCNodeInput input2 = refNode.GetNodeInputWithIndex(1);                 // Index Input Socket (Only InputField).
            MCNodeInput input3 = refNode.GetNodeInputWithIndex(2);                 // Data Input Socket (InputField / Dropdown).

            // 인덱스 입력에 라인 연결 됐을 때.
            if (input2.HasLine == true)
            {
                int nodeID = input2.input.source;
                MCNode connectedIndexNode = MCWorkspaceManager.Instance.FindNodeWithID(nodeID);
                if (connectedIndexNode is MCGetNode)
                {
                    // 기본 값 저장.
                    inputValues[1].defaultValue = targetNodeInput.text;

                    nodeValueInput.interactable = false;
                    MCGetNode getNode = connectedIndexNode as MCGetNode;
                    nodeValueInput.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
                }
                else
                {
                    onOffBlocks[0].SetActive(false);
                }
            }
            else  // 인덱스 입력에 라인 연결 안됐을 때.
            {
                nodeValueInput.interactable = true;
                //nodeValueInput.text = targetNodeInput.text;
                nodeValueInput.text = inputValues[1].defaultValue;
            }

            // 리스트는 연결됐는데,
            if (input1.HasLine == true)
            {
                onOffBlocks[1].SetActive(true);

                MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(input1.input.source);
                CheckInputNode(connectedNode);

                // 데이터 입력 연결 됐을 때.
                if (input3.HasLine == true)
                {
                    MCNode connectedDataNode = MCWorkspaceManager.Instance.FindNodeWithID(input3.input.source);
                    if (connectedDataNode is MCGetNode)
                    {
                        MCGetNode getNode = connectedDataNode as MCGetNode;
                        LeftMenuVariableItem variable;
                        if (getNode.IsLocalVariable == true)
                        {
                            variable = MCWorkspaceManager.Instance.GetLocalVariable(getNode.CurrentVariableIndex);
                        }
                        else
                        {
                            variable = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex);
                        }
                        
                        if (nodeValueInput2.gameObject.activeSelf)
                        {
                            // 기본 값 저장.
                            inputValues[2].defaultValue = targetNodeInput2.text;

                            nodeValueInput2.interactable = false;
                            nodeValueInput2.text = Constants.GetValueForKorean(variable.value, variable.dataType);
                        }
                        else if (dataDropdown.gameObject.activeSelf) // dropdown type 변수일때
                        {
                            // 기본 값 저장.
                            inputValues[2].defaultValue = targetNodeDropdown.value.ToString();

                            dataDropdown.interactable = false;
                            dataDropdown.captionText.text = Constants.GetValueForKorean(variable.value, variable.dataType);
                        }
                    }
                    else
                    {
                        onOffBlocks[1].SetActive(false);
                    }
                }

                // 데이터 입력 연결 안됐을 때.
                if (input3.HasLine == false)
                {
                    if (nodeValueInput2.gameObject.activeSelf)
                    {
                        nodeValueInput2.interactable = true;
                        //nodeValueInput2.text = targetNodeInput2.text;
                        nodeValueInput2.text = inputValues[2].defaultValue;
                    }
                    else if (dataDropdown.gameObject.activeSelf)
                    {
                        dataDropdown.interactable = true;
                        SetNodeValueDropdown(refNode);
                    }
                }
            }

            // 리스트 입력 연결 안됐을 때.
            if (input1.HasLine == false)
            {
                onOffBlocks[1].SetActive(false);
            }
        }

        private void CheckInputNode(MCNode node)
        {
            if (node is MCGetNode) //List가 input으로 들어올 수 있는 경우는 get 노드 밖에 없다.
            {
                MCGetNode getnode = node as MCGetNode;
                LeftMenuVariableItem currentVariable;
                if (getnode.IsLocalVariable == true)
                {
                    currentVariable = MCWorkspaceManager.Instance.GetLocalVariable(getnode.CurrentVariableIndex);
                }
                else
                {
                    currentVariable = MCWorkspaceManager.Instance.GetVariable(getnode.CurrentVariableIndex);
                }
                
                CheckVariableType(currentVariable);
            }
#region //set일 경우
            //else if (node is MCSetNode)
            //{
            //    MCSetNode setnode = node as MCSetNode;
            //    LeftMenuVariableItem currentVariable = MCWorkspaceManager.Instance.GetVariable(setnode.CurrentVariableIndex);
            //    CheckVariableType(currentVariable);
            //}
#endregion
        }

        private void CheckVariableType(LeftMenuVariableItem variable)
        {
            DataType type = variable.dataType;
            if (type == DataType.BOOL || type == DataType.NUMBER || type == DataType.STRING)
            {
                TurnOnOffInputFieldDropdown(nodeValueInput2.gameObject, true, dataDropdown.gameObject, false);
            }
            else if (type == DataType.FACIAL || type == DataType.MOTION || type == DataType.MOBILITY)
            {
                TurnOnOffInputFieldDropdown(nodeValueInput2.gameObject, false, dataDropdown.gameObject, true);
            }
        }

        private void SetNodeValueDropdown(MCNode node)
        {
            dataDropdown.ClearOptions();
            if (node is MCInsertNode)
            {
                MCInsertNode insertNode = node as MCInsertNode;
                dataDropdown.AddOptions(insertNode.dataDropdown.options);
                dataDropdown.value = insertNode.dataDropdown.value;
            }
            else if (node is MCSetElementNode)
            {
                MCSetElementNode setElemNode = node as MCSetElementNode;
                dataDropdown.AddOptions(setElemNode.dataDropdown.options);
                dataDropdown.value = setElemNode.dataDropdown.value;
            }
        }

        private void SyncDropdown(int value)
        {
            value = dataDropdown.value;
            if (refNode != null)
            {
#if USINGTMPPRO
                refNode.GetComponentInChildren<TMP_Dropdown>().value = value;
#else
                normalNode.GetComponentInChildren<Dropdown>().value = value;
#endif
            }
        }

        private void SyncTwoDropdown()
        {
            dataDropdown.value = targetNodeDropdown.value;
            dataDropdown.onValueChanged.AddListener((index) => { targetNodeDropdown.value = index; });
            targetNodeDropdown.onValueChanged.AddListener((index) => { dataDropdown.value = index; });
        }
    }
}