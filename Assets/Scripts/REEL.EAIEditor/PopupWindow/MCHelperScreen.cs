#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using System;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class MCHelperScreen : MonoBehaviour
	{
#if USINGTMPPRO
        public TMP_Text titleText;
        public TMP_Text helperText;
#else
        public Text titleText;
        public Text helperText;
#endif
        public GifAnimationPlayer gifPlayer;
        public MCHelperPopup helperPopup;

        public void SetHelperFrameInfo(HelperFrameInfo frameInfo, string title)
        {
            titleText.text = title;
            this.helperText.text = frameInfo.helperText;

            gifPlayer.sprites = new Sprite[frameInfo.sprites.Length];
            for (int ix = 0; ix < frameInfo.sprites.Length; ++ix)
            {
                gifPlayer.sprites[ix] = frameInfo.sprites[ix];
            }

            PlayGIF();
        }

        private void PlayGIF()
        {
            gifPlayer.gameObject.SetActive(true);
        }

        public void OnBackButtonClicked()
        {
            helperPopup.ResetScrollBar();
            gameObject.SetActive(false);
        }

        public void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
            helperPopup.gameObject.SetActive(false);
        }
	}
}