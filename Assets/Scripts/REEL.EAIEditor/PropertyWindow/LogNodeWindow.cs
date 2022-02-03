#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using Newtonsoft.Json;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class LogNodeWindow : OneInputFieldWindow
    {
        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

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
                nodeValueInput.interactable = false;
                int connectedNodeID = node.GetNodeInputWithIndex(0).input.source;

                MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);
                if (connectedNode is MCGetNode)
                {
                    MCGetNode getNode = connectedNode as MCGetNode;
                    nodeValueInput.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
                }
                else
                {
                    nodeValueInput.text = string.Empty;
                }
            }
        }
    }
}