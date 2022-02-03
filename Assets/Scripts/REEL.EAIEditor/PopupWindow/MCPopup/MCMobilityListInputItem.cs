using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCMobilityListInputItem : MonoBehaviour
    {
        public Dropdown mobilityDropdown;
        public TMPro.TMP_Dropdown TMP_MobilityDropdown;
        private string record;

        public int MobilityDropdown
        {
            get
            {
                if (TMP_MobilityDropdown != null) return TMP_MobilityDropdown.value;
                if (mobilityDropdown != null) return mobilityDropdown.value;
                return 0;
            }
            set
            {
                if (TMP_MobilityDropdown != null) TMP_MobilityDropdown.value = value;
                if (mobilityDropdown != null) mobilityDropdown.value = value;
            }
        }

        public void SetValue(string value)
        {
            //mobilityDropdown.value = Constants.GetMobilityIndexFromEnglish(value);
            record = value;
            Invoke("SetValueWithDelay", 0.1f);
        }

        public void SetValueWithDelay()
        {
            //mobilityDropdown.value = Constants.GetMobilityIndexFromEnglish(record);
            MobilityDropdown = Constants.GetMobilityIndexFromEnglish(record);
            record = "";
        }

        public void ResetValue()
        {
            //mobilityDropdown.value = 0;
            MobilityDropdown = 0;
        }
    }
}