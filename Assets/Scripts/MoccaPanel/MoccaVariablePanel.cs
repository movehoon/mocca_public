using DynamicPanels;
using REEL.D2EEditor;

namespace Mocca
{
    public class MoccaVariablePanel : MoccaPanelBase
    {
        public override void Initialize(Panel panel)
        {
            base.Initialize(panel);
            panel.name = "Variable";
        }

        public void AddVariable()
        {
            //Utils.LogRed("AddVariable");

            if (IsProjectNullOrOnSimulation)
            {
                //Utils.LogRed("AddVariable IsProjectNullOrOnSimulation");
                return;
            }

            MCVariableListPopup popup = MCEditorManager.Instance
                .GetPopup(MCEditorManager.PopupType.VariableList)
                .GetComponent<MCVariableListPopup>();

            popup.SetVariableItem(null);
            popup.ShowPopup();
        }

        private bool IsProjectNullOrOnSimulation
        {
            //get { return MCWorkspaceManager.IsProjectNull || MCWorkspaceManager.Instance.IsSimulation; }
            get { return MCWorkspaceManager.IsProjectNull || MCPlayStateManager.Instance.IsSimulation; }
        }
    }
}