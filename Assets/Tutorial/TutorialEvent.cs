using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tutorial
{
	public enum CustomEvent
	{
		None,					//없음
		ProjectCreated,			//프로젝트 생성됨
		NodeCreated,			//블록 생성됨
		NodeLinked,				//블록 연결됨
		NodeUnlinked,			//블록 연결 해제됨
		InputFieldSubmitted,    //노드 인풋 필드 입력 완료됨	//Contains
		DropdownSelected,		//드롭다운 값 변경

		ProjectPlayed,			//프로젝트 플레이 시작
		ProjectStopped,			//프로젝트 플레이 종료

		ParameterLinked,		//파라미터 연결됨.
		ParameterUnlinked,		//파라미터 연결 해제됨.

		MessageBoxOkButtonClicked,  // 메시지 박스 OK 버튼 클릭(예: "다음 튜토리얼로 넘어갈까요?").

		InputFieldSubmittedEqual,   //노드 인풋 필드 입력 완료됨

	};

}

