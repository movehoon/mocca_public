using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


namespace Tutorial
{
    public class MoccaCharacter : FunctionBase
    {
        public static MoccaCharacter Instance;

        public enum PositionType
        {
            None,
            Center,
            Right,
            //Left,
		}

        public GameObject       targetObject;
        public GameObject       speechBubble;
        public TMPro.TMP_Text   text;
        public TMPro.TMP_Text   text_shadow;

        [Header("for debug")]
        public float sayEndWait = 0.3f;
        public float sayTime = 0.0f;
        public bool ttsIsPlaying = false;


        [Header("position")]
        public PositionType positionType = PositionType.Center;
        public GameObject positionCenter;
        public GameObject positionRight;

        Vector3  velocity = Vector3.zero;
        public GameObject   positionTarget;
        

        #region FunctionBase Event

        //스퀀스 처음플레이때 호출 된다.
        public override void OnPlaySequence()
        {

        }

        //스퀀스 종료시 호출 된다.
        // completed = 1 이면 완료 0 이면 중도 취소
        public override void OnEndSequence(bool completed)
        {
            Hide();
            HideText();
            SpeechRenderrer.Instance.Stop();
            isWaiting = false;
        }


        public override void OnClear(ObjectFlag clearFlg)
        {
            if(clearFlg.HasFlag(ObjectFlag.Mocca))
            {
                Hide();
            }

            if(clearFlg.HasFlag(ObjectFlag.MoccaText))
            {
                HideText();
            }

            sayTime = 2.0f;

            isWaiting = false;
        }

        public override void OnSkip()
        {
            isWaiting = false;
            SpeechRenderrer.Instance.Stop();
        }

        #endregion //FunctionBase Event


        public void Show()
        {
            speechBubble.SetActive(false);
            targetObject.SetActive(true);
        }


        public void Say(string text)
        {
            speechBubble.SetActive(false);
            speechBubble.SetActive(true);
            targetObject.SetActive(true);

            sayEndWait = 0.3f;

#if False
            string localText = ConvertText(text);


            this.text.text = localText;
            this.text_shadow.text = this.text.text.Replace("<style=T>","<style=S>");

            text = RemoveStyleTag(localText);

            sayTime = (text.Length * 0.05f + 0.3f);

            if(SpeechRenderrer.Instance != null)
            {
                if (LocalizationManager.CurrentLanguage == LocalizationManager.Language.ENG)
                    SpeechRenderrer.Instance.Play(text, SpeechRenderrer.SpeakerType.danna);
                else
                    SpeechRenderrer.Instance.Play(text, SpeechRenderrer.SpeakerType.nhajun);
            }
#else
            string localText = ConvertText(text);
            string ttsText = text;
            int outIndex = text.IndexOf("]");
            if (outIndex >= 0)
            {
                ttsText = text.Insert(outIndex, "_TTS");
                ttsText = ConvertText(ttsText);
                Debug.Log("TTS_Text: " + ttsText);
            }


            this.text.text = localText;
            this.text_shadow.text = this.text.text.Replace("<style=T>","<style=S>");

            text = RemoveStyleTag(localText);

            sayTime = (ttsText.Length * 0.1f + 0.5f);

            if(SpeechRenderrer.Instance != null)
            {
                if (LocalizationManager.CurrentLanguage == LocalizationManager.Language.ENG)
                    SpeechRenderrer.Instance.Play(ttsText, SpeechRenderrer.SpeakerType.danna, false);
                else
                    SpeechRenderrer.Instance.Play(ttsText, SpeechRenderrer.SpeakerType.ndain, false);
            }
#endif
            isWaiting = true;
        }

		private string RemoveStyleTag(string text)
		{
            text = text.Replace("(", " ").Replace(")", "");
            text = text.Replace("'", " ");


            if(text.Contains("<") == false) return text;

            return Regex.Replace(text, "<.*?>", String.Empty);
        }

		public void Hide()
        {
            targetObject.SetActive(false);
        }


		public void HideText()
		{
			speechBubble.gameObject.SetActive(false);
		}

		public void Awake()
		{
            Instance = this;
        }

		private void Update()
		{
            if(positionType != PositionType.None )
            {
                positionTarget = positionType == PositionType.Center ? positionCenter : positionRight;
            }

            if(positionTarget != null)
            {
                targetObject.transform.position = Vector3.SmoothDamp(targetObject.transform.position, positionTarget.transform.position, ref velocity, 0.3f);
            }

            if(SpeechRenderrer.Instance == null)
            {
                isWaiting = false;
                return;
            }

            ttsIsPlaying = SpeechRenderrer.Instance.IsSpeaking;

            sayTime -= Time.deltaTime;
            if(sayTime > 0.0f) return;

            if(isWaiting == true )
            {
                if(ttsIsPlaying == false )
                {
                    if(sayEndWait > 0.0f)
                    {
                        sayEndWait -= Time.deltaTime;
                    }
                    else
                    {
                        isWaiting = false;
                    }
                }
            }
        }
    }
}