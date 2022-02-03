using UnityEngine;
using TMPro;
using System.Text;

namespace REEL.D2EEditor
{
    public class DialogTMPInputField : TMP_InputField
    {
        private readonly string logNickname = "YOU";

        private void Update()
        {
            if (Application.isPlaying && !MCPlayStateManager.Instance.IsSimulation)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SubmitDialog();
            }
        }

        public void SubmitDialog()
        {
            if (!text.Equals(string.Empty))
            {
                byte[] bytes = Encoding.Default.GetBytes(text);
                text = Encoding.UTF8.GetString(bytes);

                WebCommunicationManager.Instance.SendSTT(text);
                LogWindow.Instance.PrintLog(logNickname, text);

                text = string.Empty;
            }
        }

        public void OnStartSpeechRecognition()
        {
            Debug.Log("OnStartSpeechRecognition");
            SpeechRecognition.Instance.OnButtonRecognize();
        }
    }
}
