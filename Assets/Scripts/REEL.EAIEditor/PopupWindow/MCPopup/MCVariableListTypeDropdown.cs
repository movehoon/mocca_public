using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class MCVariableListTypeDropdown : MonoBehaviour
	{
        public Dropdown singleOrListDropdown;
        public Image typeImage;

        private MCVariableListPopup popup;

        public enum ESingleOrListType
        {
            SINGLE = 0, LIST = 1
        }

        public ESingleOrListType SingleOrListType { get; set; }

        private void OnEnable()
        {
            if (popup == null)
            {
                popup = GetComponentInParent<MCVariableListPopup>();
            }
                
            if (singleOrListDropdown == null)
            {
                singleOrListDropdown = GetComponent<Dropdown>();
            }   

            singleOrListDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDisable()
        {
            if (singleOrListDropdown != null)
            {
                singleOrListDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
            }
        }

        void OnDropdownValueChanged(int value)
        {
            SingleOrListType = (ESingleOrListType)value;
            Sprite sprite = singleOrListDropdown.options[value].image;
            typeImage.sprite = sprite;

            popup.OnSingleOrListChanged(SingleOrListType);
        }
    }
}