using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    public class TeachableMachineNodeWindow : LongOneInputWindow
    {
        public Button updateModelButton;

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            updateModelButton.onClick.RemoveAllListeners();
            if (node is MCTeachableMachineNode == true)
            {
                updateModelButton.onClick.AddListener((node as MCTeachableMachineNode).OnPropertyWindowButtonClicked);
            }
        }

        protected override void SetTargetNodeInputField()
        {
            targetNodeInput = (refNode as MCTeachableMachineNode).urlInput;
        }
    }
}