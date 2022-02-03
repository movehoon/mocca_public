using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class ConcatenatesTextsElementBlock : MonoBehaviour
    {
        [Header("Add/Remove Buttons")]
        public Button addButton;
        public Button removeButton;

        [Header("Element UI Controls")]
        public TMP_Text parameterNameText;
        public TMP_InputField inputField;

        private ConcatenateTextsNodeWindow concatenateTextsNodeWindow;

        private void OnEnable()
        {
            addButton.onClick.AddListener(AddButtonClicked);
            removeButton.onClick.AddListener(RemoveButtonClicked);
        }

        public void SetElementName(string name)
        {
            parameterNameText.text = name;
        }

        public void SetPropertyWindow(ConcatenateTextsNodeWindow concatWindow)
        {
            concatenateTextsNodeWindow = concatWindow;
        }

        private void AddButtonClicked()
        {
            concatenateTextsNodeWindow.AddButtonClicked();
        }

        private void RemoveButtonClicked()
        {
            concatenateTextsNodeWindow.RemoveButtonClicked(this);
        }
    }
}