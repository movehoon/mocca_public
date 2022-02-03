using UnityEngine;
using TMPro;

namespace REEL.D2EEditor
{
    public class FullscreenSimulationView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField dialogInput;

        [SerializeField]
        private MCHelperPopup helperPopup;

        private void OnEnable()
        {
            dialogInput.text = string.Empty;

            if (helperPopup == null)
            {
                helperPopup = FindObjectOfType<MCHelperPopup>();
            }

            // 전체 화면 모드일 때 도움말 팝업 창 닫기.
            helperPopup.CloseButtonClicked();
        }

        private void OnDisable()
        {
            dialogInput.text = string.Empty;
        }
    }
}