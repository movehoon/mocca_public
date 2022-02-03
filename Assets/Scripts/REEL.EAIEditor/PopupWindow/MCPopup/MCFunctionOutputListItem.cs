using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace REEL.D2EEditor
{
	public class MCFunctionOutputListItem : MonoBehaviour
	{
        //public InputField nameInput;
        //public Dropdown typeDropdown;
        public TMP_InputField nameInput;
        public TMP_Dropdown typeDropdown;

        public MCFunctionOutputList outputList;

        public string OutputName { get { return nameInput.text; } }
        public int DataTypeIndex
        {
            get
            {
                return Constants.ConvertVariableTypeIndexToDataType(typeDropdown.value);
            }
        }

        private void OnEnable()
        {
            if (outputList == null)
            {
                outputList = GetComponentInParent<MCFunctionOutputList>();
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
            if (outputList == null)
            {
                Debug.Log("MCFunctionOutputList reference is null");
                return;
            }

            outputList.OnMinusClicked(this);
        }
    }
}