using DynamicPanels;

namespace Mocca
{
    public class MoccaPropertyPanel : MoccaPanelBase
    {
        public override void Initialize(Panel panel)
        {
            base.Initialize(panel);
            panel.name = "Property";
        }
    }
}