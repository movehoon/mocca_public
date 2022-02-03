#define USINGTMPPRO

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USINGTMPPRO
using TMPro;
#endif

using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class RandomNodeWindow : OneInputFieldWindow
    {
        public override void ShowProperty(MCNode node)
        {
            gameObject.SetActive(true);

            refNode = node;

            nodeTypeText.text = node.nodeData.type.ToString();
            //nodeIDText.text = node.NodeID.ToString();

            // 블록의 상태(선연결/해제 등)가 변경될 때 발생하는 이벤트에 등록.
            refNode.SubscribeOnNodeStateChanged(OnNodeStateChanged);

#if USINGTMPPRO
            targetNodeInput = node.GetComponentInChildren<TMP_InputField>();
            nodeValueInput.text = targetNodeInput.text;
#else
            targetNodeInput = node.GetComponentInChildren<InputField>();
            nodeValueInput.text = targetNodeInput.text;
#endif

            //nodeValueInput.onValueChanged.AddListener(SyncInputField);
            SyncTwoInputField();
        }
    }
}

