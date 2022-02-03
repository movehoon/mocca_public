using UnityEngine;

namespace REEL.D2EEditor
{
    public class PauseTextButton : ToolbarTextButtonBase
    {
        private readonly string pauseString = "일시정지";
        private readonly string resumeString = "다시실행";

        protected override void Awake()
        {
            base.Awake();
            
            MCPlayStateManager.Instance.SubscribeOnPauseStateChanged(OnPauseStateChanged);
            OnPauseStateChanged(MCPlayStateManager.Instance.PauseState);
        }

        private void OnPauseStateChanged(bool isPause)
        {
            text.text = isPause ? 
                LocalizationManager.ConvertText(resumeString, LocalizationGroupName) : 
                LocalizationManager.ConvertText(pauseString, LocalizationGroupName);
        }

        public override void OnSimulationStateChanged(bool isSimulation)
        {
            gameObject.SetActive(isSimulation);
        }
    }
}