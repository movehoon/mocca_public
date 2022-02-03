using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationGroup : MonoBehaviour
{
	// 자식 노드는 로컬처리 안함

	[Header("[다국어지원 금지 유뮤]")]
	public bool doNotLocalize = false;


	[Header("[그룹명 지정]")]
	public string textGroup = "common";

	[Header("[보조 그룹명 지정(기본 값은 common)]")]
	public string secondGroup = "common";


	[Header("[이벤트]")]
	public bool applyOnEnable = true;
	public bool applyChild = true;


	private void OnEnable()
	{
		if (applyOnEnable)
		{
			Localize();
		}
	}


	void Localize()
	{
		if (applyChild)
		{
			LocalizationManager.LocalizeComponentsInChildren(gameObject);
		}
	}

}
