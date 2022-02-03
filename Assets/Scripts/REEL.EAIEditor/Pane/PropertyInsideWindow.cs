using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace REEL.D2EEditor
{
    public class PropertyInsideWindow : MonoBehaviour
    {
        public bool IsActive { get { return gameObject.activeSelf; } }

        private void OnEnable()
        {
            if (MCWorkspaceManager.Instance.CurrentSelectedBlockCount == 1)
            {
                MCNode node = MCWorkspaceManager.Instance.SelectedNodes[0];
                PropertyWindowManager.Instance.ShowProperty(node);
            }
        }

        private void OnDisable()
        {
            if (MCWorkspaceManager.Instance.CurrentSelectedBlockCount == 1)
            {

            }
        }
    }
}