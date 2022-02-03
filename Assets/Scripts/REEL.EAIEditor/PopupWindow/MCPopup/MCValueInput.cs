using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace REEL.D2EEditor
{
    public class MCValueInput : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField valueInputField;

        public string Input
        {
            get
            {
                return valueInputField.text;
            }
            set
            {
                valueInputField.text = value;
            }
        }

        public void SetDataType(PROJECT.DataType dataType)
        {
            Input = string.Empty;

            if (dataType == PROJECT.DataType.STRING)
            {
                valueInputField.contentType = TMP_InputField.ContentType.Standard;
                //valueInputField.characterValidation = TMP_InputField.CharacterValidation.None;
                return;
            }

            if (dataType == PROJECT.DataType.NUMBER)
            {
                valueInputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                //valueInputField.characterValidation = TMP_InputField.CharacterValidation.Decimal;
                return;
            }
        }
    }
}