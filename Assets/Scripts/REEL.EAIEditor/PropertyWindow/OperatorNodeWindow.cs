using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class OperatorNodeWindow : TwoInputFieldWindow
    {
        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            ClearInputFields();

            // 속성창 두 입력의 ContentType(ex: integer) 지정.
            SetInputFieldContentType(node.nodeData.type);

            int currentInputCount = node.GetComponentsInChildren<TMP_InputField>().Length;
            int connectedNodeID = node.GetNodeInputWithIndex(0).input.source;
            int connectedNodeID2 = node.GetNodeInputWithIndex(1).input.source;

            if (currentInputCount == 2) //하나도 연결 안되었을 때
            {
                NoInputConnected(node);
            }
            else if (currentInputCount == 1) //하나 연결 되었을 때 
            {
                if (connectedNodeID != -1) //첫번째가 연결 되었을 때 
                    ConnectedFirstInput(node, connectedNodeID);
                else //두번째가 연결 되었을 때 
                    ConnectedSecondInput(node, connectedNodeID2);
            }
            else if (currentInputCount == 0) //다 연결 되었을 때
            {
                MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);
                MCNode connectedNode2 = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID2);

                if (connectedNode is MCGetNode && connectedNode2 is MCGetNode)
                    AllInputConnected(connectedNode, connectedNode2);

                else if (connectedNode is MCGetNode || connectedNode2 is MCGetNode)
                {
                    if (connectedNode is MCGetNode)
                    {
                        MCGetNode getNode = connectedNode as MCGetNode;
                        nodeValueInput.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
                        SetInteratable(false, true);
                        onOffBlocks[1].SetActive(false);
                    }
                    else
                    {
                        MCGetNode getNode = connectedNode2 as MCGetNode;
                        nodeValueInput2.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
                        SetInteratable(true, false);
                        onOffBlocks[0].SetActive(false);
                    }
                }

                else
                    TurnOffBlocks();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            ResetContentType();
        }

        private void AllInputConnected(MCNode connectedNode, MCNode connectedNode2)
        {
            MCGetNode getNode = connectedNode as MCGetNode;
            MCGetNode getNode2 = connectedNode2 as MCGetNode;
            nodeValueInput.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
            nodeValueInput2.text = MCWorkspaceManager.Instance.GetVariableValue(getNode2.CurrentVariableIndex);
        }

        private void NoInputConnected(MCNode node)
        {
            //            SetInteratable(true, true);
            //#if USINGTMPPRO
            //            nodeValueInput.text = node.GetComponentsInChildren<TMP_InputField>()[0].text;
            //            nodeValueInput2.text = node.GetComponentsInChildren<TMP_InputField>()[1].text;
            //#else
            //            nodeValueInput.text = node.GetComponentsInChildren<InputField>()[0].text;
            //            nodeValueInput2.text = node.GetComponentsInChildren<InputField>()[1].text;
            //#endif
            //            nodeValueInput.onValueChanged.AddListener(SyncInputField);
            //            nodeValueInput2.onValueChanged.AddListener(SyncSecondInputField);
            SetInteratable(true, true);

            targetNodeInput = node.GetComponentsInChildren<TMP_InputField>()[0];
            targetNodeInput2 = node.GetComponentsInChildren<TMP_InputField>()[1];

            nodeValueInput.text = targetNodeInput.text;
            SyncTwoInputField();

            nodeValueInput2.text = targetNodeInput2.text;
            SyncTwoInputFieldSecond();
        }

        private void ConnectedSecondInput(MCNode node, int connectedNodeID2)
        {
            SetInteratable(true, false);

            targetNodeInput = node.GetComponentInChildren<TMP_InputField>();

            nodeValueInput.text = targetNodeInput.text;
            //nodeValueInput.onValueChanged.AddListener(SyncInputField);
            SyncTwoInputField();

            MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID2);
            if (connectedNode is MCGetNode)
            {
                MCGetNode getNode2 = connectedNode as MCGetNode;
                nodeValueInput2.text = MCWorkspaceManager.Instance.GetVariableValue(getNode2.CurrentVariableIndex);
            }
            else
                onOffBlocks[1].SetActive(false);
        }

        private void ConnectedFirstInput(MCNode node, int connectedNodeID)
        {
            SetInteratable(false, true);

            targetNodeInput2 = node.GetComponentInChildren<TMP_InputField>();

            nodeValueInput2.text = targetNodeInput2.text;
            nodeValueInput2.onValueChanged.AddListener((text) => { targetNodeInput2.text = text; });
            targetNodeInput2.onValueChanged.AddListener((text) => { nodeValueInput2.text = text; });

            //nodeValueInput2.onValueChanged.AddListener(SyncInputFieldForFirstLinked);

            MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);
            if (connectedNode is MCGetNode)
            {
                MCGetNode getNode = connectedNode as MCGetNode;
                nodeValueInput.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
            }
            else
                onOffBlocks[0].SetActive(false);
        }

        private void SetInputFieldContentType(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.ADD:
                case NodeType.SUB:
                case NodeType.MUL:
                case NodeType.DIV:
                case NodeType.MOD:
                case NodeType.POWER:
                case NodeType.RANDOM:
                    SetInputFieldContentType(TMP_InputField.ContentType.DecimalNumber);
                    break;

                case NodeType.AND:
                case NodeType.OR:
                    SetInputFieldContentType(TMP_InputField.ContentType.IntegerNumber);
                    break;

                case NodeType.EQUAL:
                case NodeType.NOT_EQUAL:
                    SetInputFieldContentType(TMP_InputField.ContentType.Standard);
                    break;

                case NodeType.LESS:
                case NodeType.LESS_EQUAL:
                case NodeType.GREATER:
                case NodeType.GREATER_EQUAL:
                    SetInputFieldContentType(TMP_InputField.ContentType.DecimalNumber);
                    break;

                default:
                    SetInputFieldContentType(TMP_InputField.ContentType.Standard);
                    break;
            }
        }

        private void ClearInputFields()
        {
            nodeValueInput.text = string.Empty;
            nodeValueInput2.text = string.Empty;
        }

        protected void SyncInputFieldForFirstLinked(string text)
        {
            text = nodeValueInput2.text;
            refNode.GetComponentInChildren<TMP_InputField>().text = text;
        }

        private void SetInteratable(bool first, bool second)
        {
            nodeValueInput.interactable = first;
            nodeValueInput2.interactable = second;
        }

        private void SetInputFieldContentType(TMP_InputField.ContentType contentType)
        {
            nodeValueInput.contentType = contentType;
            nodeValueInput2.contentType = contentType;
        }

        private void ResetContentType()
        {
            SetInputFieldContentType(TMP_InputField.ContentType.Standard);
        }
    }
}