using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tutorial
{

	public class FunctionBase : MonoBehaviour
    {

		[Flags]
		public enum ObjectFlag
		{
			Dim         =	1 << 1,			//dim
			Mocca       =	1 << 2,			//모카 캐릭터
			MoccaText   =	1 << 3,			//모카 말풍선
			Highlight   =	1 << 4,			//오브젝트 강조 박스(또는 동그라미)
			Hand        =	1 << 5,			//클릭 손가락
			BlockBox	=	1 << 6,			//클릭 금지 영역
			WindowArea  =	1 << 7,			//윈도우 영역
			Project		=	1 << 8,
		}


		public bool isWaiting = false;

		public GameObject Root
		{
			get
			{
				return TutorialManager.Instance.canvas;
			}
		}

#region FunctionBase Event


		//사용자 이벤트 발생스퀀스 처음플레이때 호출 된다.
		public virtual void OnCustomEvent(CustomEvent ev,string text)
		{

		}


		//스퀀스 처음플레이때 호출 된다.
		public virtual void OnPlaySequence()
		{

		}

		//스퀀스 종료시 호출 된다.
		// completed = 1 이면 완료 0 이면 중도 취소
		public virtual void OnEndSequence(bool completed)
		{

		}


		//스텝 시작시 호출된다
		public virtual void OnEnterStep(ObjectFlag clearFlg)
		{
			OnClear(clearFlg);
		}

		//스텝 종료시 호출된다(클리어 요청에 각자 처리)
		public virtual void OnExitStep(ObjectFlag clearFlg)
		{
			OnClear(clearFlg);
		}

		public virtual void OnClear(ObjectFlag clearFlg)
		{

		}

		public virtual void OnSkip()
		{

		}

		#endregion //FunctionBase Event

		//클리어 처리시 호출된다.
		public virtual void OnClear()
        {
			isWaiting = false;
		}


		public void DisableAllNode()
		{
			var nodes = GameObject.FindObjectsOfType<REEL.D2EEditor.MCNode>();

			foreach(var n in nodes)
			{
				n.DisableUserControl();
			}
		}


		public void EnableNodeUnlink(string targetNodeName)
		{
			var target = FindObject(targetNodeName);

			if(target == null)
			{
				Debug.LogError("cannot find object : " + targetNodeName);
			}

			EnableNodeSocket(targetNodeName);

			var node = target.GetComponentInParent<REEL.D2EEditor.MCNode>();
			if(node == null)
			{
				Debug.LogError("cannot find node : " + targetNodeName);
			}

			node.EnableUnlink(true);
		}


		public void EnableNodeSocketType(string nodeName, REEL.D2EEditor.MCNodeSocket.SocketType socketType)
		{
			GameObject obj = FindObject(nodeName);

			if( obj != null )
			{
				var node = obj.GetComponent<REEL.D2EEditor.MCNode>();
				if( node != null)
				{
					node.EnableSocketType(socketType);
				}
			}
		}

		public void EnableNodeSocket(string nodeName)
		{
			GameObject obj = FindObject(nodeName);

			if(obj != null)
			{
				var socket = obj.GetComponent<REEL.D2EEditor.MCNodeSocket>();
				if(socket != null)
				{
					socket.enabled = true;
				}

				var mb = obj.GetComponent<REEL.D2EEditor.MagneticBound>();
				if(mb != null)
				{
					mb.enabled = true;
				}
			}
		}



		public GameObject FindObject(string nodeName)
		{
			if( nodeName.Contains("\\") )
			{
				string[] names = nodeName.Split(new char[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);
				if(names.Length != 2) return null;

				GameObject parent = FindObject(names[0]);

				if( parent != null )
				{
					var t = parent.transform.Find(names[1]);

					if( t == null )
					{
						Debug.Log("NodeName : " + nodeName);
					}
					else
					{
						return t.gameObject;
					}
				}

				return null;
			}

			//오브젝트 이름으로 찾기
			GameObject obj = GameObject.Find(nodeName);
			if(obj != null) return obj;

			//노드 타입으로 찾기 ( 말하기블록 : "SAY" , 시작블록 : "START" )
			var nodes = GameObject.FindObjectsOfType<REEL.D2EEditor.MCNode>();

			foreach( var n in nodes)
			{
				if( n.nodeData.type.ToString() == nodeName )
				{
					return n.gameObject;
				}
			}

			return null;
		}

		public string ConvertText(string text)
		{
			text = LocalizationManager.ConvertText(text, "tutorial", "common");
			text = text.Replace("\\n", "\n");

			return text;
		}
	}
}