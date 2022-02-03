#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class SwitchElemBlock : MonoBehaviour
    {
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;

        private SwitchNodeWindow switchNodeWindow;

#if USINGTMPPRO
        public TMP_InputField inputText;
#else
        public InputField inputText;
#endif

        private void OnEnable()
        {
            addButton.onClick.AddListener(AddbuttonClicked);
            removeButton.onClick.AddListener(RemoveButtonClicked);
        }

        public void SetManager(SwitchNodeWindow switchNodeWindow)
        {
            this.switchNodeWindow = switchNodeWindow;
        }

        private void AddbuttonClicked()
        {
            switchNodeWindow.AddbuttonClicked();
        }

        private void RemoveButtonClicked()
        {
            switchNodeWindow.RemoveButtonClicked(this);
        }
    }
}