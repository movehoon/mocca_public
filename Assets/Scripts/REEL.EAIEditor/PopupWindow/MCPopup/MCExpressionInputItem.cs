using UnityEngine;
using UnityEngine.UI;
using TMPro;
using REEL.Animation;

namespace REEL.D2EEditor
{
	public class MCExpressionInputItem : MonoBehaviour
	{
        //public InputField ttsInput;
        public TMPro.TMP_InputField ttsInput;
        public Dropdown faceDropdown;
        public TMP_Dropdown TMP_FaceDropdown;
        public Dropdown motionDropdown;
        public TMP_Dropdown TMP_MotionDropdown;

        private PROJECT.Expression record;

        public int FaceDropdown
        {
            get
            {
                if (TMP_FaceDropdown != null) return TMP_FaceDropdown.value;
                if (faceDropdown != null) return faceDropdown.value;
                return 0;
            }
            set
            {
                if (TMP_FaceDropdown != null) TMP_FaceDropdown.value = value;
                if (faceDropdown != null) faceDropdown.value = value;
            }
        }

        public string FaceDropdownText
        {
            get
            {
                if (TMP_FaceDropdown != null) return TMP_FaceDropdown.itemText.text;
                if (faceDropdown != null) return faceDropdown.itemText.text;
                return "";
            }
            set
            {
                if (TMP_FaceDropdown != null) TMP_FaceDropdown.itemText.text = value;
                if (faceDropdown != null) faceDropdown.itemText.text = value;
            }
        }

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

        public string MotionDropdownText
        {
            get
            {
                if (TMP_MotionDropdown != null) return TMP_MotionDropdown.itemText.text;
                if (motionDropdown != null) return motionDropdown.itemText.text;
                return "";
            }
            set
            {
                if (TMP_MotionDropdown != null) TMP_MotionDropdown.itemText.text = value;
                if (motionDropdown != null) motionDropdown.itemText.text = value;
            }
        }

        public void SetValue(PROJECT.Expression exp)
        {
            record = exp;
            Invoke("SetValueWithDelay", 0.1f);
        }

        void SetValueWithDelay()
        {
            ttsInput.text = record.tts;
            //faceDropdown.value = Constants.GetFacialExpressionIndexFromEnglish(record.facial);
            //motionDropdown.value = Constants.GetMotionExpressionIndexFromEnglish(record.motion);

            FaceDropdown = Constants.GetFacialExpressionIndexFromEnglish(record.facial);
            MotionDropdown = Constants.GetMotionExpressionIndexFromEnglish(record.motion);

            record = null;
        }

        public void ResetValue()
        {
            ttsInput.text = string.Empty;
            //faceDropdown.value = 0;
            //motionDropdown.value = 0;
            FaceDropdown= 0;
            MotionDropdown = 0;
        }

        public void PlaySingleExpression()
        {
            SpeechRenderrer.Instance.Play(ttsInput.text);
            //FindObjectOfType<RobotFacialRenderer>().Play(faceDropdown.itemText.text);
            FindObjectOfType<RobotFacialRenderer>().Play(FaceDropdownText);
            var robots = FindObjectsOfType<REEL.PoseAnimation.RobotTransformController>();
            foreach (var robot in robots)
            {
                //string motion = string.Empty;
                //if (LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR)
                //{
                //    motion = Constants.ParseMotionExpressionKoreanToEnglish(motionDropdown.value);
                //}
                //else
                //{
                //    //motion = motionDropdown.options[(motionDropdown.value)].text;
                //}

                //string motion = Constants.ParseMotionExpressionKoreanToEnglish(motionDropdown.value);
                string motion = Constants.ParseMotionExpressionKoreanToEnglish(MotionDropdown);
                robot.PlayMotion(motion);
            }
        }
    }
}