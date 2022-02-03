using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class Wait : FunctionBase
    {
        DateTime    timeEnter = DateTime.Now;
        
        Button          waitTargetButton = null;
        CustomEvent     waitCustomEvent = CustomEvent.None;
        string          waitCustomEventText = "";

        float time = 0.0f;

#region FunctionBase Event


        public override void OnCustomEvent(CustomEvent ev,string text)
        {

        }


        //스퀀스 처음플레이때 호출 된다.
        public override void OnPlaySequence()
        {

        }

        //스퀀스 종료시 호출 된다.
        // completed = 1 이면 완료 0 이면 중도 취소
        public override void OnEndSequence(bool completed)
        {
            base.OnClear();
            time = 0.0f;
            if(waitTargetButton != null)
            {
                waitTargetButton.onClick.RemoveListener(OnClickedButton);
                waitTargetButton = null;
            }
        }


        public override void OnClear()
        {
            base.OnClear();
            time = 0.0f;

            if(waitTargetButton != null)
            {
                waitTargetButton.onClick.RemoveListener(OnClickedButton);
                waitTargetButton = null;
            }
        }

#endregion //FunctionBase Event




        //지정한 시간만큼 대기후 다음 시퀀스로 넘어감
        public void Time(float second)
        {
            time = second;
            isWaiting = true;
            timeEnter = DateTime.Now;
        }

        //TTS 플레이가 끝날때 까지 기다린다.
        public void TTS()
        {

        }

        public void ButtonClick(string objectName)
        {
            OnClear();

            GameObject obj = FindObject(objectName);

            waitTargetButton  = obj.GetComponentInChildren<UnityEngine.UI.Button>();

            if(waitTargetButton == null )
            {
                Debug.LogError("can not find button! " + objectName);
                return;
			}

            waitTargetButton.onClick.AddListener(OnClickedButton);
            isWaiting = true;
        }

        void OnClickedButton()
        {
            isWaiting = false;
        }


        //지정한 이벤트가 발생 할떄 까지 대기함
        public void EventType(CustomEvent eventType)
        {
            waitCustomEvent = eventType;
            isWaiting = true;
        }



        ////지정한 이벤트가 발생 할떄 까지 대기하되, 다른 이벤트가 발생하면 지정한 시퀀스 번호로 이동
        //public void EventIf(string passEventName , int elseGotoSeq )
        //{

        //}


		public void Update()
		{
            //대기중 일경우
            if( isWaiting )
            {
                //버튼 대기중
                if(waitTargetButton != null)
                {
                    return;
                }

                //Wait time
                if( (DateTime.Now - timeEnter).TotalSeconds > time )
                {
                    isWaiting = false;
				}
			}
			
		}

	}
}