using DynamicPanels;

namespace Mocca
{
    public class MoccaLogPanel : MoccaPanelBase
    {
        public override void Initialize(Panel panel)
        {
            base.Initialize(panel);
            panel.name = "Log";
        }
    }
}