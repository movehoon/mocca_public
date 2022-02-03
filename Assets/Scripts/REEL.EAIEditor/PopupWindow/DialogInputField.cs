using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class DialogInputField : InputField
    {
        private string logNickName = "YOU";

        private void Update()
        {
            if (Application.isPlaying && !MCPlayStateManager.Instance.IsSimulation)
            {
                return;
            }   

            if (Input.GetKeyDown(KeyCode.Return) || 
                Input.GetKeyDown(KeyCode.KeypadEnter))
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
                LogWindow.Instance.PrintLog(logNickName, text);

                text = string.Empty;

                //ActivateInputField();
                //Select();
            }
        }
    }
}