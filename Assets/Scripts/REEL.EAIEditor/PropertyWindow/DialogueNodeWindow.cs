using TMPro;

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class DialogueNodeWindow : OneInputFieldWindow
    {
        public TMP_InputField rivescriptInput;
        
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

            rivescriptInput.text = string.Empty;
        }

        public void OnSubmitRivescriptInput(string value)
        {
            if (refNode is MCDialogueNode)
            {
                MCDialogueNode dialogueNode = refNode as MCDialogueNode;
                dialogueNode?.SetBodyValue(value);
            }
            
            else if (refNode is MCChatbotPizzaOrderNode)
            {
                MCChatbotPizzaOrderNode pizzaOrderNode = refNode as MCChatbotPizzaOrderNode;
                pizzaOrderNode?.SetBodyValue(value);
            }
        }

        protected override void UpdateProperty()
        {
            base.UpdateProperty();

            // Rivescript 스크립트 텍스트 설정.
            if (refNode is MCDialogueNode)
            {
                MCDialogueNode dialogueNode = refNode as MCDialogueNode;
                if (dialogueNode != null && dialogueNode.nodeData.body != null)
                {
                    rivescriptInput.text = dialogueNode.nodeData.body.value;
                }
            }
            else if (refNode is MCChatbotPizzaOrderNode)
            {
                MCChatbotPizzaOrderNode pizzaOrderNode = refNode as MCChatbotPizzaOrderNode;
                if (pizzaOrderNode != null && pizzaOrderNode.nodeData.body != null)
                {
                    rivescriptInput.text = pizzaOrderNode.nodeData.body.value;
                }
            }

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

            // 입력 소켓에 다른 노드가 연결되지 않은 경우.
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
            if (refNode is MCDialogueNode)
            {
                targetNodeInput = (refNode as MCDialogueNode).input;
            }
            else if (refNode is MCChatbotPizzaOrderNode)
            {
                targetNodeInput = (refNode as MCChatbotPizzaOrderNode).input;
            }
        }
    }
}