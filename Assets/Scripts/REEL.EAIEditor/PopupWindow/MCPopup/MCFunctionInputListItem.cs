using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCFunctionInputListItem : MonoBehaviour
    {
        //public InputField nameInput;
        //public Dropdown typeDropdown;
        public TMP_InputField nameInput;
        public TMP_Dropdown typeDropdown;

        public MCFunctionInputList inputList;

        public string InputName { get { return nameInput.text; } }
        public int DataTypeIndex
        {
            get
            {
                return Constants.ConvertVariableTypeIndexToDataType(typeDropdown.value);
            }
        }

        private void OnEnable()
        {
            if (inputList == null)
            {
                inputList = GetComponentInParent<MCFunctionInputList>();
            }

            typeDropdown.onValueChanged.AddListener(OnDropdownChanged);
            nameInput.onSubmit.AddListener(OnNameInputSubmit);
        }

        private void OnDisable()
        {
            typeDropdown.onValueChanged.RemoveListener(OnDropdownChanged);
            nameInput.onSubmit.RemoveListener(OnNameInputSubmit);
        }

        private void OnDropdownChanged(int value)
        {
            
        }

        private void OnNameInputSubmit(string value)
        {
            
        }

        public void ResetValue()
        {
            nameInput.text = string.Empty;
            typeDropdown.value = 0;
        }

        public void OnMinusClicked()
        {
            if (inputList == null)
            {
                Debug.Log("MCFunctionInputList reference is null");
                return;
            }

            inputList.OnMinusClicked(this);
        }
    }
}