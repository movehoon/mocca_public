#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    [System.Serializable]
    public class HelperFrameInfo
    {
        public Sprite[] sprites;
        public string helperText;
    }

	public class MCHelperPopup : MonoBehaviour
	{
        public HelperFrameInfo[] frameInfos;
        public GameObject[] items;
        public MCHelperScreen helperScreen;
        public GameObject shortcutMenu;
        public ScrollRect scrollRect;

        private Dictionary<int, HelperFrameInfo> helperMenuDictionary = new Dictionary<int, HelperFrameInfo>();

        private void OnEnable()
        {
            if (helperMenuDictionary.Count == 0)
            {
                InitDictionary();
            }

            ResetScrollBar();
            MCWorkspaceManager.Instance.SubscribeOnToolbarButtonClicked(CloseButtonClicked);
        }

        private void OnDisable()
        {
            MCWorkspaceManager.Instance.UnSubscribeOnToolbarButtonClicked(CloseButtonClicked);
        }

        private void InitDictionary()
        {
            for (int ix = 0; ix < items.Length; ++ix)
            {
                helperMenuDictionary.Add(items[ix].GetInstanceID(), frameInfos[ix]);
            }
        }

        public void ResetScrollBar()
        {
            if (scrollRect != null && scrollRect.verticalScrollbar != null)
            {
                scrollRect.verticalScrollbar.value = 1f;
            }
        }

        // 메뉴 리스트 버튼 클릭됐을 때 실행.
        public void OnButtonClicked(GameObject button)
        {
            HelperFrameInfo frameInfo = null;
            if (helperMenuDictionary.TryGetValue(button.GetInstanceID(), out frameInfo))
            {
#if USINGTMPPRO
                helperScreen.SetHelperFrameInfo(frameInfo, button.GetComponentInChildren<TMP_Text>().text);
#else
                helperScreen.SetHelperFrameInfo(frameInfo, button.GetComponentInChildren<Text>().text);
#endif
                helperScreen.gameObject.SetActive(true);
            }
        }

        public void OpenHelperPopup()
        {
            gameObject.SetActive(true);
            if (shortcutMenu.activeSelf == true)
            {
                shortcutMenu.SetActive(false);
            }
        }
        
        public void CloseButtonClicked()
        {
            helperScreen.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}