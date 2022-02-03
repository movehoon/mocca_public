using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class FileListItem : MonoBehaviour
    {
        [SerializeField] private Text fileNameText = null;
        [SerializeField] private Text typeNameText = null;

        private InputField inputField;
        private FileWindow fileWindow;

        public void SetFileName(string fileName)
        {
            fileNameText.text = fileName;
        }

        public void SetTypeName(string typeName)
        {
            typeNameText.text = typeName;
        }

        // Set Save/Load window.
        public void SetFileWindow(FileWindow fileWindow)
        {
            this.fileWindow = fileWindow;
        }

        public void SetInputField(InputField inputField)
        {
            this.inputField = inputField;
        }

        public void OnClicked()
        {
            if (inputField == null) return;

            fileWindow.SetSelectedProjectFileName(fileNameText);
            inputField.text = fileNameText.text;
        }
    }
}