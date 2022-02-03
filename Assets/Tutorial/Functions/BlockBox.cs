using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class BlockBox : FunctionBase
    {

		#region FunctionBase Event

		//스퀀스 처음플레이때 호출 된다.
		public override void OnPlaySequence()
		{

		}

		//스퀀스 종료시 호출 된다.
		// completed = 1 이면 완료 0 이면 중도 취소
		public override void OnEndSequence(bool completed)
		{

		}


#endregion //FunctionBase Event


		//지정한 오브젝트의 영역만 클릭 가능 하도록 함
		public void EnableClickObject(GameObject targetObject)
        {

        }
    }
}