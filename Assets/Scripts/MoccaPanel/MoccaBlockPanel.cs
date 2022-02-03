using DynamicPanels;

namespace Mocca
{
    public class MoccaBlockPanel : MoccaPanelBase
    {
        public override void Initialize(Panel panel)
        {
            base.Initialize(panel);
            panel.name = "Block and Variable";
        }
    }
}