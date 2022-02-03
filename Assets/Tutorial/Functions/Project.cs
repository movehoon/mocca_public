using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class Project : FunctionBase
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

		public override void OnClear(ObjectFlag clearFlg)
		{
			if(clearFlg.HasFlag(ObjectFlag.Project))
			{
			}
		}


		#endregion //FunctionBase Event

		public void Create(string titleName)
		{
			NewProjectWindow.CreateProject(titleName, "tutorial project" , false);
		}


		public void Load(string projectUuid)
		{

			ProjectDesc desc = new ProjectDesc()
			{
				uuid = projectUuid,
				title = "tutorial" + TutorialManager.Instance.lastTutorialIndex.ToString()
			};

			ProjectManager.Load(desc, null);

		}
	}
}
