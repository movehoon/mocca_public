using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class MCNodeTooltip : MonoBehaviour
    {
        private MouseOverChecker mouseOverChecker;

        private void OnEnable()
        {
            if (mouseOverChecker == null)
            {
                mouseOverChecker = GetComponent<MouseOverChecker>();
                if (mouseOverChecker == null)
                {
                    mouseOverChecker = gameObject.AddComponent<MouseOverChecker>();
                }
            }

            mouseOverChecker.SetShowTooltipFunction(ShowTooltip);
            mouseOverChecker.SetCloseTooltipFunction(CloseTooltip);
        }

        private void Update()
        {
            if (mouseOverChecker.IsHover)
            {
                var node = GetComponent<MCNode>();

                if (node != null)
                {
                    node.HighlightLine();
                }
            }
        }

        public void ShowTooltip()
        {
            if (GetComponent<MCNode>() != null)
            {
                MCTooltipManager.Instance.ShowTooltip(GetComponent<MCNode>());
            }

            else if (GetComponent<NodeListItemComponent>() != null)
            {
                MCTooltipManager.Instance.ShowTooltip(GetComponent<NodeListItemComponent>());
            }
        }

        public void CloseTooltip()
        {
            //Debug.Log("Close Tooptip");
            MCTooltipManager.Instance.CloseTooltip();
        }
    }
}