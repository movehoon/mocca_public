using TMPro;

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class LongOneInputWindow : OneInputFieldWindow
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
            if (refNode.nodeData.type == PROJECT.NodeType.SAY)
            {
                targetNodeInput = (refNode as MCSayNode).input;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.LOG)
            {
                targetNodeInput = (refNode as MCLogNode).input;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.CHAT)
            {
                targetNodeInput = (refNode as MCChatbotNode).input;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.USER_API)
            {
                targetNodeInput = (refNode as MCUserAPINode).apiInput;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.USER_API_CAMERA)
            {
                targetNodeInput = (refNode as MCUserAPICameraNode).apiInput;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.DELETE_FACE)
            {
                targetNodeInput = (refNode as MCDeleteFaceNode).nameInput;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.TEACHABLE_MACHINE_SERVER)
            {
                targetNodeInput = (refNode as MCTeachableMachineNode).urlInput;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.DETECT_OBJECT)
            {
                targetNodeInput = (refNode as MCDetectObjectNode).inputField;
            }

            else if (refNode.nodeData.type == PROJECT.NodeType.WIKI_QNA)
            {
                targetNodeInput = (refNode as MCBrainyNode).input;
            }
        }
    }
}