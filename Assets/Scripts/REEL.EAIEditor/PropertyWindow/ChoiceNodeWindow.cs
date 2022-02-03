#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class ChoiceNodeWindow : OneInputFieldWindow
    {
#if USINGTMPPRO
        [SerializeField] private TMP_Text listName;
        [SerializeField] private TMP_Text inputString;
#else
        [SerializeField] private Text listName;
        [SerializeField] private Text inputString;
#endif

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            int connectedNodeID = node.GetNodeInputWithIndex(0).input.source;
            int connectedNodeID2 = node.GetNodeInputWithIndex(1).input.source;

            if (connectedNodeID != -1)
            {
                MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);
                if (connectedNode is MCGetNode)
                {
                    MCGetNode getNode = connectedNode as MCGetNode;
                    if (getNode.IsLocalVariable == true)
                    {
                        listName.text = MCWorkspaceManager.Instance.GetLocalVariable(getNode.CurrentVariableIndex).name;
                    }
                    else
                    {
                        listName.text = MCWorkspaceManager.Instance.GetVariable(getNode.CurrentVariableIndex).name;
                    }
                }
            }
            else
            {
                listName.text = string.Empty;
            }

            targetNodeInput = node.GetComponentInChildren<TMP_InputField>();
            if (targetNodeInput != null)
            {
                nodeValueInput.interactable = true;
                nodeValueInput.text = targetNodeInput.text;
                //nodeValueInput.onValueChanged.AddListener(SyncInputField);
                SyncTwoInputField();
            }
            else
            {
                MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID2);
                if (connectedNode is MCGetNode)
                {
                    nodeValueInput.interactable = false;
                    MCGetNode getNode = connectedNode as MCGetNode;
                    if (getNode.IsLocalVariable == true)
                    {
                        nodeValueInput.text = MCWorkspaceManager.Instance.GetLocalVariableValue(getNode.CurrentVariableIndex);
                    }
                    else
                    {
                        nodeValueInput.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
                    }
                }
                else
                {
                    onOffBlocks[0].SetActive(false);
                }
            }
        }
    }
}
