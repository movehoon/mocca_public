using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class PlayButton : MonoBehaviour
    {
        private Image image;
        private Text text;

        [SerializeField] private Sprite playButtonImage;
        [SerializeField] private Sprite stopButtonImage;

        private readonly string executeString = "실행";
        private readonly string stopString = "정지";

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
            MCPlayStateManager.Instance.SubscribeSimulationStateChanged(OnSimulationStateChanged);
        }

        private void OnSimulationStateChanged(bool isSimulation)
        {
            image.sprite = isSimulation ? stopButtonImage : playButtonImage;
            if (text != null)
            {
                string str = isSimulation ? stopString : executeString;
                text.text = LocalizationManager.ConvertText(str, "top");
            }
        }
    }
}