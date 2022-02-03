using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{

	//데이터 윈도우 영역
	[Flags]
	public enum WindowAreaFlg
	{
		Top_Window          =   1 << 1,     //상단 윈도우(메뉴)
		Left_Window         =   1 << 2,     //좌측 메뉴(블록리스트)
		Right_Window        =   1 << 3,     //우측 메뉴(로봇,카메라뷰어)
		Log_Window          =   1 << 4,     //하단 채팅 윈도우
		WorkSpace           =   1 << 5,     //중앙 블록 작업 에디터
		Tab_Block_Layout	=	1 << 6,
		//Search_Box			=	1 << 7,

		//		필요한것 추가 (GameObject 이름으로 추가)
	}

	public class WindowArea : FunctionBase
    {
		public static WindowArea Instace;

		[Header("Window Area List")]
		public GameObject           prefabWindowArea;
		public List<GameObject>     windowAreaList = new List<GameObject>();


		#region FunctionBase Event

		public WindowArea()
		{
			Instace = this;
		}

		//스퀀스 처음플레이때 호출 된다.
		public override void OnPlaySequence()
		{

		}

		//스퀀스 종료시 호출 된다.
		// completed = 1 이면 완료 0 이면 중도 취소
		public override void OnEndSequence(bool completed)
		{
			DisableAll();
		}

		public override void OnClear(ObjectFlag clearFlg)
		{
			if(clearFlg.HasFlag(ObjectFlag.WindowArea))
			{
				//모든 윈도우 영역을 사용 가능 하도록 한다.
				//EnableAll();
			}
		}


		#endregion //FunctionBase Event


		// 해당 윈도우 영역을 클릭하지 못하게 한다.
		public void Disable(string name)
		{
			GameObject obj = FindWindow(name);
			if(obj != null)
			{
				obj.SetActive(true);
			}
		}

		// 해당 윈도우 영역을 클릭가능 하도록 한다.
		public void Enable(string name)
		{
			GameObject obj = FindWindow(name);
			if(obj != null)
			{
				obj.SetActive(false);
			}
		}


		public void DisableAll()
		{
			foreach(Tutorial.WindowAreaFlg flg in Enum.GetValues(typeof(Tutorial.WindowAreaFlg)))
			{
				Enable(flg.ToString());
			}
		}

		public void EnableAll()
		{
			foreach(Tutorial.WindowAreaFlg flg in Enum.GetValues(typeof(Tutorial.WindowAreaFlg)))
			{
				Disable(flg.ToString());
			}
		}



		private GameObject FindWindow(string objectName)
		{
			GameObject obj = windowAreaList.Find( x=>x.name == objectName);

			if(obj == null)
			{
				string disableName = objectName + "_Disable";
				obj = windowAreaList.Find(x => x.name == disableName);
				if(obj == null)
				{
					disableName = disableName.Replace(" " ,"");
					obj = windowAreaList.Find(x => x.name == disableName);
					if(obj == null)
					{
						Debug.LogError(string.Format($"cannot find : {objectName} or {objectName}_Disable"));
						return null;
					}
				}
			}

			return obj;
		}

		public void Enable(WindowAreaFlg areas)
		{
			foreach(Tutorial.WindowAreaFlg flg in Enum.GetValues(typeof(Tutorial.WindowAreaFlg)))
			{
				if(areas.HasFlag(flg) )
				{
					Enable(flg.ToString());
				}
				else
				{
					Disable(flg.ToString());
				}
			}
		}


		void Start()
		{
			InitWindowArea();
		}

		private void InitWindowArea()
		{
			windowAreaList.Clear();

			foreach(Tutorial.WindowAreaFlg flg in Enum.GetValues(typeof(Tutorial.WindowAreaFlg)))
			{
				GameObject window = GameObject.Find(flg.ToString());
				if(window == null)
				{
					Debug.LogError("error! Cannot find : " + flg.ToString());
					continue;
				}

				GameObject area = GameObject.Instantiate(prefabWindowArea, window.transform);

				string areaName = flg.ToString() + "_Disable";
				area.name = areaName;

				windowAreaList.Add(area);
				area.SetActive(false);
			}
		}
	}
}