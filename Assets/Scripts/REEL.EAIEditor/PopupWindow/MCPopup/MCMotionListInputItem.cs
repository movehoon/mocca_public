using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class MCMotionListInputItem : MonoBehaviour
	{
        public Dropdown motionDropdown;
        public TMPro.TMP_Dropdown TMP_MotionDropdown;
        private string record;

        public int MotionDropdown
        {
            get
            {
                if (TMP_MotionDropdown != null) return TMP_MotionDropdown.value;
                if (motionDropdown != null) return motionDropdown.value;
                return 0;
            }
            set
            {
                if (TMP_MotionDropdown != null) TMP_MotionDropdown.value = value;
                if (motionDropdown != null) motionDropdown.value = value;
            }
        }

        public void SetValue(string value)
        {
            //motionDropdown.value = Constants.GetMotionIndexFromEnglish(value);
            record = value;
            Invoke("SetValueWithDelay", 0.1f);
        }

        public void SetValueWithDelay()
        {
            //motionDropdown.value = Constants.GetMotionIndexFromEnglish(record);
            MotionDropdown = Constants.GetMotionIndexFromEnglish(record);
            record = "";
        }

        public void ResetValue()
        {
            //motionDropdown.value = 0;
            MotionDropdown = 0;
        }
	}
}