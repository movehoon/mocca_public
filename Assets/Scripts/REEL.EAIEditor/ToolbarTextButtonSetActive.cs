using UnityEngine;
using UnityEngine.UI;

// 시나리오 실행 중에는 툴바 버튼 안눌리게 막아주는 스크립트.
namespace REEL.D2EEditor
{
    public class ToolbarTextButtonSetActive : MonoBehaviour
    {
        Button button;

        private void OnEnable()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            MCPlayStateManager.Instance.SubscribeSimulationStateChanged(OnSimulationStateChanged);
        }

        private void OnSimulationStateChanged(bool isSimulation)
        {
            if (button != null)
            {
                button.interactable = !isSimulation;
            }
        }
    }
}