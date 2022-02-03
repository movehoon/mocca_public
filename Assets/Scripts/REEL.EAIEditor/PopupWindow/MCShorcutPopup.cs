using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    [System.Serializable]
    public class ShortCutInfo
    {
        public string command;
        public string shortcut;
    }

    public class MCShorcutPopup : MonoBehaviour
    {
        [Header("ShortCut Informations")]
        public ShortCutInfo[] shortCutInfos;

        [Header("References for instantiation")]
        public McShorcutListItem itemPrefab;
        public RectTransform shortcutInfoParent;

        [Header("References for Reset")]
        public ScrollRect scrollRect;
        public GameObject windowRoot;

        private float itemHeight = 40f;
        private bool hasInit = false;

        private void OnEnable()
        {
            if (hasInit == false)
            {
                InitializeShortCuts();
                hasInit = true;
            }

            Invoke("ResetScroll", 0.1f);
        }

        private void InitializeShortCuts()
        {
            // Create shortcut items.
            foreach (var shorcut in shortCutInfos)
            {
                var shorcutItem = Instantiate(itemPrefab, shortcutInfoParent);
                shorcutItem.SetShortCutInformation(shorcut);
            }

            // Set content recttransform's height.
            float height = itemHeight * shortCutInfos.Length + (10f * (shortCutInfos.Length - 1));
            shortcutInfoParent.sizeDelta = new Vector2(shortcutInfoParent.sizeDelta.x, height);
        }

        public void CloseButtonClicked()
        {
            ResetScroll();
            windowRoot.SetActive(false);
        }

        private void ResetScroll()
        {
            scrollRect.verticalScrollbar.value = 1.0f;
        }
    }
}