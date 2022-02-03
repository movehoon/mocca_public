using UnityEngine;
using DynamicPanels;

namespace Mocca
{
    public class MoccaToolbarPanel : MoccaPanelBase
    {
        public override void Initialize(Panel panel)
        {
            base.Initialize(panel);

            panel.name = "Toolbar";
            panel.Header.gameObject.SetActive(false);
            panel.headerHeight = 0f;
            panel.ContentParent.sizeDelta = new Vector2(-4f, -4f);
            
            panel.ContentParent.offsetMin = new Vector2(panel.ContentParent.offsetMin.x, 2f);
            panel.ContentParent.offsetMax = new Vector2(panel.ContentParent.offsetMax.x, 2f);

            //Destroy(panel.ResizeZonesParent.gameObject);
        }
    }
}