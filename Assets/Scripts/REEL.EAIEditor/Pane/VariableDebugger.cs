using UnityEngine;

namespace REEL.D2EEditor
{
	public class VariableDebugger : MonoBehaviour
	{
        private LeftMenuVariableItem refVariable;
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

            if (refVariable == null)
            {
                refVariable = GetComponent<LeftMenuVariableItem>();
            }

            mouseOverChecker.SetShowTooltipFunction(ShowTooltip);
            mouseOverChecker.SetCloseTooltipFunction(CloseTooltip);
        }

        private void ShowTooltip()
        {
            MCTooltipManager.Instance.ShowTooltip(refVariable);
        }

        private void CloseTooltip()
        {
            MCTooltipManager.Instance.CloseTooltip();
        }
    }
}