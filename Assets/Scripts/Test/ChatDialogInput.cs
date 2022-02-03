using UnityEngine;
using UnityEngine.UI;
using REEL.D2EEditor;
using System.Text;

namespace REEL.Test
{
    public class ChatDialogInput : InputField
    {
        private string senderString = "You";

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!text.Equals(string.Empty))
                {
                    byte[] bytes = Encoding.Default.GetBytes(text);
                    text = Encoding.UTF8.GetString(bytes);

                    ChatWindow.Instance.SendText(text, senderString);

                    text = string.Empty;

                    ActivateInputField();
                    Select();
                }
            }
        }
    }
}