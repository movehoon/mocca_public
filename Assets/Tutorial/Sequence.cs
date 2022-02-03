using Malee.List;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace Tutorial
{
    public class Sequence : MonoBehaviour
    {

        [System.Serializable]
        public class WaitCustomEvent
        {
            public CustomEvent                      waitEventType = CustomEvent.None;
            public string                           waitEventString = "";
        }


        [System.Serializable]
        public class Step
        {
            [Tooltip("코멘트")]
            public string                           comment = " ";
            [Tooltip("액션 실행후 최소 대기 시간")]
            public float                            minTimeLength = 0.3f;
            public bool                             changeWindowArea;
            public WindowAreaFlg                    enableWindowArea;
            [Tooltip("스텝 시작시 클리어할 오브젝트")]
            public FunctionBase.ObjectFlag          clearFlagOnEnter;
            [Tooltip("스텝 종료시 클리어할 오브젝트")]
            public FunctionBase.ObjectFlag          clearFlagOnExit;
            public MoccaCharacter.PositionType      moccaPosition;
            public UnityEvent                       actions;
            public WaitCustomEvent                  events;

        }


        public bool isWaiting = false;

        public Step CurrentStep
        {
            get
            {
                return stepList[currentStepIndex];
            }
		}


        public bool                     hide = false;
        public bool                     active = true;

        [NonSerialized]
        public int                      currentStepIndex;
        [NonSerialized]
        public bool                     isRunning=false;


        [Header("Tutorial Comment")]
        public int                      numbering;
        public string                   title;
        public string                   content;

        [Header("Tutorial Project")]
        public string                   projectUuid;
        public string                   projectTitle;


        [System.Serializable]
        public class StepList : ReorderableArray<Step>
        {

        }

        [Header("Tutorial Sequence Steps")]

        [Reorderable]
        public StepList         stepList;


        [Header("for debug")]
        public string           currentStep = "";
        public float            timeStep = 0.0f;
        public CustomEvent      waitEventType = CustomEvent.None;
        public string           waitEventString = "";

        bool PlayCurrentStep()
        {
#if UNITY_EDITOR
            Debug.Log(string.Format("Tutorial Step : {0} ", CurrentStep.comment));
#endif

            TutorialManager.Instance.ResetContentScaler();
            TutorialManager.Instance.ResetScrollbars();


            if( CurrentStep.changeWindowArea == true )
            {
                TutorialManager.Instance.EnableWindowArea(CurrentStep.enableWindowArea);
            }

            Array.ForEach(TutorialManager.Instance.functions, x =>
            {
                x.OnEnterStep(CurrentStep.clearFlagOnEnter);
            });

            CurrentStep.actions?.Invoke();
            timeStep = 0.0f;
            isWaiting = true;

            waitEventType = CurrentStep.events.waitEventType;
            waitEventString = CurrentStep.events.waitEventString;

            currentStep = CurrentStep.comment;

            MoccaCharacter.Instance.positionType = CurrentStep.moccaPosition;

            return true;
        }


        public bool PlayStart()
		{
            currentStepIndex = 0;
            isRunning = true;

            if(stepList.Length == 0) return false;

            LoadProject();

			return PlayCurrentStep();
        }

		private void LoadProject()
		{
            if( string.IsNullOrWhiteSpace(projectUuid) ) return;

            ProjectDesc desc = new ProjectDesc()
            {
                uuid = projectUuid,
                title = string.IsNullOrWhiteSpace(projectTitle) ? "tutorial" : projectTitle
            };

            ProjectManager.Load(desc , null);
        }

		public bool PlayNextStep()
		{
            Array.ForEach(TutorialManager.Instance.functions, x =>
            {
                x.OnExitStep( CurrentStep.clearFlagOnExit);
            });
            
            currentStepIndex++;
            if(currentStepIndex >= stepList.Length)
            {
                Stop();
                return false;
            }
            return PlayCurrentStep();
        }

        public void Stop()
        {
            timeStep = 0.0f;
            isRunning = false;
        }


        public void OnCustomEvent(CustomEvent ev, string text)
        {
            if(ev != waitEventType) return;

            switch( ev )
            {
                case CustomEvent.InputFieldSubmitted:   //Contains
                {
                    string text1 = text.Replace(" ", "").ToLower();
                    string text2 = LocalizationManager.ConvertText(waitEventString, "tutorial");
                    text2 = text2.Replace(" ", "").ToLower();

                    if(text1 == text2 || text1.Contains(text2) )
                    {
                        TutorialManager.Instance.SkipSequenceStep();
                        return;
                    }

                }
                break;

                case CustomEvent.InputFieldSubmittedEqual:
                {
                    string text1 = text.Replace(" ", "").ToLower();
                    string text2 = LocalizationManager.ConvertText(waitEventString, "tutorial");
                    text2 = text2.Replace(" ", "").ToLower();

                    if(text1 == text2)
                    {
                        TutorialManager.Instance.SkipSequenceStep();
                        return;
                    }

                }
                break;

                case CustomEvent.DropdownSelected:
                {
                    if( text.Replace(" " , "") == waitEventString.Replace(" " , ""))
                    {
                        TutorialManager.Instance.SkipSequenceStep();
                    }
                }
                break;
			}

            if( ev == CustomEvent.NodeLinked ||
                ev == CustomEvent.ParameterLinked )
            {
                TutorialManager.Instance.SkipSequenceStep();
                return;
            }

            if(ev == waitEventType && text == waitEventString)
            {
                TutorialManager.Instance.SkipSequenceStep();
            }

        }

        internal void OnSkip()
		{
            timeStep = CurrentStep.minTimeLength;
            waitEventType = CustomEvent.None;
            waitEventString = "";
        }

		private void Update()
		{
			if( isWaiting == true )
            {
                timeStep += Time.deltaTime;

                if(waitEventType != CustomEvent.None)
                {
                    return;
                }

                if( timeStep >= CurrentStep.minTimeLength )
                {
                    isWaiting = false;
                }
            }
		}

	}
}