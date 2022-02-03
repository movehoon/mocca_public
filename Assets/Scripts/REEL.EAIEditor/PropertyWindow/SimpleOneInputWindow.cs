using TMPro;

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class SimpleOneInputWindow : OneInputFieldWindow
    {
        protected InputValue inputValue = new InputValue();
        protected bool isInitialized = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputValue = new InputValue();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            nodeValueInput.onValueChanged.RemoveAllListeners();
            if (targetNodeInput != null)
            {
                targetNodeInput.onValueChanged.RemoveAllListeners();
            }

            inputValue.Clear();
            isInitialized = false;
        }

        protected override void UpdateProperty()
        {
            base.UpdateProperty();

            MCNodeInput input = refNode.GetNodeInputWithIndex(0);

            // 입력 소켓에 다른 노드가 연결된 경우.
            if (input.HasLine == true)
            {
                nodeValueInput.interactable = false;
                MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(input.input.source);

                if (connectedNode is MCGetNode)
                {
                    //Utils.Log($"before targetNodeInput.text: {targetNodeInput.text} / input.input.default_value: {input.input.default_value}");

                    // 기본 값 저장.
                    inputValue.defaultValue = input.input.default_value;

                    MCGetNode getNode = connectedNode as MCGetNode;
                    nodeValueInput.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);

                    //Utils.Log($"after targetNodeInput.text: {targetNodeInput.text} / input.input.default_value: {input.input.default_value}");

                    isInitialized = true;
                }
                else
                {
                    onOffBlocks[0].SetActive(false);
                }
            }

            if (input.HasLine == false)
            {
                onOffBlocks[0].SetActive(true);

                nodeValueInput.interactable = true;
                if (isInitialized == true)
                {
                    nodeValueInput.text = string.IsNullOrEmpty(inputValue.defaultValue) ? targetNodeInput.text : inputValue.defaultValue;
                }
                else
                {
                    nodeValueInput.text = targetNodeInput.text;
                    isInitialized = true;
                }

                SyncTwoInputField();
            }
        }

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            SetTargetNodeInputField();
            UpdateProperty();
        }

        protected virtual void SetTargetNodeInputField()
        {
            if (refNode.nodeData.type == PROJECT.NodeType.YESNO)
            {
                targetNodeInput = (refNode as MCYesNoNode).input;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.WHILE)
            {
                targetNodeInput = (refNode as MCWhileNode).input;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.IF)
            {
                targetNodeInput = (refNode as MCIFNode).input;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.DELAY)
            {
                targetNodeInput = (refNode as MCDelayNode).input;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.ABS)
            {
                targetNodeInput = (refNode as MCAbsNode).input1;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.ROUND)
            {
                targetNodeInput = (refNode as MCRoundNode).input1;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.ROUND_UP)
            {
                targetNodeInput = (refNode as MCCeilingNode).input1;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.ROUND_DOWN)
            {
                targetNodeInput = (refNode as MCFloorNode).input1;
            }
        }
    }
}