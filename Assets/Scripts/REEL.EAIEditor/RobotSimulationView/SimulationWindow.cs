using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class SimulationWindow : MonoBehaviour
    {
        [SerializeField] private GameObject robotViewRT;
        [SerializeField] private GameObject fullScreenSimulationWindow;
        [SerializeField] private Camera robotCamera;

        [Header("카메라 사용/중지 토글")]
        public Toggle useCameraToggle = null;
        public TMPro.TextMeshProUGUI useCameraToggleText = null;

        private readonly string useCameraString = "[ID_USE_CAMERA]";
        private readonly string stopCameraString = "[ID_STOP_CAMERA]";

        private bool isFullScreen = false;
        public bool IsFullScreen { get { return isFullScreen; } }

        private void Awake()
        {
            SetRenderTexture();

            fullScreenSimulationWindow.SetActive(true);
            fullScreenSimulationWindow.SetActive(false);
        }

        private void Update()
        {
            if (MessageBox.IsActive == false)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (isFullScreen)
                    {
                        SetFullScreenMode(false);
                    }
                }
            }
        }

        public void SetFullScreenMode(bool isFullScreen)
        {
            this.isFullScreen = isFullScreen;
            SetRenderTexture();
        }

        private void SetRenderTexture()
        {
            fullScreenSimulationWindow.SetActive(isFullScreen);
            robotViewRT.SetActive(!isFullScreen);
        }
        
        public void OnUseCameraChanged(bool isOn)
        {
            string finalString = isOn ? useCameraString : stopCameraString;
            bool isKorean = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR;
            //finalString = isKorean == true ? 
            //    LocalizationManager.GetLocalText(finalString).kor : LocalizationManager.GetLocalText(finalString).eng;
            finalString = LocalizationManager.ConvertText(finalString);

            useCameraToggleText.text = finalString;

            if (isOn == true)
            {
                Webcam.Instance.SwitchWebcam();
            }
            else
            {
                Webcam.Instance.StopWebcam();
            }
        }
    }
}