using UnityEngine;

namespace REEL.D2EEditor
{
    public class PlayTextButton : ToolbarTextButtonBase
    {
        private readonly string executeString = "실행";
        private readonly string stopString = "정지";

        public override void OnSimulationStateChanged(bool isSimulation)
        {
            text.text = LocalizationManager.ConvertText(
                isSimulation ? stopString : executeString, 
                LocalizationGroupName
            );
        }
    }
}