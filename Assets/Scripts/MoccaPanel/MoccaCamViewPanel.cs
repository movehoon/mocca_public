using DynamicPanels;

namespace Mocca
{
    public class MoccaCamViewPanel : MoccaPanelBase
    {
        public override void Initialize(Panel panel)
        {
            base.Initialize(panel);
            panel.name = "Cam";
        }
    }
}