using UnityEngine;
using DynamicPanels;

namespace Mocca
{
    public class MoccaPanelBase : MonoBehaviour, IMoccaPanel
    {
        protected Panel panel;
        protected DynamicPanelsCanvas canvas;
        protected RectTransform refRT;

        private void OnEnable()
        {
            if (canvas == null)
            {
                canvas = FindObjectOfType<DynamicPanelsCanvas>();
            }

            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
            }
        }

        public virtual void Initialize(Panel panel)
        {
            this.panel = panel;
            panel.name = "Panel";
            panel.Header.gameObject.AddComponent<MoccaPanelHeaderDrag>();
        }
    }
}