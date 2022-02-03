#define USINGTMPPRO

using UnityEngine;

#if USINGTMPPRO
using TMPro;
#else
#endif
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class LogWindow : Singleton<LogWindow>
    {
#if USINGTMPPRO
        [SerializeField] private TMP_Text logText;
#else
        [SerializeField] private Text logText;
#endif
        [SerializeField] private ScrollRect logScrollRect;

        private RectTransform logTextTransform;
        private bool shouldUpdate = false;
        private Vector2 originSize;

        private void Awake()
        {
            logTextTransform = logText.GetComponent<RectTransform>();
            originSize = logTextTransform.sizeDelta;
        }

        private void UpdateScrollbar()
        {
            Canvas.ForceUpdateCanvases();
            logScrollRect.verticalScrollbar.value = 0f;
        }

        public void PrintLogDebug(string message)
        {
            if (logText.text.Equals(string.Empty))
            {
                logText.text = GetFinalMessage("Debug", message);
            }
            else
            {
                logText.text = logText.text + "\n" + GetFinalMessage("Debug", message);
            }

            logTextTransform.sizeDelta = new Vector2(logTextTransform.sizeDelta.x, logText.preferredHeight);
            shouldUpdate = true;
        }

        public void PrintLog(string sender, string message)
        {
            if (logText.text.Equals(string.Empty))
            {
                logText.text = GetFinalMessage(sender, message);
            }
            else
            {
                logText.text = logText.text + "\n" + GetFinalMessage(sender, message);
            }

            logTextTransform.sizeDelta = new Vector2(logTextTransform.sizeDelta.x, logText.preferredHeight);
            shouldUpdate = true;
        }

        public void PrintWarning(string sender, string message)
        {
            if (logText.text.Equals(string.Empty))
            {
                logText.text = "<color=yellow>" + GetFinalMessage(sender, message) + "</color>";
            }
            else
            {
                logText.text = logText.text + "\n" + "<color=yellow>" + GetFinalMessage(sender, message) + "</color>";
            }

            logTextTransform.sizeDelta = new Vector2(logTextTransform.sizeDelta.x, logText.preferredHeight);
            shouldUpdate = true;
        }

        public void PrintError(string sender, string message)
        {
            if (logText.text.Equals(string.Empty))
            {
                logText.text = "<color=red>" + GetFinalMessage(sender, message) + "</color>";
            }
            else
            {
                logText.text = logText.text + "\n" + "<color=red>" + GetFinalMessage(sender, message) + "</color>";
            }

            logTextTransform.sizeDelta = new Vector2(logTextTransform.sizeDelta.x, logText.preferredHeight);
            shouldUpdate = true;
        }

        public void ClearAllMessages()
        {
            logText.text = string.Empty;
            logTextTransform.sizeDelta = originSize;
        }

        private void LateUpdate()
        {
            if (shouldUpdate)
            {
                UpdateScrollbar();
                shouldUpdate = false;
            }
        }

        private string GetFinalMessage(string sender, string message)
        {
            return GetSenderString(sender) + message;
        }

        private string GetSenderString(string sender)
        {
            return "[" + sender + "] ";
        }
    }
}