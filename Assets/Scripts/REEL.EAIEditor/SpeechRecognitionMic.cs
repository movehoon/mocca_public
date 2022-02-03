using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class SpeechRecognitionMic : MonoBehaviour
    {
        [Header("마이크 이미지")]
        public Sprite recStartImage;
        public Sprite recStopImage;

        private Image micButtonImage;

        private void OnEnable()
        {
            if (micButtonImage == null)
            {
                micButtonImage = GetComponent<Image>();
            }
        }

        public void OnStartSpeechRecognition()
        {
            if (Application.isPlaying == true && MCPlayStateManager.Instance.IsSimulation == false)
            {
                return;
            }

            Debug.Log("OnStartSpeechRecognition");
            SpeechRecognition.Instance.OnButtonRecognize();
            if (SpeechRecognition.Instance.IsRecognizing == true)
            {
                micButtonImage.sprite = recStartImage;
            }
            else
            {
                micButtonImage.sprite = recStopImage;
            }
        }
    }
}