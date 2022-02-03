using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCBooleanListInputItem : MonoBehaviour
    {
        public Toggle booleanToggle;

        private TextMeshProUGUI labelText;
        
        private string record = "";

        private string LabelText
        {
            get
            {
                return labelText.text;
            }
            set
            {
                labelText.text = value;
            }
        }

        private void OnEnable()
        {
            if (labelText == null)
            {
                labelText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        public void SetValue(string value)
        {
            record = value;
            Invoke("SetValueWithDelay", 0.1f);
        }

        public void SetValueWithDelay()
        {
            //booleanToggle.isOn = record == "0" ? false : true;
            if (record == "0" || record == "false")
            {
                booleanToggle.isOn = false;
            }
            else
            {
                booleanToggle.isOn = true;
            }

            record = "";
        }

        public void ResetValue()
        {
            booleanToggle.isOn = false;
        }

        public void OnBooleanToggleChanged(bool isON)
        {
            LabelText = GetBooleanStringValue(isON);
        }

        private string GetBooleanStringValue(bool isON)
        {
            if (isON == true)
            {
                if (LocalizationManager.CurrentLanguage == LocalizationManager.Language.ENG)
                {
                    return "True";
                }
                else
                {
                    return "참";
                }
            }
            else
            {
                if (LocalizationManager.CurrentLanguage == LocalizationManager.Language.ENG)
                {
                    return "False";
                }
                else
                {
                    return "거짓";
                }
            }
        }
    }
}