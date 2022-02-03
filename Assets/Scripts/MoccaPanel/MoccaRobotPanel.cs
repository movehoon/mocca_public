using DynamicPanels;

namespace Mocca
{
    public class MoccaRobotPanel : MoccaPanelBase
    {
        public override void Initialize(Panel panel)
        {
            base.Initialize(panel);
            panel.name = "Robot";
        }
    }
}