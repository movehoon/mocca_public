using UnityEngine;
using TMPro;

namespace REEL.D2EEditor
{
    public class MCWebcamProcessMessageWindow : MonoBehaviour
    {
        public TMP_Text messageText;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            messageText.text = string.Empty;
        }

        public void SetMessage(string message)
        {
            if (gameObject.activeSelf == false)
            {
                Show();
            }

            messageText.text = message;
            LocalizationManager.Localize(messageText);
        }
    }
}