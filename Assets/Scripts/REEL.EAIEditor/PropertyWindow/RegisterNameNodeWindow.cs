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
    public class RegisterNameNodeWindow : OneInputFieldWindow
    {
        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            int connectedNodeID = node.GetNodeInputWithIndex(0).input.source;

#if USINGTMPPRO
            targetNodeInput = node.GetComponentInChildren<TMP_InputField>();
#else
            targetNodeInput = node.GetComponentInChildren<InputField>();
#endif
            if (targetNodeInput != null)
            {
                nodeValueInput.interactable = true;
                nodeValueInput.text = targetNodeInput.text;
                //nodeValueInput.onValueChanged.AddListener(SyncInputField);
                SyncTwoInputField();
            }
            else
            {
                MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);
                if (connectedNode is MCGetNode)
                {
                    nodeValueInput.interactable = false;
                    MCGetNode getNode = connectedNode as MCGetNode;
                    nodeValueInput.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
                }
                else
                {
                    onOffBlocks[0].SetActive(false);
                }
            }
        }
    }
}
