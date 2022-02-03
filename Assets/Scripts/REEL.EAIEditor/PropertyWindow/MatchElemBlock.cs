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
    public class MatchElemBlock : MonoBehaviour
    {
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;

        private MatchNodeWindow matchNodeWindow;

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

        public void SetManager(MatchNodeWindow matchNodeWindow)
        {
            this.matchNodeWindow = matchNodeWindow;
        }

        private void AddbuttonClicked()
        {
            matchNodeWindow.AddbuttonClicked();
        }

        private void RemoveButtonClicked()
        {
            matchNodeWindow.RemoveButtonClicked(this);
        }
    }
}