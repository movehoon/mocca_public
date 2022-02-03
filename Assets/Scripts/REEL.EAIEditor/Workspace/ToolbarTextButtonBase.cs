using UnityEngine;

namespace REEL.D2EEditor
{
    public abstract class ToolbarTextButtonBase : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI text;

        protected readonly string LocalizationGroupName = "top";

        protected virtual void Awake()
        {
            if (text == null)
            {
                text = GetComponent<TMPro.TextMeshProUGUI>();
            }

            MCPlayStateManager.Instance.SubscribeSimulationStateChanged(OnSimulationStateChanged);
            OnSimulationStateChanged(MCPlayStateManager.Instance.IsSimulation);
        }

        public abstract void OnSimulationStateChanged(bool isSimulation);
    }
}