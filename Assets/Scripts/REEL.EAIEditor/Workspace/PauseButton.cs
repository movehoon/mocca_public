using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class PauseButton : MonoBehaviour
    {
        private Image image;
        private Text text;

        [SerializeField] private Sprite pauseButtonImage;
        [SerializeField] private Sprite resumeButtonImage;

        public GameObject[] targetGO;

        private readonly string pauseString = "Paused";
        private readonly string resumeString = "Resume";

        private void Awake()
        {
            if (image == null)
            {
                image = GetComponentInChildren<Image>();
            }

            if (text == null)
            {
                text = GetComponentInChildren<Text>();
            }

            //MCWorkspaceManager.Instance.SubscribeSimulationStateChanged(OnSimulationStateChanged);
            //MCWorkspaceManager.Instance.SubscribeOnPauseStateChanged(OnPauseStateChanged);
            MCPlayStateManager.Instance.SubscribeSimulationStateChanged(OnSimulationStateChanged);
            MCPlayStateManager.Instance.SubscribeOnPauseStateChanged(OnPauseStateChanged);
            OnPauseStateChanged(MCPlayStateManager.Instance.PauseState);
            OnSimulationStateChanged(MCPlayStateManager.Instance.IsSimulation);
        }

        private void OnEnable()
        {
            OnPauseStateChanged(MCPlayStateManager.Instance.PauseState);
        }

        private void OnPauseStateChanged(bool isPause)
        {
            image.sprite = isPause ? resumeButtonImage : pauseButtonImage;
            if (text != null)
            {
                text.text = isPause ?
                LocalizationManager.ConvertText(resumeString) :
                LocalizationManager.ConvertText(pauseString);
            }
        }

        private void OnSimulationStateChanged(bool isSimulation)
        {
            //gameObject.SetActive(isSimulation);
            foreach (GameObject go in targetGO)
            {
                go.SetActive(isSimulation);
            }
        }
    }
}