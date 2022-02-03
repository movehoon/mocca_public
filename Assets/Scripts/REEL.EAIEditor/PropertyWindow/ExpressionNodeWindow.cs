#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class ExpressionNodeWindow : MCNodeWindow
    {
#if USINGTMPPRO
        public TMP_Text expressionCountText;
#else
        public Text expressionCountText;
#endif

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);
            
            if (node.GetNodeInputWithIndex(0).input.source != -1)
            {
                int connectedNodeID = node.GetNodeInputWithIndex(0).input.source;
                MCGetNode getNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID) as MCGetNode;
                if (getNode != null)
                {
                    string jsonString = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
                    expressionCountText.text = JsonConvert.DeserializeObject<PROJECT.Expression[]>(jsonString).Length.ToString();
                }
            }
        }
    }
}