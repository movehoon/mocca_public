using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechVisemeInfo : MonoBehaviour
{
	public Text textUi = null;
	public Text visemeTextui = null;
	public InputField inputField = null;

	public string[] ttsTestList = { "안녕하세요? 만나서 반갑습니다. 저는 모카입니다.",
									"오늘 날씨 참 좋네요",
									"가나다라마바사아자차카타파하",
									"아에이오우",
									"오빤강남스타일"};

	int testCount = 0;
	//string currentViseme = "";


	public void TestButton_Clicked()
	{
		if (inputField == null) return;

		if (string.IsNullOrWhiteSpace(inputField.text)) return;

		var tts = FindObjectOfType<SpeechRenderrer>();

		tts.Play(inputField.text);
	}

	public void TestButton2_Clicked()
	{

		var tts = FindObjectOfType<SpeechRenderrer>();

		testCount = testCount % ttsTestList.Length;
		tts.Play(ttsTestList[testCount]);

		if (inputField != null)
		{
			inputField.text = ttsTestList[testCount];
		}

		testCount++;

	}


	// Use this for initialization
	void Start ()
	{
		
	}
	

	// Update is called once per frame
	void Update ()
	{
		if(textUi != null)
		{
			textUi.text = SpeechRenderrer.LastTTS;
		}

		if(visemeTextui != null )
		{
			string text = RobotOvrLipSyncContext.VismeString;

			if (string.IsNullOrWhiteSpace(text) == false )
			{
				visemeTextui.text = RobotOvrLipSyncContext.VismeString;
			}
		}

		
	}




}
