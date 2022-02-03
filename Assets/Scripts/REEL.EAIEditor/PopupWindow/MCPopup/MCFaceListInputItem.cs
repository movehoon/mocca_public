using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCFaceListInputItem : MonoBehaviour
    {
        public Dropdown faceInput;
        public TMPro.TMP_Dropdown TMP_FaceDropdown;
        private string record;

        public int FaceDropdown
        {
            get
            {
                if (TMP_FaceDropdown != null) return TMP_FaceDropdown.value;
                if (faceInput != null) return faceInput.value;
                return 0;
            }

            set
            {
                if (TMP_FaceDropdown != null) TMP_FaceDropdown.value = value;
                if (faceInput != null) faceInput.value = value;
            }
        }

        public void SetValue(string value)
        {
            //faceInput.value = Constants.GetFacialIndexFromEnglish(value);
            record = value;
            Invoke("SetValueWithDelay", 0.1f);
        }

        public void SetValueWithDelay()
        {
            //faceInput.value = Constants.GetFacialIndexFromEnglish(record);
            FaceDropdown = Constants.GetFacialIndexFromEnglish(record);
            record = "";
        }

        public void ResetValue()
        {
            //faceInput.value = 0;
            FaceDropdown = 0;
        }
	}
}