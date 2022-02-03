using UnityEngine;
using UnityEngine.UI;

namespace REEL.Test
{
    public class ChatWindow : Singleton<ChatWindow>
    {
        [SerializeField] private Text chatText;
        private ScrollRect scrollRect;

        private int linesOfMessage = 0;
        private int threshouldOfLines = 9;
        private float lineHeight = 25f;

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            lineHeight = chatText.fontSize;
        }

        void UpdateScrollbar()
        {
            scrollRect.verticalScrollbar.value = 0f;
        }

        public void SendText(string message, string sender)
        {
            if (chatText.text.Equals(string.Empty))
            {
                chatText.text = GetFinalMessage(message, sender);
            }
            else
            {
                chatText.text = chatText.text + "\n" + GetFinalMessage(message, sender);
            }

            ++linesOfMessage;

            if (linesOfMessage >= threshouldOfLines)
            {
                RectTransform rectTransform = chatText.GetComponent<RectTransform>();
                rectTransform.sizeDelta += new Vector2(0f, lineHeight);

                UpdateScrollbar();
            }
        }

        private string GetFinalMessage(string message, string sender)
        {
            return GetSenderString(sender) + message;
        }

        private string GetSenderString(string sender)
        {
            return "[" + sender + "] ";
        }
    }
}