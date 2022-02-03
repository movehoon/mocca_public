using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCValueListInputItem : MonoBehaviour
    {
        public InputField input;
        public TMPro.TMP_InputField TMP_Input;

        private PROJECT.DataType dataType = PROJECT.DataType.NONE;

        public string Input
        {
            get
            {
                if (TMP_Input != null) return TMP_Input.text;
                if (input != null) return input.text;
                return "";
            }
            set
            {
                if (TMP_Input != null) TMP_Input.text = value;
                if (input != null) input.text = value;
            }
        }

        private string record;

        public void SetValue(string value)
        {
            Invoke("SetValueWithDelay", 0.1f);
            record = value;
        }

        public PROJECT.DataType GetDataType()
        {
            return dataType;
        }

        public void SetDataType(PROJECT.DataType dataType)
        {
            this.dataType = dataType;

            if (dataType == PROJECT.DataType.STRING)
            {
                TMP_Input.characterValidation = TMPro.TMP_InputField.CharacterValidation.None;
                return;
            }

            if (dataType == PROJECT.DataType.NUMBER)
            {
                TMP_Input.characterValidation = TMPro.TMP_InputField.CharacterValidation.Decimal;
                return;
            }
        }

        private void SetValueWithDelay()
        {
            //input.text = record;
            Input = record;
            record = "";
        }

        private void ResetValue()
        {
            //if (input != null)
            //{
            //    input.text = string.Empty;
            //}
            Input = string.Empty;
        }
    }
}