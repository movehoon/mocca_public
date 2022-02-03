using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class Dim : FunctionBase
    {
        public GameObject   dimObject;


		#region FunctionBase Event

		//스퀀스 처음플레이때 호출 된다.
		public override void OnPlaySequence()
		{

		}

		//스퀀스(튜토리얼) 종료시 호출 된다.
		// completed = 1 이면 완료 0 이면 중도 취소
		public override void OnEndSequence(bool completed)
		{
			Hide();
		}

		public override void OnClear(ObjectFlag clearFlg)
		{
			if( clearFlg.HasFlag(ObjectFlag.Dim) )
			{
				Hide();
			}
		}


		#endregion //FunctionBase Event


		public void Show()
        {
			TutorialManager.Instance.ResetContentScaler();
			TutorialManager.Instance.ResetScrollbars();

			dimObject.SetActive(true);

			DisableAllNode();
		}

        public void Hide()
        {
			TutorialManager.Instance.ResetContentScaler();
			TutorialManager.Instance.ResetScrollbars();

			dimObject.SetActive(false);

			DisableAllNode();

		}

	}

}